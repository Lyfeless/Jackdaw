using System.Numerics;
using static Foster.Framework.Ease;

namespace LittleLib;

public class ValueTween<T>(T start, T end, float duration, Easer ease, string? tracker = null) where T : IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>, IMultiplyOperators<T, float, T> {
    public T Start = start;
    public T End = end;
    public Timer Timer = tracker == null ? new(duration) : new(duration, tracker);
    public Easer Ease = ease;

    public T Value => ((End - Start) * Ease((float)Timer.Percent)) + Start;
    public bool Done => Timer.Done;
}