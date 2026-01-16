using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// The main entry point for a game and storage for all management utilies.
/// Created using a <seealso cref="GameConfig" />, either defined locally or with an external config file. <br/>
/// Start the game with <see cref="Start" /> to automatically handle crashlog creation when running in release.
/// </summary>
public class Game : App {
    /// <summary>
    /// Asset data storage.
    /// </summary>
    public Assets Assets;

    /// <summary>
    /// Utilities for input management.
    /// </summary>
    public Controls Controls;

    /// <summary>
    /// Trackers for non-realtime timing utilities.
    /// </summary>
    public TimeManager Timers;

    /// <summary>
    /// Collision system for any <seealso cref="CollisionComponent" /> objects. <br/>
    /// Currently has no broadphase support, all collisions need to be handled with the supplied manual check functions.
    /// </summary>
    public CollisionManager Collision;

    /// <summary>
    /// Event system.
    /// </summary>
    public EventBus Events;

    /// <summary>
    /// Conversion functions between many common coordinate spaces.
    /// </summary>
    public SpaceConverter Convert;

    /// <summary>
    /// Random Utility
    /// </summary>
    public Rng Random = new(DateTime.Now.Millisecond);

    Color BackgroundColor;

    readonly Batcher Batcher;

    Actor root;

    /// <summary>
    /// Root actor for the primary node tree.
    /// The actor assigned to root and all children will be automatically updated and rendered.
    /// </summary>
    public Actor Root {
        get => root;
        set {
            if (value == null || root == value) { return; }
            if (root.IsValid) {
                root.ExitTree();
            }

            root = value;
            root.EnterTree();
            if (containersLocked) { root.SetContainerQueuing(true); }
        }
    }

    readonly List<IElementInvalidation> InvalidateQueue = [];

    bool containersLocked = false;

    /// <summary>
    /// Create a new game instance using a configuration file.
    /// </summary>
    /// <param name="configPath">The path to the configuration file, relative to the application.</param>
    public Game(string configPath)
        : this(JsonSerializer.Deserialize(File.ReadAllText(configPath), SourceGenerationContext.Default.GameConfig)) { }

    /// <summary>
    /// Create a new game instance using manually defined configuration data.
    /// </summary>
    /// <param name="config">The game's configuration data.</param>
    public Game(GameConfig config) : base(new AppConfig() {
        ApplicationName = config.ApplicationName,
        WindowTitle = config.Window.Title,
        Width = config.Window.Width,
        Height = config.Window.Height,
        Resizable = true
    }) {
        Assets = new(GraphicsDevice, config.Content);
        Controls = new(Input);
        Timers = new(this);
        Collision = new();
        Events = new();
        Convert = new(this);

        BackgroundColor = config.Window.ClearColor;
        Window.Resizable = config.Window.Resizable;

        Batcher = new(GraphicsDevice);

        root = Actor.Invalid;
    }

    /// <summary>
    /// Primary class start point, run to begin the update loop.
    /// Automatically handles crashlog creation when running in release.
    /// </summary>
    public void Start() {
#if DEBUG
        // Crash normally in debug so error debugging still works
        Run();
#else
        // Send to crashlog in release
        try {
            Run();
        }
        catch (Exception e) {
            using FileStream stream = File.OpenWrite("crashlog.txt");
            using StreamWriter writer = new(stream);
            writer.WriteLine(e.ToString());
        }
#endif
    }

    protected override void Startup() { }

    protected override void Shutdown() { }

    protected override void Update() {
        if (!containersLocked) {
            containersLocked = true;
            Root?.SetContainerQueuing(true);
        }

        // Run before main tick to ensure events before setup are applied
        Events.ProcessDispatchQueue();

        Controls.Update();
        Timers.Update();

        Root?.Update();

        Collision.Update();

        Root?.ApplyChanges();
        if (RunInvalidation()) {
            Root?.ApplyChanges();
        }
    }

    protected override void Render() {
        Window.Clear(BackgroundColor);
        Batcher.Clear();

        Batcher.PushBlend(BlendMode.NonPremultiplied);
        root?.Render(Batcher);
        Batcher.Render(Window);
    }

    internal void QueueInvalidate(Component component)
        => InvalidateQueue.Add(new ComponentInvalidation(component));
    internal void QueueInvalidate(Actor actor, bool invalidateChildren = true, bool invalidateComponents = true)
        => InvalidateQueue.Add(new ActorInvalidation(actor, invalidateChildren, invalidateComponents));
    bool RunInvalidation() {
        if (InvalidateQueue.Count == 0) { return false; }
        for (int i = 0; i < InvalidateQueue.Count; ++i) {
            InvalidateQueue[i].Invalidate();
        }
        InvalidateQueue.Clear();
        return true;
    }
}