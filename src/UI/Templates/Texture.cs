using Foster.Framework;

namespace LittleLib;

public class UITexture(Subtexture texture, UICreateArgs args, Color? color = null) : UIElement(args) {
    public Subtexture Texture { get; protected set; } = texture;
    public Color Color { get; protected set; } = color ?? Color.White;

    public UITexture(string texture, UICreateArgs args, Color? color = null) : this(Assets.GetTexture(texture), args, color) { }

    public override void Render(Batcher batcher) {
        batcher.Image(Texture, Color);
    }
}