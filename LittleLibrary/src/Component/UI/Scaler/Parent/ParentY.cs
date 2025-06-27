namespace LittleLib;

public class ParentYScalerElement(ScalerComponent component) : ScalerElement(component) {
    public static ScalerElement FixedOffset(ScalerComponent component, float offset) => new OffsetScalerElement(
        component,
        new ParentYScalerElement(component),
        new FixedScalerElement(component, offset)
    );

    public static ScalerElement Percent(ScalerComponent component, float percent) => new PercentScalerElement(
        component,
        new ParentYScalerElement(component),
        percent
    );

    public override float Get() => Component.Actor.Parent.Position.Precise.Y;
}