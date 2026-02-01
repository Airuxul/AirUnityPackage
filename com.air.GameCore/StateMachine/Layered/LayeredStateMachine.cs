namespace StateMachine.Layered
{
    public abstract class LayeredStateMachine : StateMachine
    {
        protected LayeredStateMachine(State currentState) : base(currentState)
        {
            Transition = new LayeredStateTransition();
        }
        
        public new void ChangeState(State from, State to)
        {
            Transition.ChangeState(from, to);
        }
    }
}