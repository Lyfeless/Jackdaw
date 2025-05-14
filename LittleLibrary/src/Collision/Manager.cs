namespace LittleLib;

public class CollisionManager {
    readonly HashSet<CollisionDetectorComponent> Colliders = [];
    readonly HashSet<CollisionResolverComponent> Resolvers = [];

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
}