namespace LittleLib;

public class ParentLeftScalerElement(ScalerComponent component) : ScalerElement(component) {
    public static ScalerElement FixedOffset(ScalerComponent component, float offset) => new OffsetScalerElement(
        component,
        new ParentLeftScalerElement(component),
        new FixedScalerElement(component, offset)
    );

    public static ScalerElement Percent(ScalerComponent component, float percent) => new PercentScalerElement(
        component,
        new ParentLeftScalerElement(component),
        percent
    );

    public override float Get() => Component.ParentBounds.Rect.Left;
}