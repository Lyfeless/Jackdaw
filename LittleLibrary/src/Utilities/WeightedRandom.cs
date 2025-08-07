namespace LittleLib;

public class WeightedRandom<T>(LittleGame game) {
    record struct Entry(T Value, int weight, int weightTotal);

    readonly LittleGame Game = game;
    readonly List<Entry> values = [];
    int weightTotal;

    public WeightedRandom<T> Add(T value, int weight) {
        weightTotal += weight;
        values.Add(new(value, weight, weightTotal));
        return this;
    }

    public T Get() {
        if (values.Count == 0) {
            Console.WriteLine("Attempting to get a weight random with no values, returning default");
            return default;
        }

        int value = Game.Random.Int(weightTotal);
        for (int i = 0; i < values.Count; ++i) {
            if (values[i].weightTotal >= value) {
                return values[i].Value;
            }
        }

        return default;
    }
}