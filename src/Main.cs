using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public static class LittleLibMain {
    public static Batcher Batcher { get; private set; } = new();
    public static Target Target { get; private set; }
    public static Transition? Transition { get; private set; }

    public static void Init(Point2 viewport) {
        Assets.Init();
        LevelManager.Init();

        //! FIXME (Alex): Only do this when in debug mode
        Debug.Init();

        Target = new(viewport.X, viewport.Y);
        Camera.Viewport = viewport;
    }

    public static void Update() {
        TimeManager.Update();
        UIManager.Update();
        EntityManager.Update();
        Camera.ActiveCamera.Update();
        Transition?.Update();
    }

    public static void SetTransition(Transition transition) {
        //! FIXME (Alex): Do I want to ignore like this? theres probably a more elegant way
        if (Transition != null) { return; }
        Transition = transition;
    }

    public static void EndTransition() {
        Transition = null;
    }

    public static void Render(Color? backgroundColor = null, Color? letterboxColor = null) {
        Target.Clear(backgroundColor ?? Color.White);

        LevelManager.ActiveLevel.Render(Batcher);
        UIManager.Render(Batcher);
        Transition?.Render(Batcher);
        Batcher.Render(Target);

        //! FIXME (Alex): Only run this in debug mode
        Debug.Render(Batcher);
        Batcher.Render(Target);

        ScreenScaler.Render(Target, Batcher, letterboxColor ?? Color.Black);
    }
}