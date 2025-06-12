namespace LittleLib;

public class PercentParentWidthScalerElement(ScalerComponent component, float percent) : ScalerElement(component) {
    readonly float Percent = percent;
    public override float Get() => Component.ParentBounds.Rect.Width * Percent;
}