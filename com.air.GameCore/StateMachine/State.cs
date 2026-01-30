namespace Air.GameCore.StateMachine
{
    public abstract class State
    {
        protected StateMachine Machine;
        protected State ActiveState => this;

        protected State(StateMachine machine)
        {
            Machine = machine;
        }

        internal virtual void Enter()
        {
            OnEnter();
        }

        internal virtual void Exit()
        {
            OnExit();
        }

        internal virtual void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        protected virtual void OnEnter()
        {
            throw new System.NotImplementedException();
        }

        protected virtual void OnUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        protected virtual void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }
}