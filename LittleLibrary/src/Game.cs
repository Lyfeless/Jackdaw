using System.Numerics;
using System.Text.Json;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// The main entry point for a game and storage for all management utilies.
/// Created using a <seealso cref="LittleGameConfig"/>, either defined locally or with an external config file. <br/>
/// Start the game with <see cref="Start"> to automatically handle crashlog creation when running in release.
/// </summary>
public class LittleGame : App {
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
    /// Collision system for any <seealso cref="CollisionComponent"> objects. <br/>
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
    public SpaceConverter CoordSpace;

    /// <summary>
    /// Random Utility
    /// </summary>
    public Rng Random = new(DateTime.Now.Millisecond);

    readonly LittleGameRenderer Renderer;
    public Batcher Batcher { get; private set; }
    public Viewspace Viewspace { get; private set; }

    public bool LockContainers { get; private set; } = false;

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
        }
    }

    record struct ComponentInvalidation(Component Component);
    record struct ActorInvalidation(Actor Actor, bool InvalidateChildren, bool InvalidateComponents);
    readonly List<ComponentInvalidation> ComponentInvalidateQueue = [];
    readonly List<ActorInvalidation> ActorInvalidateQueue = [];

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
        CoordSpace = new(this);

        //! FIXME (Alex): YUCK
        switch (config.Window.Renderer) {
            case LittleGameWindowConfig.RendererType.FULL_WINDOW:
                Viewspace = new(new(config.Window.WindowWidth, config.Window.WindowHeight));
                Renderer = new FullWindowRenderer(this);
                break;
            case LittleGameWindowConfig.RendererType.FIXED_VIEWPORT:
            default:
                Viewspace = new(new(config.Window.ViewportWidth, config.Window.ViewportHeight));
                FixedViewportRenderer fixedViewport = new(this, Viewspace.Size) {
                    ViewportColor = Color.FromHexStringRGB(config.Window.ViewportColor)
                };
                Renderer = fixedViewport;
                break;
        }
        Renderer.ClearColor = Color.FromHexStringRGB(config.Window.ClearColor);
        Window.Resizable = config.Window.Resizable;

        Batcher = new(GraphicsDevice);

        root = Actor.Invalid;
    }

    /// <summary>
    /// Primary class start point, run to begin the update loop.
    /// Automatically handles crashlog creation when running in release.
    /// </summary>
    public void Start() {
        LockContainers = true;

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
        Controls.Update();
        Timers.Update();

        Root?.Update();

        Collision.Update();

        if (ComponentInvalidateQueue.Count > 0) {
            foreach (ComponentInvalidation item in ComponentInvalidateQueue) {
                item.Component.OnInvalidated();
            }
            ComponentInvalidateQueue.Clear();
        }

        if (ActorInvalidateQueue.Count > 0) {
            foreach (ActorInvalidation item in ActorInvalidateQueue) {
                item.Actor.Invalidate(item.InvalidateChildren, item.InvalidateComponents);
            }
            ActorInvalidateQueue.Clear();
        }

        Root?.ApplyChanges();
    }

    protected override void Render() => Renderer.Render(Batcher, Root);

    /// <summary>
    /// Convert a coordinate from a window coordinate to a position in the current viewspace.
    /// Most useful when using the fixed viewport <seealso cref="RendererType(in LittleGameWindowConfig)">.
    /// This functionality is also available in <see cref="CoordSpace"> for consistency.
    /// </summary>
    /// <param name="position">The position relative to the full window.</param>
    /// <returns>The position transformed to be local to the adjusted viewport.</returns>
    public Vector2 WindowToViewspace(Vector2 position) => Renderer.WindowToViewspace(position);

    /// <summary>
    /// Convert a coordinate from a coordinate in the current viewspace to a window coordinate.
    /// Most useful when using the fixed viewport <seealso cref="RendererType(in LittleGameWindowConfig)">.
    /// This functionality is also available in <see cref="CoordSpace"> for consistency.
    /// </summary>
    /// <param name="position">The position relative to the viewport.</param>
    /// <returns>The position transformed to be local to the full window.</returns>
    public Vector2 ViewspaceToWindow(Vector2 position) => Renderer.ViewspaceToWindow(position);

    internal void QueueInvalidate(Component component) => ComponentInvalidateQueue.Add(new(component));
    internal void QueueInvalidate(Actor actor, bool invalidateChildren = true, bool invalidateComponents = true) => ActorInvalidateQueue.Add(new(actor, invalidateChildren, invalidateComponents));
}