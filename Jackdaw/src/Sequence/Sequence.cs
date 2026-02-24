namespace Jackdaw;

/// <summary>
/// A container for playing back actions with controllable time delays.
/// Supports fixed and dynamic waiting actions. <br/>
/// Playback a sequence using a <see cref="SequenceComponent"/> instance.
/// </summary>
public class Sequence() {
    readonly List<ISequenceElement> Elements = [];

    /// <summary>
    /// Gets element at the given index in the sequence. <br/>
    /// Does no bounds checking, make sure element isn't out of range with <see cref="SequenceDone"/> before accessing.
    /// </summary>
    /// <param name="i">The index to get the element at.</param>
    /// <returns>The element at the given index.</returns>
    public ISequenceElement this[int i] => Elements[i];

    /// <summary>
    /// If the sequence contains an element at the given index. If the element doesn't exist, the sequence is done.
    /// </summary>
    /// <param name="index">The index to check.</param>
    /// <returns>If the sequence is finished at the given index.</returns>
    public bool SequenceDone(int index) => index >= Elements.Count;

    /// <summary>
    /// Add a sequence element to the end of the sequence.
    /// </summary>
    /// <param name="element">The element to add to the end of the sequence.</param>
    /// <returns>The current sequence.</returns>
    public Sequence Element(ISequenceElement element) {
        Elements.Add(element);
        return this;
    }

    /// <summary>
    /// Add an action to the end of the sequence. Runs given action with no delay.
    /// </summary>
    /// <param name="action">The action to add.</param>
    /// <returns>The current sequence.</returns>
    public Sequence Action(Action action)
        => Element(new SequenceElementAction(action));

    /// <summary>
    /// Adds a fixed length delay to the end of the sequence.
    /// </summary>
    /// <param name="time">The time to wait.</param>
    /// <returns>The current sequence.</returns>
    public Sequence Wait(TimeSpan time)
        => Element(new SequenceElementDelay(time));

    /// <summary>
    /// Adds a dynamic delay to the end of the sequence. Pauses the sequence playback until the condition is met.
    /// </summary>
    /// <param name="condition">The condition to wait until.</param>
    /// <returns>The current sequence.</returns>
    public Sequence WaitUntil(Func<bool> condition)
        => Element(new SequenceElementConditionWait(condition));

    /// <summary>
    /// Adds a dynamic delay with a check interval to the end of the sequence.
    /// Pauses the sequence playback until the condition is met, only checking once every interval amount of time.
    /// </summary>
    /// <param name="condition">The condition to wait until.</param>
    /// <param name="interval">The amount of time to wait between checks.</param>
    /// <param name="startPercent">The percent through the interval to start at when the element starts.</param>
    /// <returns>The current sequence.</returns>
    public Sequence WaitUntil(Func<bool> condition, TimeSpan interval, float startPercent = 0)
        => Element(new SequenceElementConditionIntervalWait(condition, interval, startPercent));

    /// <summary>
    /// Adds an action to cancel the sequence and all parent sequences.
    /// </summary>
    /// <returns>The current sequence.</returns>
    public Sequence Cancel()
        => Element(new SequenceElementCancel());

    /// <summary>
    /// Add subsequences to the end of the sequence.
    /// Pauses the sequence playback until the given number of subsequences are finished.
    /// </summary>
    /// <param name="requiredFinishedSequences">The number of sequences that need to be completed before continuing.</param>
    /// <param name="sequences">The subsequences to add.</param>
    /// <returns>The current sequence.</returns>
    public Sequence SubSequences(int requiredFinishedSequences, params Sequence[] sequences) {
        if (requiredFinishedSequences < 0 || requiredFinishedSequences >= sequences.Length) {
            requiredFinishedSequences = sequences.Length;
        }
        Element(new SequenceElementSubSequences(requiredFinishedSequences, sequences));
        return this;
    }

    /// <summary>
    /// Add subsequences to the end of the sequence.
    /// Pauses the sequence playback until all of the subsequences are finished.
    /// </summary>
    /// <param name="sequences">The subsequences to add.</param>
    /// <returns>The current sequence.</returns>
    public Sequence SubSequencesAll(params Sequence[] sequences)
        => SubSequences(-1, sequences);

    /// <summary>
    /// Add subsequences to the end of the sequence.
    /// Pauses the sequence playback until all the first subsequence is finished.
    /// </summary>
    /// <param name="sequences">The subsequences to add.</param>
    /// <returns>The current sequence.</returns>
    public Sequence SubSequencesFirst(params Sequence[] sequences)
        => SubSequences(1, sequences);
}