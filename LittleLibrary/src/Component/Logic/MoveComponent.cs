using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class MoveComponent(LittleGame game, Vector2 speed) : Component(game) {
    Vector2 Speed = speed;

    public override void Update() {
        Vector2 change = new();
        if (Actor.Game.Input.Keyboard.Down(Keys.Up)) { change += new Vector2(0, -Speed.Y); }
        if (Actor.Game.Input.Keyboard.Down(Keys.Down)) { change += new Vector2(0, Speed.Y); }
        if (Actor.Game.Input.Keyboard.Down(Keys.Left)) { change += new Vector2(-Speed.X, 0); }
        if (Actor.Game.Input.Keyboard.Down(Keys.Right)) { change += new Vector2(Speed.X, 0); }
        if (change != Vector2.Zero) {
            Actor.Position.Change(change);
        }
    }
}