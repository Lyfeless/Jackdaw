namespace Jackdaw;

/// <summary>
/// Component responsible for playing back a given sequence. <br/>
/// Not visible by default.
/// </summary>
public class SequenceComponent : Component {
    Sequence sequence;

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

    /// <summary>
    /// If the component should automatically run the sequence when added to the node tree.
    /// </summary>
    public bool Autostart = false;

    /// <param name="game">The current game instance.</param>
    /// <param name="sequence">The sequence to run.</param>
    public SequenceComponent(Game game, Sequence sequence) : base(game) {
        this.sequence = sequence;
        Visible = false;
    }

    protected override void EnterTree() {
        if (Autostart) { Run(); }
    }

    protected override void ExitTree() {
        Cancel();
    }

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

    /// <summary>
    /// Begin sequence execution.
    /// </summary>
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

    /// <summary>
    /// Stop the sequence from running.
    /// </summary>
    public void Cancel() {
        if (!Running) { return; }
        Runner!.Cancel();
        Runner = null;
        Done = false;
    }
}