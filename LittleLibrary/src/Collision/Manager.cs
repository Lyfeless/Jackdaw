using System.Numerics;

namespace LittleLib;

public class CollisionManager {
    record struct ParsedCollider(CollisionComponent Component, Collider Collider);

    readonly List<CollisionComponent> Colliders = [];

    public void Add(CollisionComponent collider) {
        Colliders.Add(collider);
    }

    public void Remove(CollisionComponent collider) {
        Colliders.Remove(collider);
    }

    public void Update() {
        if (Colliders.Count < 2) { return; }

        ParsedCollider[] parsedColliders = [.. Colliders.Select(e => new ParsedCollider(e, e.Collider.Offset(e.Actor.GlobalPosition)))];

        //! FIXME (Alex): Store collisions from last tick to only clear ones that are needed
        foreach (CollisionComponent collider in Colliders) {
            collider.Collisions.Clear();
        }

        //! FIXME (Alex): Very temp, implement a spatial collision optimization system
        //! FIXME (Alex): Move this to a subfunction to control what entities are tested
        for (int a = 0; a < parsedColliders.Length - 1; ++a) {
            for (int b = a + 1; b < parsedColliders.Length; ++b) {
                ParsedCollider colliderA = parsedColliders[a];
                ParsedCollider colliderB = parsedColliders[b];

                bool tagMatchA = colliderA.Component.Mask.Any(colliderB.Component.Tags);
                bool tagMatchB = colliderB.Component.Mask.Any(colliderA.Component.Tags);

                if (
                    // Ensure either collider has matching tags
                    (tagMatchA || tagMatchB) &&
                    // Check collider bounds
                    colliderA.Collider.Bounds.Overlaps(colliderB.Collider.Bounds) &&
                    // Do more precise check if there's a match
                    colliderA.Collider.Overlaps(colliderB.Collider, out Vector2 pushout)
                ) {
                    //! FIXME (Alex): Likely not the data that will be stored on the entity in the end
                    if (tagMatchA) { colliderA.Component.Collisions.Add(new(colliderB.Component, pushout)); }
                    if (tagMatchB) { colliderB.Component.Collisions.Add(new(colliderA.Component, -pushout)); }
                }
            }
        }

        //! FIXME (Alex): Collision resolution
        /*
            Not as easy as it may initially seem, there's some edge-cases to worry about
            1. Should resolution only happen between collision resolvers? I think probably, a non-resolver
                components should just be treated as a trigger volume
            2. How should this be passed around?
                Possibly insteadof handling the resolver as a seperate component, it could be an optional
                member of the collision detector. when a collision occurs, store both locally if a
                resolver exists for both and handle all stored resolves afterwards?
        */
        //! FIXME (Alex): How should resolution between object with one-directional matching tags work?
        //      Does the ground need to have player tags? Or should every physics resolve be handled individually?
        //      Or should it only resolve collisions with a tag match in both directions
    }
}