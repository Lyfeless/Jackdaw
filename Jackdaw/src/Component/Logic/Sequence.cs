namespace Jackdaw;

/// <summary>
/// Component responsible for playing back a given sequence.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="sequence">The sequence to run.</param>
public class SequenceComponent(Game game, Sequence sequence) : Component(game) {
    Sequence sequence = sequence;

    /// <summary>
    /// The sequence to run.
    /// Cancel the current running sequence if changed mid-execution.
    /// </summary>
    public Sequence Sequence {
        get => sequence;
        set {
            Cancel();
            sequence = value;
        }
    }

    SingleSequenceRunner? Runner = null;

    /// <summary>
    /// If the sequence is currently running.
    /// </summary>
    public bool Running => Runner != null;

    /// <summary>
    /// If the sequence has finished running.
    /// </summary>
    public bool Done { get; private set; } = false;

    /// <summary>
    /// If the sequence should restart from the beginning when finished.
    /// </summary>
    public bool LoopOnFinish = false;

    /// <summary>
    /// If the component should automatically remove itself when finished.
    /// </summary>
    public bool RemoveOnFinish = false;

    protected override void Update() {
        if (!Running) { return; }
        switch (Runner!.IsDone()) {
            case SequenceRunnerState.CANCEL:
                Cancel();
                break;
            case SequenceRunnerState.ELEMENT_COMPLETE:
                Finish();
                break;
        }
    }

    public void Run() {
        if (Running) { Cancel(); }
        Runner = new SingleSequenceRunner(Game, sequence);
    }

    void Finish() {
        if (LoopOnFinish) {
            Run();
            return;
        }

        if (RemoveOnFinish) {
            QueueInvalidate();
            return;
        }

        Done = true;
        Runner = null;
    }

    public void Cancel() {
        if (!Running) { return; }
        Runner!.Cancel();
        Runner = null;
        Done = false;
    }
}