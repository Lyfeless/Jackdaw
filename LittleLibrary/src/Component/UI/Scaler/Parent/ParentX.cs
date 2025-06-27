namespace LittleLib;

public class ParentXScalerElement(ScalerComponent component) : ScalerElement(component) {
    public static ScalerElement FixedOffset(ScalerComponent component, float offset) => new OffsetScalerElement(
        component,
        new ParentXScalerElement(component),
        new FixedScalerElement(component, offset)
    );

    public static ScalerElement Percent(ScalerComponent component, float percent) => new PercentScalerElement(
        component,
        new ParentXScalerElement(component),
        percent
    );

    public override float Get() => Component.Actor.Parent.Position.Precise.X;
}