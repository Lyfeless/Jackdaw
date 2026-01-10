namespace Jackdaw;

public interface ISequenceElementRunner {
    public SequenceRunnerState IsDone();
    public void Cancel();
}