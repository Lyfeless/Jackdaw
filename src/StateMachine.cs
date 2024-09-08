namespace LittleLib;

public class StateMachine<T>(T initialState) where T : Enum {
    public struct State(Action? update, Action? enter, Action? exit) {
        public Action? Update = update;
        public Action? Enter = enter;
        public Action? Exit = exit;
    }

    readonly Dictionary<T, State> states = [];
    public T currentState = initialState;
    public T lastState = initialState;

    public StateMachine<T> Add(T state, Action? update = null, Action? enter = null, Action? exit = null) {
        states.Add(state, new(update, enter, exit));
        return this;
    }

    public void SetState(T state) {
        if (state.ToString() == currentState.ToString()) { return; }

        lastState = currentState;
        currentState = state;

        states[lastState].Exit?.Invoke();
        states[currentState].Enter?.Invoke();
    }

    public void Update() {
        states[currentState].Update?.Invoke();
    }
}