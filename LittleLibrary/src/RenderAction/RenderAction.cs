using System.Numerics;

namespace LittleLib;

/// <summary>
/// An action designed to modify the rendering of all child and component elements on an actor.
/// </summary>
public abstract class ActorRenderAction() {
    /// <summary>
    /// Component identifier, used for searching render elements attached to an actor.
    /// </summary>
    public ObjectIdentifier<ActorRenderAction> Match;

    /// <summary>
    /// The display transform caused by applying the render action.
    /// </summary>
    public virtual Matrix3x2 PositionOffset { get; } = Matrix3x2.Identity;

    /// <summary>
    /// Apply render changes. Pre render actions are called in the order set by the actor's action container.
    /// </summary>
    /// <param name="container">The container the action is assigned to.</param>
    public virtual void PreRender(RenderActionContainer container) { }

    /// <summary>
    /// Clear all render changes. Post render actions are called in reverse order from pre render.
    /// </summary>
    /// <param name="container">The container the action is assigned to.</param>
    public virtual void PostRender(RenderActionContainer container) { }
}