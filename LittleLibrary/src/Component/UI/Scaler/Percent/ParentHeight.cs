namespace LittleLib;

public class PercentParentHeightScalerElement(ScalerComponent component, float percent) : ScalerElement(component) {
    readonly float Percent = percent;
    public override float Get() => Component.ParentBounds.Rect.Height * Percent;
}