namespace LittleLib;

public class ParentBottomScalerElement(ScalerComponent component) : ScalerElement(component) {
    public static ScalerElement FixedOffset(ScalerComponent component, float offset) => new OffsetScalerElement(
        component,
        new ParentBottomScalerElement(component),
        new FixedScalerElement(component, offset)
    );

    public static ScalerElement Percent(ScalerComponent component, float percent) => new PercentScalerElement(
        component,
        new ParentBottomScalerElement(component),
        percent
    );

    public override float Get() => Component.ParentBounds.Rect.Bottom;
}