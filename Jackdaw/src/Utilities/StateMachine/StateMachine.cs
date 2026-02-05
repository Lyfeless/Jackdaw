namespace Jackdaw;

public class StateMachine<T>(T initialState) where T : Enum {
    readonly struct State(T value) {
        public readonly T Value = value;
        readonly List<IStateMachineElement> Elements = [];

        public readonly void Add(IStateMachineElement element) {
            Elements.Add(element);
        }

        public readonly void Set(bool value) {
            IStateMachineElement[] elementsTemp = [.. Elements];
            foreach (IStateMachineElement element in elementsTemp) {
                element.SetState(value);
            }
        }

        public readonly bool Has(IStateMachineElement element) {
            foreach (IStateMachineElement other in Elements) {
                if (element.Matches(other)) { return true; }
            }
            return false;
        }
    }

    readonly Dictionary<T, State> States = [];
    T CurrentState = initialState;

    #region Actor Ticking

    public StateMachine<T> AddTicking(T state, Actor actor)
        => Add(state, new StateMachineElementActorTicking(actor));

    public StateMachine<T> AddTicking(Actor actor, params T[] states)
        => Add(new StateMachineElementActorTicking(actor), states);

    public StateMachine<T> AddTicking(T state, params Actor[] actors) {
        foreach (Actor actor in actors) {
            AddTicking(state, actor);
        }
        return this;
    }

    #endregion

    #region Actor Visible

    public StateMachine<T> AddVisible(T state, Actor actor)
        => Add(state, new StateMachineElementActorVisible(actor));

    public StateMachine<T> AddVisible(Actor actor, params T[] states)
        => Add(new StateMachineElementActorVisible(actor), states);

    public StateMachine<T> AddVisible(T state, params Actor[] actors) {
        foreach (Actor actor in actors) {
            AddVisible(state, actor);
        }
        return this;
    }

    #endregion

    #region Component Ticking

    public StateMachine<T> AddTicking(T state, Component component)
        => Add(state, new StateMachineElementComponentTicking(component));

    public StateMachine<T> AddTicking(Component component, params T[] states)
        => Add(new StateMachineElementComponentTicking(component), states);

    public StateMachine<T> AddTicking(T state, params Component[] components) {
        foreach (Component component in components) {
            AddTicking(state, component);
        }
        return this;
    }

    #endregion

    #region Component Visible

    public StateMachine<T> AddVisible(T state, Component component)
        => Add(state, new StateMachineElementComponentVisible(component));

    public StateMachine<T> AddVisible(Component component, params T[] states)
        => Add(new StateMachineElementComponentVisible(component), states);

    public StateMachine<T> AddVisible(T state, params Component[] components) {
        foreach (Component component in components) {
            AddVisible(state, component);
        }
        return this;
    }

    #endregion

    public StateMachine<T> Add(IStateMachineElement element, params T[] states) {
        foreach (T state in states) {
            Add(state, element);
        }

        return this;
    }

    public StateMachine<T> Add(T state, params IStateMachineElement[] elements) {
        State stateValue = GetOrAddState(state);

        foreach (IStateMachineElement element in elements) {
            Add(stateValue, element);
        }

        return this;
    }

    public StateMachine<T> Add(T state, IStateMachineElement element) {
        Add(GetOrAddState(state), element);
        return this;
    }

    void Add(State state, IStateMachineElement element) {
        state.Add(element);

        if (Is(state)) { element.SetState(true); }
        else if (!IsElementActive(element)) { element.SetState(false); }
    }

    State GetOrAddState(T state) {
        if (!States.TryGetValue(state, out State value)) {
            value = new(state);
            States.Add(state, value);
        }
        return value;
    }

    bool IsElementActive(IStateMachineElement element)
        => States.TryGetValue(CurrentState, out State value) && value.Has(element);

    public void Set(T state) {
        if (Is(state)) { return; }
        SetCurrent(false);
        CurrentState = state;
        SetCurrent(true);
    }

    bool Is(State state) => Is(state.Value);
    public bool Is(T state) => state.Equals(CurrentState);

    void SetCurrent(bool value) => SetState(CurrentState, value);
    void SetState(T state, bool value) {
        if (!States.TryGetValue(state, out State stateValue)) { return; }
        stateValue.Set(value);
    }
}