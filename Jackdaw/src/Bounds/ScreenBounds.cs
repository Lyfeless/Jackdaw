using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A bounds component that stores the current size of the main window. <br/>
/// Not visible by default.
/// </summary>
public class ScreenBoundsComponent : BoundsComponent {
    /// <param name="game">The current game instance.</param>
    public ScreenBoundsComponent(Game game) : base(game, game.Window.BoundsInPixels()) {
        Visible = false;
        Ticking = true;
    }

    protected override void Update() {
        Rect = Game.Window.BoundsInPixels();
    }
}