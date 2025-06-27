namespace LittleLib;

public class ParentHeightScalerElement(ScalerComponent component) : ScalerElement(component) {
    public static ScalerElement FixedOffset(ScalerComponent component, float offset) => new OffsetScalerElement(
        component,
        new ParentHeightScalerElement(component),
        new FixedScalerElement(component, offset)
    );

    public static ScalerElement Percent(ScalerComponent component, float percent) => new PercentScalerElement(
        component,
        new ParentHeightScalerElement(component),
        percent
    );

    public override float Get() => Component.ParentBounds.Rect.Height;
}