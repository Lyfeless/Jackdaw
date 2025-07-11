using Foster.Framework;

namespace LittleLib;

public abstract class Component {
    public LittleGame Game;
    public Actor Actor = Actor.Invalid;
    public bool ActorValid => Actor.IsValid;
    public bool InTree => Actor.InTree;

    public ObjectIdentifier<Component> Match;

    public bool Ticking = true;
    public bool Visible = true;

    public bool addedToActor = false;
    public bool AddedToActor {
        get => addedToActor;
        set { if (value) { addedToActor = true; } }
    }

    public bool addedToTree = false;
    public bool AddedToTree {
        get => addedToTree;
        set { if (value) { addedToTree = true; } }
    }

    public Component(LittleGame game) {
        Game = game;
        Match = new(this);
    }

    public virtual void Update() { }
    public virtual void Render(Batcher batcher) { }

    public virtual void AddedFirst() { }
    public virtual void Added() { }
    public virtual void Removed() { }

    public virtual void EnterTreeFirst() { }
    public virtual void EnterTree() { }
    public virtual void ExitTree() { }
}