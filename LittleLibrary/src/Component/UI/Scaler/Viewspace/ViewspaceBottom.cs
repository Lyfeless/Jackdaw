using Foster.Framework;

namespace LittleLib;

public class ViewspaceBottomScalerElement(ScalerComponent component) : ScalerElement(component) {
    public static ScalerElement FixedOffset(ScalerComponent component, float offset) => new OffsetScalerElement(
        component,
        new ViewspaceBottomScalerElement(component),
        new FixedScalerElement(component, offset)
    );

    public static ScalerElement Percent(ScalerComponent component, float percent) => new PercentScalerElement(
        component,
        new ViewspaceBottomScalerElement(component),
        percent
    );

    public override float Get() => Component.Game.Viewspace.Bounds.Bottom;
}