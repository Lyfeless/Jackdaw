namespace Jackdaw;

public interface ISequenceElement {
    public ISequenceElementRunner GetRunner(Game game);
}