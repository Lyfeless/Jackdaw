namespace Jackdaw;

public class SequenceElementSubSequences(int requiredFinishedSequences, params Sequence[] sequences) : ISequenceElement {
    public ISequenceElementRunner GetRunner(Game game)
        => new SequenceElementSubSequencesRunner(game, requiredFinishedSequences, sequences);
}

public class SequenceElementSubSequencesRunner(Game game, int requiredFinishedSequences, params Sequence[] sequences) : ISequenceElementRunner {
    public int RequiredFinishedSequences = requiredFinishedSequences;
    public SingleSequenceRunner[] Sequences = [.. sequences.Select(e => new SingleSequenceRunner(game, e))];

    public SequenceRunnerState IsDone() {
        bool isCancelled = false;
        int finishedCount = 0;

        foreach (SingleSequenceRunner runner in Sequences) {
            SequenceRunnerState state = runner.IsDone();
            switch (state) {
                case SequenceRunnerState.CANCEL:
                    isCancelled = true;
                    break;
                case SequenceRunnerState.ELEMENT_COMPLETE:
                    finishedCount++;
                    break;
            }
        }

        if (isCancelled) {
            //! FIXME (Alex): MAKE SURE CANCEL ACTION GETS RUN IF THIS HAPPENS
            return SequenceRunnerState.CANCEL;
        }

        if (finishedCount >= RequiredFinishedSequences) {
            return SequenceRunnerState.ELEMENT_COMPLETE;
        }

        return SequenceRunnerState.ELEMENT_INCOMPLETE;
    }

    public void Cancel() {
        foreach (SingleSequenceRunner runner in Sequences) {
            runner.Cancel();
        }
    }
}