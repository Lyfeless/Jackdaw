using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A simple container class used fopr storing additional input-based utilities.
/// </summary>
/// <param name="input">The current game's input manager.</param>
public class Controls(Input input) {
    /// <summary>
    /// The current game's input manager.
    /// </summary>
    public Input Input = input;

    /// <summary>
    /// Input bindings.
    /// </summary>
    public Bindings Bindings = new();

    /// <summary>
    /// Mouse claim tracker, used for handling mouse usage between objects.
    /// </summary>
    public ComponentClaimer MouseClaim = new();

    internal void Update() {
        MouseClaim.Update();
    }
}