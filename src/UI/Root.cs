using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class UIRoot() : UIElement(new() { ID = "root" }) {

    public override Vector2 Size { get => Camera.Viewport; }

    public override void Render(Batcher batcher) { }
}