namespace LittleLib;

public class ViewspaceTopScalerElement(ScalerComponent component) : ScalerElement(component) {
    public static ScalerElement FixedOffset(ScalerComponent component, float offset) => new OffsetScalerElement(
        component,
        new ViewspaceTopScalerElement(component),
        new FixedScalerElement(component, offset)
    );

    public static ScalerElement Percent(ScalerComponent component, float percent) => new PercentScalerElement(
        component,
        new ViewspaceTopScalerElement(component),
        percent
    );

    public override float Get() => Component.Game.Viewspace.Bounds.Top;
}