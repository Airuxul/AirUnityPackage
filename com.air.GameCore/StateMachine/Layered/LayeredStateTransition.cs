using System.Collections.Generic;
using Air.GameCore.StateMachine;

namespace Air.GameCore.StateMachine.Layered
{
    public class LayeredStateTransition : IStateTransition
    {
        public void ChangeState(State f, State t)
        {
            if (f is not LayeredState from || t is not LayeredState to) return;
            var lcaState = Lca(from, to);
            for (var s = from; s != lcaState; s = s.Parent)
                s.Exit();
            var stack = new Stack<LayeredState>();
            for (var s = to; s != lcaState; s = s.Parent)
                stack.Push(s);
            while (stack.Count > 0)
                stack.Pop().Enter();
        }

        private static LayeredState Lca(LayeredState a, LayeredState b)
        {
            var aParent = new HashSet<LayeredState>();
            for (var s = a; s != null; s = s.Parent)
                aParent.Add(s);
            for (var s = b; s != null; s = s.Parent)
                if (aParent.Contains(s))
                    return s;
            return null;
        }
    }
}