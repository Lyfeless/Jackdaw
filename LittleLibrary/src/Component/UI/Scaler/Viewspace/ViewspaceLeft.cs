namespace LittleLib;

public class ViewspaceLeftScalerElement(ScalerComponent component) : ScalerElement(component) {
    public static ScalerElement FixedOffset(ScalerComponent component, float offset) => new OffsetScalerElement(
        component,
        new ViewspaceLeftScalerElement(component),
        new FixedScalerElement(component, offset)
    );

    public static ScalerElement Percent(ScalerComponent component, float percent) => new PercentScalerElement(
        component,
        new ViewspaceLeftScalerElement(component),
        percent
    );

    public override float Get() => Component.Game.Viewspace.Bounds.Left;
}