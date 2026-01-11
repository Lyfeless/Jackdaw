namespace Jackdaw;

/// <summary>
/// A sequence element for cancelling sequence playback.
/// </summary>
public class SequenceElementCancel() : ISequenceElement {
    public ISequenceElementRunner GetRunner(Game game)
        => new SequenceElementCancelRunner();
}

/// <summary>
/// A runner for handling <see cref="SequenceElementCancel"/> elements.
/// </summary>
public class SequenceElementCancelRunner() : ISequenceElementRunner {
    public SequenceRunnerState IsDone() => SequenceRunnerState.CANCEL;

    public void Cancel() { }
}