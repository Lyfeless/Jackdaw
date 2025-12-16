namespace Jackdaw;

internal interface IElementInvalidation {
    public void Invalidate();
}

internal readonly struct ActorInvalidation(Actor actor, bool invalidateChildren, bool invalidateComponents) : IElementInvalidation {
    readonly Actor Actor = actor;
    readonly bool InvalidateChildren = invalidateChildren;
    readonly bool InvalidateComponents = invalidateComponents;

    public readonly void Invalidate() => Actor.Invalidate(InvalidateChildren, InvalidateComponents);
}

internal readonly struct ComponentInvalidation(Component component) : IElementInvalidation {
    readonly Component Component = component;

    public readonly void Invalidate() => Component.OnInvalidated();
}