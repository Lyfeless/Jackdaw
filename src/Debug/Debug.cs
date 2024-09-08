using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public static class Debug {
    public static SpriteFont Font { get; private set; }
    public static void SetFont(string font) => Font = Assets.GetFont(font);

    public static bool DrawHitboxes = false;
    static readonly Color HitboxColor = Color.Red;
    static readonly Color HitboxActiveColor = Color.Green;
    public static bool DrawHitboxContainer = false;
    static readonly Color HitboxContainerColor = Color.Blue;
    public static bool DrawRenderbox = false;
    static readonly Color RenderboxColor = Color.Yellow;

    static readonly Dictionary<EntityReference, List<DebugEntry>> entityEntries = [];
    static readonly List<DebugEntry> globalEntries = [];

    public static void Init() {
        SetFont("error");
    }

    public static void AddValueToGlobal(string valueID, string label, Func<string> value) {
        globalEntries.Add(new(valueID, label, value));
    }

    public static void RemoveValueFromGlobal(string valueID) {
        globalEntries.RemoveAll(e => e.ID == valueID);
    }

    public static void ClearGlobalEntries() {
        globalEntries.Clear();
    }

    public static void AddValueToEntity(EntityReference reference, string valueID, string label, Func<string> value) {
        if (!entityEntries.TryGetValue(reference, out List<DebugEntry>? entry)) {
            entry = ([]);
            entityEntries.Add(reference, entry);
        }

        entry.Add(new(valueID, label, value));
    }

    public static void RemoveValueFromEntity(EntityReference reference, string valueID) {
        if (!entityEntries.TryGetValue(reference, out List<DebugEntry>? value)) { return; }
        value.RemoveAll(e => e.ID == valueID);
        if (value.Count == 0) { ClearEntityEntries(reference); }
    }

    public static void ClearEntityEntries(EntityReference reference) {
        entityEntries.Remove(reference);
    }

    public static void Render(Batcher batcher) {
        batcher.Clear();

        //! FIXME (Alex): Should probably be a util function in camera so this code doesnt need to be repeated
        batcher.PushMatrix(-Camera.ActiveCamera.Bounds.Position);
        foreach (EntityReference entity in EntityManager.Entities) {
            if (entity.Entity == null) { continue; }

            batcher.PushMatrix(entity.Entity.Position.Rounded);

            if (entityEntries.TryGetValue(entity, out List<DebugEntry>? entries)) {
                Vector2 topLeft = entity.Entity.Hitboxes.Bounds.TopLeft;
                for (int i = 0; i < entries.Count; ++i) {
                    DrawText(batcher, entries[i].GetText(), topLeft - (Vector2.UnitY * (i + 1) * Font.Height));
                }
            }


            if (DrawRenderbox) {
                batcher.RectLine(new(entity.Entity.Renderbox.TopLeft, entity.Entity.Renderbox.BottomRight), 1, RenderboxColor);
            }

            if (DrawHitboxContainer) {
                Rect hitboxBounds = entity.Entity.Hitboxes.Bounds;
                batcher.RectLine(new(hitboxBounds.TopLeft, hitboxBounds.BottomRight), 1, HitboxContainerColor);
            }

            if (DrawHitboxes) {
                foreach (Hitbox hitbox in entity.Entity.Hitboxes.Hitboxes) {
                    batcher.RectLine(new(hitbox.Bounds.TopLeft, hitbox.Bounds.BottomRight), 1, entity.Entity.HitboxCollided(hitbox) ? HitboxActiveColor : HitboxColor);
                }
            }

            batcher.PopMatrix();
        }
        batcher.PopMatrix();

        for (int i = 0; i < globalEntries.Count; ++i) {
            DrawText(batcher, globalEntries[i].GetText(), Vector2.UnitY * i * Font.Height);
        }
    }

    static void DrawText(Batcher batcher, string text, Vector2 position) {
        batcher.Text(Font, text, position + Vector2.UnitX, Color.Black);
        batcher.Text(Font, text, position - Vector2.UnitX, Color.Black);
        batcher.Text(Font, text, position + Vector2.UnitY, Color.Black);
        batcher.Text(Font, text, position - Vector2.UnitY, Color.Black);
        batcher.Text(Font, text, position, Color.White);
    }
}

readonly struct DebugEntry(string id, string label, Func<string> value) {
    public readonly string ID = id;
    public readonly string Label = label;
    public readonly Func<string> Value = value;

    public string GetText() {
        return $"{Label}: {Value()}";
    }
}