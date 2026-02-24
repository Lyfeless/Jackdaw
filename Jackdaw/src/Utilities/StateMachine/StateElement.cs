namespace Jackdaw;

/// <summary>
/// An element that can be enabled or disabled by a state machine.
/// </summary>
public interface IStateMachineElement {
    /// <summary>
    /// Set the enabled state of the element.
    /// </summary>
    /// <param name="value">The state to set the element to.</param>
    public void SetState(bool value);

    /// <summary>
    /// Check if the element is the same as another element.
    /// </summary>
    /// <param name="other">The element to compare to.</param>
    /// <returns>If the two elements match.</returns>
    public bool Matches(IStateMachineElement other);
}

/// <summary>
/// A state machine element for changing the ticking state of an actor.
/// </summary>
/// <param name="actor">The actor to tick.</param>
public readonly struct StateMachineElementActorTicking(Actor actor) : IStateMachineElement {
    public readonly Actor Actor = actor;
    public void SetState(bool value) => Actor.Ticking = value;
    public bool Matches(IStateMachineElement other) {
        if (other is not StateMachineElementActorTicking otherCast) { return false; }
        return otherCast.Actor == Actor;
    }
}

/// <summary>
/// A state machine element for changing the visibility state of an actor.
/// </summary>
/// <param name="actor">The actor to render.</param>
public readonly struct StateMachineElementActorVisible(Actor actor) : IStateMachineElement {
    public readonly Actor Actor = actor;
    public void SetState(bool value) => Actor.Visible = value;
    public bool Matches(IStateMachineElement other) {
        if (other is not StateMachineElementActorVisible otherCast) { return false; }
        return otherCast.Actor == Actor;
    }
}

/// <summary>
/// A state machine element for changing the ticking state of a component.
/// </summary>
/// <param name="component">The component to tick.</param>
public readonly struct StateMachineElementComponentTicking(Component component) : IStateMachineElement {
    public readonly Component Component = component;
    public void SetState(bool value) => Component.Ticking = value;
    public bool Matches(IStateMachineElement other) {
        if (other is not StateMachineElementComponentTicking otherCast) { return false; }
        return otherCast.Component == Component;
    }
}

/// <summary>
/// A state machine element for changing the visibility state of a component.
/// </summary>
/// <param name="component">The component to render.</param>
public readonly struct StateMachineElementComponentVisible(Component component) : IStateMachineElement {
    public readonly Component Component = component;
    public void SetState(bool value) => Component.Visible = value;
    public bool Matches(IStateMachineElement other) {
        if (other is not StateMachineElementComponentVisible otherCast) { return false; }
        return otherCast.Component == Component;
    }
}