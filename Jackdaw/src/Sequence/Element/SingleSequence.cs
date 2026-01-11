namespace Jackdaw;

internal class SingleSequenceRunner : ISequenceElementRunner {
    readonly Game Game;
    readonly Sequence Sequence;

    int index = 0;
    ISequenceElementRunner? Current = null;

    public SingleSequenceRunner(Game game, Sequence sequence) {
        Game = game;
        Sequence = sequence;
        if (!Sequence.SequenceDone(index)) {
            Current = Sequence[index].GetRunner(Game);
        }
    }

    public SequenceRunnerState IsDone() {
        if (Current == null) { return SequenceRunnerState.ELEMENT_COMPLETE; }
        SequenceRunnerState state = Current.IsDone();
        switch (state) {
            case SequenceRunnerState.CANCEL:
                Current.Cancel();
                return SequenceRunnerState.CANCEL;
            case SequenceRunnerState.ELEMENT_COMPLETE:
                index++;
                if (Sequence.SequenceDone(index)) { return SequenceRunnerState.ELEMENT_COMPLETE; }
                Current = Sequence[index].GetRunner(Game);
                break;
        }

        return SequenceRunnerState.ELEMENT_INCOMPLETE;
    }

    public void Cancel() {
        Current?.Cancel();
    }
}