using System;

namespace StateMachine
{
    public class StateMachine
    {
        protected IStateTransition Transition;

        private State _currentState;

        private bool _isStarted;

        protected StateMachine(State currentState)
        {
            _currentState = currentState;
            Transition = new StateTransition();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void Start()
        {
            if (_currentState == null)
            {
                throw new Exception("StateMachine has no initial state set.");
            }
            if (_isStarted) return;
            _isStarted = true;
            _currentState?.Enter();
        }

        public void Tick(float deltaTime)
        {
            if(!_isStarted) Start();
            _currentState.Update(deltaTime);
        }

        public void ChangeState(State from, State to)
        {
            Transition.ChangeState(from, to);
            _currentState = to;
        }
    }
}