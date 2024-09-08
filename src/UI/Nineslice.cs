using Foster.Framework;

namespace LittleLib;

public class UINineslice(Nineslice nineslice, UICreateArgs args) : UIElement(args) {
    Nineslice Nineslice = nineslice;

    public override void Render(Batcher batcher) {
        Nineslice.Render(batcher, (RectInt)RelativeBounds);
    }
}