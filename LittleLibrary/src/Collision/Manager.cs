using System.Numerics;

namespace LittleLib;

public class CollisionManager {
    record struct ParsedCollider(CollisionDetectorComponent Component, Collider Collider);

    readonly List<CollisionDetectorComponent> Colliders = [];
    readonly List<CollisionResolverComponent> Resolvers = [];

    public void Add(CollisionResolverComponent resolver) {
        Resolvers.Add(resolver);
    }

    public void Remove(CollisionResolverComponent resolver) {
        Resolvers.Remove(resolver);
    }

    public void Add(CollisionDetectorComponent collider) {
        Colliders.Add(collider);
    }

    public void Remove(CollisionDetectorComponent collider) {
        Colliders.Remove(collider);
    }

    public void Update() {
        if (Colliders.Count < 2) { return; }

        ParsedCollider[] parsedColliders = [.. Colliders.Select(e => new ParsedCollider(e, e.Collider.Offset(e.Actor.GlobalPosition)))];

        //! FIXME (Alex): Store collisions from last tick to only clear ones that are needed
        foreach (CollisionDetectorComponent collider in Colliders) {
            collider.Collisions.Clear();
        }

        //! FIXME (Alex): Very temp, implement a spatial collision optimization system
        //! FIXME (Alex): Move this to a subfunction to control what entities are tested
        for (int a = 0; a < parsedColliders.Length - 1; ++a) {
            for (int b = a + 1; b < parsedColliders.Length; ++b) {
                ParsedCollider colliderA = parsedColliders[a];
                ParsedCollider colliderB = parsedColliders[b];

                if (
                    // First check collider bounds
                    colliderA.Collider.Bounds.Overlaps(colliderB.Collider.Bounds) &&
                    // Then do more precise check if there's a match
                    colliderA.Collider.Overlaps(colliderB.Collider, out Vector2 pushout)
                ) {
                    //! FIXME (Alex): Probably not how this'll be handled in the end
                    colliderA.Component.Collisions.Add(new(colliderB.Component, pushout));
                    colliderB.Component.Collisions.Add(new(colliderA.Component, -pushout));
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
    }
}