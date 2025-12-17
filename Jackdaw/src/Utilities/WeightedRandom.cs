using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A small utility to get random values with weights.
/// </summary>
/// <typeparam name="T">The random return type.</typeparam>
/// <param name="game">The game instance.</param>
public class WeightedRandom<T>(Game game) {
    record struct Entry(T Value, int Weight, int WeightTotal);

    readonly Game Game = game;
    readonly List<Entry> values = [];
    int weightTotal;

    /// <summary>
    /// Add a value to the random pool.
    /// </summary>
    /// <param name="value">The value to return if selected.</param>
    /// <param name="weight">The randomized weight, with higher numbers being more likely to select.</param>
    /// <returns>The weighted randomizer.</returns>
    public WeightedRandom<T> Add(T value, int weight) {
        weightTotal += weight;
        values.Add(new(value, weight, weightTotal));
        return this;
    }

    /// <summary>
    /// Get a random value from the pool.
    /// </summary>
    /// <returns>The randomly selected value.</returns>
    public T Get() {
        if (values.Count == 0) {
            Log.Warning("WeightedRandom: Attempting to get a weight random with no values, returning default");
            return default!;
        }

        int value = Game.Random.Int(weightTotal);
        for (int i = 0; i < values.Count; ++i) {
            if (values[i].WeightTotal > value) {
                return values[i].Value;
            }
        }

        // Unreachable, just a fallback
        return default!;
    }
}