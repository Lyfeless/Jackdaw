namespace Jackdaw;

public class Sequence() {
    readonly List<ISequenceElement> Elements = [];


    public ISequenceElement this[int i] => Elements[i];
    public bool SequenceDone(int index) => index >= Elements.Count;

    public Sequence Element(ISequenceElement element) {
        Elements.Add(element);
        return this;
    }

    public Sequence Action(Action action)
        => Element(new SequenceElementAction(action));

    public Sequence Delay(TimeSpan time)
        => Element(new SequenceElementDelay(time));

    public Sequence WaitUntil(Func<bool> condition)
        => Element(new SequenceElementConditionWait(condition));

    public Sequence WaitUntil(Func<bool> condition, TimeSpan interval, bool startFinished = false)
        => Element(new SequenceElementConditionIntervalWait(condition, interval, startFinished));

    public Sequence Cancel()
        => Element(new SequenceElementCancel());

    public Sequence SubSequences(int requiredFinishedSequences, params Sequence[] sequences) {
        if (requiredFinishedSequences < 0 || requiredFinishedSequences >= sequences.Length) {
            requiredFinishedSequences = sequences.Length;
        }
        Element(new SequenceElementSubSequences(requiredFinishedSequences, sequences));
        return this;
    }

    public Sequence SubSequencesAll(params Sequence[] sequences)
        => SubSequences(-1, sequences);

    public Sequence SubSequencesFirst(params Sequence[] sequences)
        => SubSequences(1, sequences);
}