namespace Jackdaw;

/// <summary>
/// A state machine that functions by changing the ticking/visibility of different elements,
/// primarily <see cref="Actor"/> or <see cref="Component"/> objects.
/// </summary>
/// <typeparam name="T">The enum designating the possible states.</typeparam>
/// <param name="initialState">The state to start in.</param>
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

    /// <summary>
    /// Set an actor to tick when the given state is active.
    /// </summary>
    /// <param name="state">The state that should tick that actor.</param>
    /// <param name="actor">The actor to tick.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> AddTicking(T state, Actor actor)
        => Add(state, new StateMachineElementActorTicking(actor));

    /// <summary>
    /// Set an actor to tick when any of the given states are active.
    /// </summary>
    /// <param name="actor">The actor to tick.</param>
    /// <param name="states">The states that should tick that actor.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> AddTicking(Actor actor, params T[] states)
        => Add(new StateMachineElementActorTicking(actor), states);

    /// <summary>
    /// Set multiple actors to tick when the given state is active.
    /// </summary>
    /// <param name="state">The state that should tick that actors.</param>
    /// <param name="actors">The actors to tick.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> AddTicking(T state, params Actor[] actors) {
        foreach (Actor actor in actors) {
            AddTicking(state, actor);
        }
        return this;
    }

    #endregion

    #region Actor Visible

    /// <summary>
    /// Set an actor to render when the given state is active.
    /// </summary>
    /// <param name="state">The state that should show that actor.</param>
    /// <param name="actor">The actor to render.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> AddVisible(T state, Actor actor)
        => Add(state, new StateMachineElementActorVisible(actor));

    /// <summary>
    /// Set an actor to render when any of the given states are active.
    /// </summary>
    /// <param name="actor">The actor to render.</param>
    /// <param name="states">The states that should show that actor.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> AddVisible(Actor actor, params T[] states)
        => Add(new StateMachineElementActorVisible(actor), states);

    /// <summary>
    /// Set multiple actors to render when the given state is active.
    /// </summary>
    /// <param name="state">The state that should show that actors.</param>
    /// <param name="actors">The actors to render.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> AddVisible(T state, params Actor[] actors) {
        foreach (Actor actor in actors) {
            AddVisible(state, actor);
        }
        return this;
    }

    #endregion

    #region Component Ticking

    /// <summary>
    /// Set a component to tick when the given state is active.
    /// </summary>
    /// <param name="state">The state that should tick that component.</param>
    /// <param name="component">The component to tick.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> AddTicking(T state, Component component)
        => Add(state, new StateMachineElementComponentTicking(component));

    /// <summary>
    /// Set a component to tick when any of the given states are active.
    /// </summary>
    /// <param name="component">The component to tick.</param>
    /// <param name="states">The states that should tick that component.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> AddTicking(Component component, params T[] states)
        => Add(new StateMachineElementComponentTicking(component), states);

    /// <summary>
    /// Set multiple components to tick when the given state is active.
    /// </summary>
    /// <param name="state">The state that should tick that components.</param>
    /// <param name="components">The components to tick.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> AddTicking(T state, params Component[] components) {
        foreach (Component component in components) {
            AddTicking(state, component);
        }
        return this;
    }

    #endregion

    #region Component Visible

    /// <summary>
    /// Set a component to render when the given state is active.
    /// </summary>
    /// <param name="state">The state that should render that component.</param>
    /// <param name="component">The component to render.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> AddVisible(T state, Component component)
        => Add(state, new StateMachineElementComponentVisible(component));

    /// <summary>
    /// Set a component to render when any of the given states are active.
    /// </summary>
    /// <param name="component">The component to render.</param>
    /// <param name="states">The states that should render that component.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> AddVisible(Component component, params T[] states)
        => Add(new StateMachineElementComponentVisible(component), states);

    /// <summary>
    /// Set multiple components to render when the given state is active.
    /// </summary>
    /// <param name="state">The state that should render that components.</param>
    /// <param name="components">The components to render.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> AddVisible(T state, params Component[] components) {
        foreach (Component component in components) {
            AddVisible(state, component);
        }
        return this;
    }

    #endregion

    /// <summary>
    /// Set a statemachine element to enable when the given states are active.
    /// </summary>
    /// <param name="element">The element to enable when the given states are active.</param>
    /// <param name="states">The states that should enable the element.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> Add(IStateMachineElement element, params T[] states) {
        foreach (T state in states) {
            Add(state, element);
        }

        return this;
    }

    /// <summary>
    /// Set multiple statemachine elements to enable when the given state is active.
    /// </summary>
    /// <param name="state">The state that should enable the elements.</param>
    /// <param name="elements">The elements to enable when the given state is active.</param>
    /// <returns>The statemachine instance.</returns>
    public StateMachine<T> Add(T state, params IStateMachineElement[] elements) {
        State stateValue = GetOrAddState(state);

        foreach (IStateMachineElement element in elements) {
            Add(stateValue, element);
        }

        return this;
    }

    /// <summary>
    /// Set a statemachine element to enable when the given state is active.
    /// </summary>
    /// <param name="state">The state that should enable the element.</param>
    /// <param name="element">The element to enable when the given state is active.</param>
    /// <returns>The statemachine instance.</returns>
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

    /// <summary>
    /// Set the current state of the statemachine.
    /// </summary>
    /// <param name="state">The state to set to current.</param>
    public void Set(T state) {
        if (Is(state)) { return; }
        SetCurrent(false);
        CurrentState = state;
        SetCurrent(true);
    }

    bool Is(State state) => Is(state.Value);

    /// <summary>
    /// Check if a state is the current state of the statemachine.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns>If the given state is the current state.</returns>
    public bool Is(T state) => state.Equals(CurrentState);

    void SetCurrent(bool value) => SetState(CurrentState, value);
    void SetState(T state, bool value) {
        if (!States.TryGetValue(state, out State stateValue)) { return; }
        stateValue.Set(value);
    }
}