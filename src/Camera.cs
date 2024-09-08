using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class Camera(Vector2 startPos, UIVector2 anchor, Vector2 panAmount, Vector2 moveEase, Vector2 panEase, Vector2 deadzone) {

    public static Camera ActiveCamera = new(
        startPos: Vector2.Zero,
        anchor: UIVector2.Scaling(0, 0),
        panAmount: Vector2.Zero,
        moveEase: Vector2.One,
        panEase: Vector2.One,
        deadzone: Vector2.Zero);

    public static Point2 Viewport;

    //! FIXME (Alex): Possibly make this a renderposition, just so we have a rounded position to offset to
    public RenderablePosition Position = new(startPos);
    public UIVector2 Anchor = anchor;

    // readonly Aligner aligner = new(startPos, Viewport, anchor);

    public Rect PositionBounds = new(0, 0);

    public Vector2 CombinedPosition => Position.Rounded + Pan + Shake;
    public Vector2 CombinedPositionWithoutShake => Position.Rounded + Pan;

    public Vector2 TopLeft => CombinedPosition - Anchor.GetAppliedValue(Viewport);
    public Vector2 TopLeftWithoutShake => CombinedPositionWithoutShake - Anchor.GetAppliedValue(Viewport);

    public Rect Bounds {
        get {
            Vector2 topleft = TopLeft;
            return new(topleft, topleft + Viewport);
        }
    }
    public Rect BoundsWithoutShake {
        get {
            Vector2 topleft = TopLeftWithoutShake;
            return new(topleft, topleft + Viewport);
        }
    }

    public EntityReference? EntityTarget { get; private set; }
    public bool HasEntityTarget => EntityTarget != null && EntityTarget.Entity != null;
    public Vector2? PositionTarget { get; private set; }
    public bool HasPositionTarget => PositionTarget != null;
    public bool HasTarget => HasEntityTarget || HasPositionTarget;

    public Vector2 PanAmount = panAmount;
    public Vector2 MoveEase = moveEase;
    public Vector2 PanEase = panEase;
    public Vector2 ShakeStrength = Vector2.Zero;
    public Vector2 Deadzone = deadzone;

    public static VirtualStick? PanInput;
    bool lastPanController = false;
    Vector2 lastPanControllerPosition = Vector2.Zero;
    public Vector2 Pan { get; private set; } = Vector2.Zero;

    public Vector2 Shake => (ShakeTimer?.Done ?? true) ? Vector2.Zero : (new Vector2(Util.random.Float(1), Util.random.Float(1)) * ShakeStrength) - (ShakeStrength / 2);

    public Timer? ShakeTimer;

    public static Camera SetActiveCamera(Camera camera) {
        ActiveCamera = camera;
        return camera;
    }

    public void Update() {
        // Update Target Position
        Vector2 targetPos = Position.Precise;
        if (HasTarget) {
            if (HasEntityTarget) {
                targetPos = EntityTarget?.Entity?.GetTargetPosition() ?? Position.Precise;
            }
            else if (HasPositionTarget) {
                targetPos = PositionTarget ?? Position.Precise;
            }

            Vector2 diff = targetPos - Position.Precise;
            targetPos = new(
                (Math.Abs(diff.X) < Deadzone.X) ? Position.Precise.X : targetPos.X - (Math.Sign(diff.X) * Deadzone.X),
                (Math.Abs(diff.Y) < Deadzone.Y) ? Position.Precise.Y : targetPos.Y - (Math.Sign(diff.Y) * Deadzone.Y)
            );
        }

        // Update Pan
        if (PanAmount != Vector2.Zero) {
            Vector2 targetPan;
            if (
                (lastPanController && Input.Mouse.Position.Equals(Input.LastState.Mouse.Position)) ||
                (!lastPanController && !lastPanControllerPosition.Equals(PanInput?.Value))
            ) {

                // Set pan based on controller
                lastPanController = true;
                lastPanControllerPosition = PanInput?.Value ?? Vector2.Zero;

                targetPan = (PanInput?.Value ?? Vector2.Zero) * PanAmount;
            }
            else {
                // Set pan based on mouse position
                lastPanController = false;

                int maxDistance = Math.Min(Viewport.X, Viewport.Y) / 2;
                float distance = Math.Min(Vector2.Distance(GetCameraAdjustedMousePosition(), Viewport / 2), maxDistance) / maxDistance;
                float angle = (GetCameraAdjustedMousePosition() - Viewport / 2).Angle();
                targetPan = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * distance * PanAmount;
            }

            Pan += (targetPan - Pan) / PanEase;
        }

        // Constrain and Update Position
        targetPos = ConstrainPosition(targetPos);
        Position.Change((targetPos - Position.Precise) / MoveEase);
    }

    Vector2 ConstrainPosition(Vector2 position) {
        if (PositionBounds.Width >= Viewport.X && PositionBounds.Height >= Viewport.Y) {
            // Vector2 topLeft = TopLeftWithoutShake;
            Vector2 topLeft = position + Pan - Anchor.GetAppliedValue(Viewport);
            Vector2 adjustedTopLeft = Vector2.Clamp(topLeft, PositionBounds.TopLeft, PositionBounds.BottomRight - Viewport);
            Vector2 diff = topLeft - position;
            return adjustedTopLeft - diff;
        }

        return position;
    }

    public void ConstrainImmediately() {
        Position.Set(ConstrainPosition(Position.Precise));
    }

    public void SetTarget(EntityReference entity, bool moveToTarget = false) {
        PositionTarget = null;
        EntityTarget = entity;
        if (moveToTarget && entity.Entity != null) {
            Position.Set(entity.Entity.GetTargetPosition());
        }
    }

    public void SetTarget(Vector2 position, bool moveToTarget = false) {
        EntityTarget = null;
        PositionTarget = position;
        if (moveToTarget) {
            Position.Set(position);
        }
    }

    public void StartShake(float duration, Vector2 amount) {
        ShakeTimer = new(duration);
        ShakeStrength = amount;
    }

    public Vector2 GetCameraAdjustedMousePosition() {
        return ScreenScaler.GetScaledMousePosition() + CombinedPosition - (Viewport / 2);
    }
}