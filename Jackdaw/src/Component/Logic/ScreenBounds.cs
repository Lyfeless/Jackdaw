using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

public class ScreenBoundsComponent(Game game) : BoundsComponent(game, game.Window.BoundsInPixels()) {
    protected override void Update() {
        Rect = Game.Window.BoundsInPixels();
    }
}