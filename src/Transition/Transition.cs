using Foster.Framework;

namespace LittleLib;

public class Transition(TransitionAnimation exit, TransitionAnimation enter, string targetLevel) {

    readonly TransitionAnimation Exit = exit;
    readonly TransitionAnimation Enter = enter;
    TransitionAnimation Active = exit;

    readonly string TargetLevel = targetLevel;

    public virtual void Update() {
        if (Active.Timer.Done) {
            if (Active == Exit) {
                Active = Enter;
                Enter.Timer.Restart();

                LevelManager.SetActiveLevel(TargetLevel);
            }
            else {
                LittleLibMain.EndTransition();
            }
        }
    }

    public virtual void Render(Batcher batcher) {
        Active.Render(batcher);
    }
}