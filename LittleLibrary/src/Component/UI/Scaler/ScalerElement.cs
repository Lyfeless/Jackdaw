namespace LittleLib;

public abstract class ScalerElement(ScalerComponent component) {
    protected ScalerComponent Component = component;

    public abstract float Get();
}