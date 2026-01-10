namespace Jackdaw;

public class SequenceElementCancel() : ISequenceElement {
    public ISequenceElementRunner GetRunner(Game game)
        => new SequenceElementCancelRunner();
}

public class SequenceElementCancelRunner() : ISequenceElementRunner {
    public SequenceRunnerState IsDone() => SequenceRunnerState.CANCEL;

    public void Cancel() { }
}