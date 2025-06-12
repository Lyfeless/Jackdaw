using Foster.Framework;

namespace LittleLib;

public class Controls(Input input) {
    public Input Input = input;
    public Bindings Bindings = new();
    public ComponentClaimer MouseClaim = new();

    public void Update() {
        MouseClaim.Update();
    }
}