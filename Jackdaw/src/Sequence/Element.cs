namespace Jackdaw;

/// <summary>
/// An interface for storing information related to a single sequence element and creating a runner on playback.
/// </summary>
public interface ISequenceElement {
    /// <summary>
    /// Get a runner responsible for executing the element in playback.
    /// </summary>
    /// <param name="game">The game instance to use for playback.</param>
    /// <returns>A runner instance for the sequence element.</returns>
    public ISequenceElementRunner GetRunner(Game game);
}