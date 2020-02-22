
namespace StateMachine
{
    public class StateMachine<T>
    {
        public State<T> currentState { get; private set; }
        public T Owner;

        public StateMachine(T _Owner)
        {
            Owner = _Owner;
            currentState = null;
        }

        public void ChangeState(State<T> _newState)
        {
            if (currentState != null)
                currentState.ExitState(Owner);
            currentState = _newState;
            currentState.EnterState(Owner);
        }
        public void UpdateMachine()
        {
            if (currentState != null)
                currentState.UpdateState(Owner);
        }
    }

    public abstract class State<T>
    {
        public abstract void EnterState(T _owner);
        public abstract void ExitState(T _owner);
        public abstract void UpdateState(T _owner);
    }
}