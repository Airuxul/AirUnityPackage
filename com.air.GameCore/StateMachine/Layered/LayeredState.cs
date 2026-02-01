using System.Collections.Generic;

namespace StateMachine.Layered
{
    public abstract class LayeredState : State
    {
        public readonly LayeredState Parent;
        private LayeredState _activeChild;
        private new LayeredState ActiveState => _activeChild ?? this;
        
        protected abstract LayeredState GetInitialState();

        protected LayeredState(StateMachine machine, LayeredState parent) : base(machine)
        {
            Machine = machine;
            Parent = parent;
        }

        internal override void Enter()
        {
            if (Parent != null)
            {
                Parent._activeChild = this;
            }
            OnEnter();
            GetInitialState()?.Enter();
        }

        internal override void Exit()
        {
            ActiveState?.Exit();
            _activeChild = null;
            OnExit();
        }

        internal override void Update(float deltaTime)
        {
            ActiveState.Update(deltaTime);
            OnUpdate(deltaTime);
        }

        public LayeredState Leaf()
        {
            var s = this;
            while (s._activeChild != null) s = s._activeChild;
            return s;
        }

        public IEnumerable<LayeredState> Path2Root()
        {
            for (var s = this; s != null; s = s.Parent)
                yield return s;
        }
    }
}