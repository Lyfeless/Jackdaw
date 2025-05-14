using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class MultiCollider(params Collider[] colliders) : Collider {
    readonly Collider[] Colliders = colliders;

    public override Rect Bounds { get; } = new BoundsBuilder(colliders.Select(e => e.Bounds).ToArray()).Rect;

    public override bool Overlaps(Collider with, out Vector2 pushout) {
        //! FIXME (Alex): Does this make sense? Does this work? I haven't tested it
        pushout = Vector2.Zero;
        bool collide = false;
        foreach (Collider collider in Colliders) {
            if (collider.Overlaps(with, out Vector2 compare)) {
                if (compare.LengthSquared() > pushout.LengthSquared()) {
                    pushout = compare;
                    collide = true;
                }
            }
        }

        return collide;
    }
}