namespace LittleLib;

public class ViewspaceRightScalerElement(ScalerComponent component) : ScalerElement(component) {
    public static ScalerElement FixedOffset(ScalerComponent component, float offset) => new OffsetScalerElement(
        component,
        new ViewspaceRightScalerElement(component),
        new FixedScalerElement(component, offset)
    );

    public static ScalerElement Percent(ScalerComponent component, float percent) => new PercentScalerElement(
        component,
        new ViewspaceRightScalerElement(component),
        percent
    );

    public override float Get() => Component.Game.Viewspace.Bounds.Right;
}