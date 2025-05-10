using Foster.Framework;

namespace LittleLib;

public class LittleGame : App {
    public Assets Assets;
    public TimeManager Timers;
    public AudioManager Audio;

    ScreenScaler Scaler;

    public Rng Random = new(DateTime.Now.Millisecond);

    public Batcher Batcher { get; private set; }

    Actor root;
    public Actor Root {
        get => root;
        set {
            //! FIXME (Alex): Probably do unloading stuff here
            if (value == null || root == value) { return; }
            if (root.IsValid) {
                root.ExitTree();
            }

            root = value;
            root.EnterTree();
        }
    }

    // Debug Debug;

    record struct QueuedInvalidation(Actor Actor, bool InvalidateChildren);
    readonly List<QueuedInvalidation> InvalidateQueue = [];

    public LittleGame() : base(new AppConfig() {
        //! FIXME (Alex): Temp data, need to probably load from a config?
        ApplicationName = "Test App",
        WindowTitle = "Test Window",
        Width = 800,
        Height = 800,
        Resizable = true
    }) {
        Assets = new(GraphicsDevice);
        Timers = new(this);
        Audio = new(this);

        //! FIXME (Alex): Needs viewport size config
        Scaler = new(this, new(800, 800));

        Batcher = new(GraphicsDevice);

        root = Actor.Invalid;

        //! FIXME (Alex): Only do this when in debug mode
        // Debug = new();
    }

    protected override void Startup() { }

    protected override void Shutdown() {
        Audio.Shutdown();
    }

    protected override void Update() {
        Timers.Update();
        Audio.Update();

        Root?.Update();

        foreach (QueuedInvalidation item in InvalidateQueue) {
            item.Actor.Invalidate(item.InvalidateChildren);
        }
    }

    protected override void Render() {
        //! FIXME (Alex): Needs background and letterbox colors
        Window.Clear(Color.Black);

        Root?.Render(Batcher);

        //! FIXME (Alex): Only run this in debug mode
        //! FIXME (Alex): Should this be rendered at a higher res?
        // Debug.Render(Batcher);

        // Scaler operates under the assumtion of:
        //  1. Game clears window
        //  2. Game batches all draw calls
        //  3. Game sends batchers filled with calls to scaler
        //  4. Scaler renders scaled
        //  5. scaler clears batcher
        // Unsure if this is the best pipeline but it could be used in the future to build multiple renderer types
        Scaler.Render(Batcher);
    }

    public void QueueInvalidate(Actor actor, bool invalidateChildren) {
        InvalidateQueue.Add(new(actor, invalidateChildren));
    }
}