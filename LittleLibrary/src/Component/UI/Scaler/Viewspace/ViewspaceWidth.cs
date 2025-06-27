namespace LittleLib;

public class ViewspaceWidthScalerElement(ScalerComponent component) : ScalerElement(component) {
    public static ScalerElement FixedOffset(ScalerComponent component, float offset) => new OffsetScalerElement(
        component,
        new ViewspaceWidthScalerElement(component),
        new FixedScalerElement(component, offset)
    );

    public static ScalerElement Percent(ScalerComponent component, float percent) => new PercentScalerElement(
        component,
        new ViewspaceWidthScalerElement(component),
        percent
    );

    public override float Get() => Component.Game.Viewspace.Size.X;
}