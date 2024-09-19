using Foster.Framework;

namespace LittleLib;

public class UINineslice(Nineslice nineslice, UICreateArgs args) : UIElement(args) {
    public Nineslice Nineslice = nineslice;

    public override void Render(Batcher batcher) {
        Nineslice.Render(batcher, new RectInt((int)Size.X, (int)Size.Y));
    }
}