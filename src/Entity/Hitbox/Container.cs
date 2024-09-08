using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): This could probably be turned into its own hitbox class to avoid tag redundancy
public class HitboxContainer {
    public List<Hitbox> Hitboxes = [];
    public void Add(Hitbox hitbox) {
        Hitboxes.Add(hitbox);
    }
    public void Remove(Hitbox hitbox) {
        Hitboxes.Remove(hitbox);
    }

    public long Tags {
        get {
            long tags = 0;
            foreach (Hitbox hitbox in Hitboxes) {
                tags |= hitbox.Tags;
            }
            return tags;
        }
    }

    public long CollisionTags {
        get {
            long tags = 0;
            foreach (Hitbox hitbox in Hitboxes) {
                tags |= hitbox.CollisionTags;
            }
            return tags;
        }
    }

    public Rect Bounds {
        get {
            if (Hitboxes.Count == 0) { return new Rect(0, 0); }
            if (Hitboxes.Count == 1) { return Hitboxes[0].Bounds; }

            BoundsBuilder bounds = new();
            foreach (Hitbox hitbox in Hitboxes) {
                bounds.Add(hitbox.Bounds);
            }

            return bounds.Rect;
        }
    }

    public bool HasTags(long tags) {
        if (tags == 0) { return false; }
        return (Tags & tags) == tags;
    }

    public bool HasAnyTag(long tags) {
        if (tags == 0) { return false; }
        return (Tags & tags) != 0;
    }
}