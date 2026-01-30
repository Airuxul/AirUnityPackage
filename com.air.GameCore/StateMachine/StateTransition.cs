namespace Air.GameCore.StateMachine
{
    public interface IStateTransition
    {
        void ChangeState(State from, State to);
    }

    public class StateTransition : IStateTransition
    {
        public void ChangeState(State from, State to)
        {
            if (from == null) return;
            from.Exit();
            to.Enter();
        }
    }
}