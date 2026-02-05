namespace Jackdaw;

public interface IStateMachineElement {
    public void SetState(bool value);
    public bool Matches(IStateMachineElement other);
}

public readonly struct StateMachineElementActorTicking(Actor actor) : IStateMachineElement {
    public readonly Actor Actor = actor;
    public void SetState(bool value) => Actor.Ticking = value;
    public bool Matches(IStateMachineElement other) {
        if (other is not StateMachineElementActorTicking otherCast) { return false; }
        return otherCast.Actor == Actor;
    }
}

public readonly struct StateMachineElementActorVisible(Actor actor) : IStateMachineElement {
    public readonly Actor Actor = actor;
    public void SetState(bool value) => Actor.Visible = value;
    public bool Matches(IStateMachineElement other) {
        if (other is not StateMachineElementActorVisible otherCast) { return false; }
        return otherCast.Actor == Actor;
    }
}

public readonly struct StateMachineElementComponentTicking(Component component) : IStateMachineElement {
    public readonly Component Component = component;
    public void SetState(bool value) => Component.Ticking = value;
    public bool Matches(IStateMachineElement other) {
        if (other is not StateMachineElementComponentTicking otherCast) { return false; }
        return otherCast.Component == Component;
    }
}

public readonly struct StateMachineElementComponentVisible(Component component) : IStateMachineElement {
    public readonly Component Component = component;
    public void SetState(bool value) => Component.Visible = value;
    public bool Matches(IStateMachineElement other) {
        if (other is not StateMachineElementComponentVisible otherCast) { return false; }
        return otherCast.Component == Component;
    }
}