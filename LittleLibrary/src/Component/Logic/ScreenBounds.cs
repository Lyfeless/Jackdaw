using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class ScreenBounds(LittleGame game) : BoundsComponent(game, game.Window.BoundsInPixels()) {
    protected override void Update() {
        Rect = Game.Window.BoundsInPixels();
    }
}