using System.Collections;
using System.Collections.Concurrent;
using Foster.Framework;

namespace Jackdaw;

internal class AssetLoaderPipeline {
    public enum QueueAction {
        LOAD_COLLECTION,
        UNLOAD_COLLECTION
    }

    struct QueueElement(AssetCollection collection, QueueAction action) {
        public readonly AssetCollection Collection = collection;
        public readonly QueueAction Action = action;
        public readonly Semaphore Semaphore = new(0, 1);
    }

    readonly Assets Assets;
    readonly List<AssetLoaderStage> Stages = [];

    readonly Thread QueueWorkerThread;

    readonly HashSet<string> LoadedCollections = [];
    readonly ConcurrentQueue<QueueElement> LoadQueue = [];

    QueueElement InterruptRunner = new();
    bool ShouldInterrupt = false;

    bool IsQueueFinished => !IsQueueRunning;
    bool IsQueueRunning => QueueWorkerThread.IsAlive;

    public AssetLoaderPipeline(Assets assets) {
        Assets = assets;
        QueueWorkerThread = new(QueueWorker);
    }

    /// <summary>
    /// Find an asset loader stage in the queue by type. Currently only one loader of each type is supported.
    /// </summary>
    /// <typeparam name="T">The loader type to find.</typeparam>
    /// <returns>The asset loader, null if no loader matches the given type.</returns>
    public T? Find<T>() where T : AssetLoaderStage => (T?)Stages.FirstOrDefault(e => e.GetType() == typeof(T));

    public void Register(AssetLoaderStage loader) {
        if (Stages.Any(e => e.GetType() == loader.GetType())) {
            Log.Warning($"Asset Loader: Trying to add a second loader of type {loader.GetType()}. Behavior is currently unsupported, skipping.");
            return;
        }
        Stages.Add(loader);
    }

    public AssetProviderItem[] GetLoadOptions() {
        IEnumerable<AssetProviderItem> options = [];
        foreach (AssetLoaderStage stage in Stages) {
            options = options.Concat(stage.GetLoadOptions(Assets));
        }
        return [.. options];
    }

    public void RunLoad(AssetCollection collection) {
        if (!CanLoadCollection(collection.Name)) { return; }
        Run(collection, QueueAction.LOAD_COLLECTION);
    }

    public void RunUnload(AssetCollection collection) {
        if (!CanUnloadCollection(collection.Name)) { return; }
        Run(collection, QueueAction.UNLOAD_COLLECTION);
    }

    void Run(AssetCollection collection, QueueAction action) {
        QueueElement element = new(collection, action);
        if (IsQueueRunning) {
            SetInterrupt(element);
            element.Semaphore.WaitOne();
        }
        else {
            RunQueueLoad(element);
        }
    }

    public void RunLoadAsync(AssetCollection collection) {
        if (!CanLoadCollection(collection.Name)) { return; }
        RunAsync(collection, QueueAction.LOAD_COLLECTION);
    }

    public void RunUnloadAsync(AssetCollection collection) {
        if (!CanUnloadCollection(collection.Name)) { return; }
        RunAsync(collection, QueueAction.UNLOAD_COLLECTION);
    }

    public void RunAsync(AssetCollection collection, QueueAction action) {
        LoadQueue.Enqueue(new(collection, action));
        TryStartQueue();
    }

    public void WaitForLoad(string collection) => WaitFor(collection, QueueAction.LOAD_COLLECTION);
    public void WaitForUnload(string collection) => WaitFor(collection, QueueAction.UNLOAD_COLLECTION);

    void WaitFor(string collection, QueueAction action) {
        if (IsQueueFinished) { return; }
        QueueElement? element = LoadQueue.FirstOrDefault(e => e.Collection.Name == collection && e.Action == action);
        if (element == null) { return; }
        ((QueueElement)element).Semaphore.WaitOne();
    }

    public void WaitForLoadAll() => WaitForAll(QueueAction.LOAD_COLLECTION);
    public void WaitForUnloadAll() => WaitForAll(QueueAction.UNLOAD_COLLECTION);

    void WaitForAll(QueueAction action) {
        if (IsQueueFinished) { return; }

        // Loop wait in the rare case a loader requests a new collection to be loaded/unloaded
        while (true) {
            QueueElement? element = LoadQueue.LastOrDefault(e => e.Action == action);
            if (element == null) { return; }
            ((QueueElement)element).Semaphore.WaitOne();
        }
    }

    public void WaitForQueueFinish() => QueueWorkerThread.Join();

    public bool IsLoaded(string name) {
        lock (LoadedCollections) {
            return LoadedCollections.Contains(name);
        }
    }

    public bool IsLoading(string name) => LoadQueue.Any(e => e.Collection.Name == name);

    public bool IsLoadedOrLoading(string name) => IsLoaded(name) || IsLoading(name);

    public bool IsUnloaded(string name) => !IsLoadedOrLoading(name);

    void SetInterrupt(QueueElement element) {
        ShouldInterrupt = true;
        InterruptRunner = element;
    }

    void ClearInterrupt() {
        ShouldInterrupt = false;
        InterruptRunner = new();
    }

    void TryStartQueue() {
        if (IsQueueRunning) { return; }
        QueueWorkerThread.Start();
    }

    void QueueWorker() {
        while (NextElement(out QueueElement element)) {
            switch (element.Action) {
                case QueueAction.LOAD_COLLECTION: RunQueueLoad(element); break;
                case QueueAction.UNLOAD_COLLECTION: RunQueueUnload(element); break;
            }
            element.Semaphore.Release();
        }
    }

    void RunQueueLoad(QueueElement element) {
        new AssetLoaderRunner(element.Collection, AssetLoaderRunner.RunnerActionLoad).Run(Assets, Stages);
        LoadedCollections.Add(element.Collection.Name);
    }

    void RunQueueUnload(QueueElement element) {
        new AssetLoaderRunner(element.Collection, AssetLoaderRunner.RunnerActionUnload).Run(Assets, Stages);
        LoadedCollections.Remove(element.Collection.Name);
    }

    bool NextElement(out QueueElement element) {
        if (ShouldInterrupt) {
            element = InterruptRunner;
            ClearInterrupt();
            return true;
        }

        return LoadQueue.TryDequeue(out element);
    }

    bool CanLoadCollection(string name) {
        if (IsLoaded(name)) {
            Log.Warning($"Asset Loader: Asset collection {name} is already loaded, skipping load.");
            return false;
        }

        if (IsLoading(name)) {
            Log.Warning($"Asset Loader: Asset collection {name} is already loading, skipping load.");
            return false;
        }

        return true;
    }

    bool CanUnloadCollection(string name) {
        if (IsUnloaded(name)) {
            Log.Warning($"Asset Loader: Asset collection {name} is already unloaded, skipping unload.");
            return false;
        }

        return true;
    }
}