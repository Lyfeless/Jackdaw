namespace LittleLib;

/// <summary>
/// A small claimer utility to track if an object is already being used by a different component.
/// </summary>
public class ComponentClaimer() {
    Component? tickClaim = null;
    Component? storeClaim = null;

    /// <summary>
    /// If the component is claimed by any component in any form.
    /// </summary>
    public bool Claimed => tickClaim != null || storeClaim != null;

    /// <summary>
    /// Check if a component is holding the claim.
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    public bool ClaimedBy(Component component) => storeClaim == component || tickClaim == component;

    /// <summary>
    /// Attempt to claim indefinitely. Fails if a different component already holds the claim.
    /// </summary>
    /// <param name="component">The component trying to claim.</param>
    /// <returns>If the component successfully got the claim.</returns>
    public bool TryClaim(Component component) {
        if (!Claimed || ClaimedBy(component)) {
            storeClaim = component;
            return true;
        }

        return false;
    }


    /// <summary>
    /// Attempt to unclaim. Fails if a different component is holding the claim.
    /// </summary>
    /// <param name="component">The component trying to unclaim.</param>
    /// <returns>If the component successfully unclaimed.</returns>
    public bool TryUnclaim(Component component) {
        if (storeClaim == component) {
            storeClaim = null;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempt to claim for a single tick, unclaims at the next update.
    /// </summary>
    /// <param name="component">The component trying to claim.</param>
    /// <returns>If the component successfully got the claim.</returns>
    public bool TryClaimForTick(Component component) {
        if (!Claimed || ClaimedBy(component)) {
            tickClaim = component;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Update claim state.
    /// Needs to be run once per tick for tick claiming to work as expected
    /// </summary>
    public void Update() {
        tickClaim = null;
    }
}