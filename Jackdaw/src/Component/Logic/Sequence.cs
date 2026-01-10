namespace Jackdaw;

public class SequenceComponent(Game game, Sequence sequence) : Component(game) {
    Sequence sequence = sequence;

    public Sequence Sequence {
        get => sequence;
        set {
            Cancel();
            sequence = value;
        }
    }

    SingleSequenceRunner? Runner = null;
    public bool Running => Runner != null;
    public bool Done { get; private set; } = false;

    public bool LoopOnFinish = false;
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
        Console.WriteLine("RUN!");
        Runner = new SingleSequenceRunner(Game, sequence);
    }

    void Finish() {
        Console.WriteLine("FINISH!");

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
        Console.WriteLine("CANCEL!");
        Runner!.Cancel();
        Runner = null;
        Done = false;
    }
}