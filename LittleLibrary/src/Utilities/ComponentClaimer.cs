namespace LittleLib;

public class ComponentClaimer() {
    public bool Claimed => tickClaim != null || storeClaim != null;
    Component? tickClaim = null;
    Component? storeClaim = null;

    public bool TryClaim(Component component) {
        if (!Claimed) {
            storeClaim = component;
            return true;
        }

        return false;
    }

    public bool TryUnclaim(Component component) {
        if (storeClaim == component) {
            storeClaim = null;
            return true;
        }

        return false;
    }

    public bool TryClaimForTick(Component component) {
        if (!Claimed) {
            tickClaim = component;
            return true;
        }

        return false;
    }

    public void Update() {
        tickClaim = null;
    }
}