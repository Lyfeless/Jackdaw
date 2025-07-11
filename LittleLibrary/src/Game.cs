using System.Text.Json;
using Foster.Framework;

namespace LittleLib;

public class LittleGame : App {
    public Assets Assets;
    public Controls Controls;
    public TimeManager Timers;
    public CollisionManager Collision;
    public EventBus Events;
    public AudioManager Audio;

    public Rng Random = new(DateTime.Now.Millisecond);

    readonly LittleGameRenderer Renderer;
    public Batcher Batcher { get; private set; }
    public Viewspace Viewspace { get; private set; }

    //! FIXME (Alex): Need some way to dynamically store global systems, perhaps a childcontainer of components or actors?

    Actor root;
    public Actor Root {
        get => root;
        set {
            if (value == null || root == value) { return; }
            if (root.IsValid) {
                root.ExitTree();
            }

            root = value;
            root.EnterTree();
        }
    }

    record struct QueuedInvalidation(Actor Actor, bool InvalidateChildren);
    readonly List<QueuedInvalidation> InvalidateQueue = [];

    public LittleGame(string configPath)
        : this(JsonSerializer.Deserialize(File.ReadAllText(configPath), SourceGenerationContext.Default.LittleGameConfig)) { }

    public LittleGame(LittleGameConfig config) : base(new AppConfig() {
        ApplicationName = config.ApplicationName,
        WindowTitle = config.WindowTitle,
        Width = config.Window.WindowWidth,
        Height = config.Window.WindowHeight,
        Resizable = true
    }) {
        Assets = new(GraphicsDevice, config.Content);
        Controls = new(Input);
        Timers = new(this);
        Collision = new();
        Events = new();
        Audio = new();

        //! FIXME (Alex): YUCK
        switch (config.Window.Renderer) {
            case LittleGameWindowConfig.RendererType.FULL_WINDOW:
                Viewspace = new(new(config.Window.WindowWidth, config.Window.WindowHeight));
                Renderer = new FullWindowRenderer(this);
                break;
            case LittleGameWindowConfig.RendererType.FIXED_VIEWPORT:
            default:
                Viewspace = new(new(config.Window.ViewportWidth, config.Window.ViewportHeight));
                Renderer = new FixedViewportRenderer(this, Viewspace.Size);
                break;
        }

        foreach (LittleGameAudioBusConfig bus in config.Audio.Buses) {
            Audio.AddBus(bus.Name, bus.Parent != string.Empty ? bus.Parent : null, bus.DefaultVolume);
        }
        if (config.Audio.DefaultBus != string.Empty) { Audio.SetDefaultBus(config.Audio.DefaultBus); }

        Batcher = new(GraphicsDevice);

        root = Actor.Invalid;
    }

    protected override void Startup() { }

    protected override void Shutdown() {
        Audio.Shutdown();
    }

    protected override void Update() {
        Controls.Update();
        Timers.Update();
        Audio.Update();

        Root?.Update();
        Collision.Update();

        foreach (QueuedInvalidation item in InvalidateQueue) {
            item.Actor.Invalidate(item.InvalidateChildren);
        }
    }

    protected override void Render() => Renderer.Render(Batcher, Root);

    public void QueueInvalidate(Actor actor, bool invalidateChildren) => InvalidateQueue.Add(new(actor, invalidateChildren));
}