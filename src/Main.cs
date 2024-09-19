using System.Numerics;
using System.Reflection;
using System.Text;
using Foster.Framework;

namespace LittleLib;

public static class LittleLibMain {
    public static Batcher Batcher { get; private set; } = new();
    public static Target Target { get; private set; }
    public static Transition? Transition { get; private set; }

    // public delegate void TransitionStart();
    // public delegate void LevelChange();
    // public delegate void TransitionEnd();
    // public delegate void PreUpdate();
    // public delegate void PostUpdate();
    public delegate void Callback();
    public delegate void RenderCallback(Batcher batcher);
    public static Callback TransitionStart = () => { };
    public static Callback PreLevelChange = () => { };
    public static Callback PostLevelChange = () => { };
    public static Callback TransitionEnd = () => { };
    public static Callback PreUpdate = () => { };
    public static Callback PostUpdate = () => { };
    public static RenderCallback PreRender = (batcher) => { };
    public static RenderCallback PostRender = (batcher) => { };

    public static void Init(Point2 viewport) {
        Assets.Init();
        LevelManager.Init();

        //! FIXME (Alex): Only do this when in debug mode
        Debug.Init();

        Target = new(viewport.X, viewport.Y);
        Camera.Viewport = viewport;
    }

    public static void Update() {
        PreUpdate();

        TimeManager.Update();
        UIManager.Update();
        EntityManager.Update();
        Camera.ActiveCamera.Update();
        Transition?.Update();

        PostUpdate();
    }

    public static void SetTransition(Transition transition) {
        //! FIXME (Alex): Do I want to ignore like this? theres probably a more elegant way
        if (Transition != null) { return; }

        TransitionStart();
        Transition = transition;
    }

    public static void EndTransition() {
        TransitionEnd();
        Transition = null;
    }

    public static void Render(Color? backgroundColor = null, Color? letterboxColor = null) {
        Batcher.Clear();
        Target.Clear(backgroundColor ?? Color.White);

        PreRender(Batcher);

        LevelManager.ActiveLevel.Render(Batcher);
        UIManager.Render(Batcher);
        Transition?.Render(Batcher);

        PostRender(Batcher);

        Batcher.Render(Target);

        //! FIXME (Alex): Only run this in debug mode
        Debug.Render(Batcher);
        Batcher.Render(Target);

        ScreenScaler.Render(Target, Batcher, letterboxColor ?? Color.Black);
    }
}