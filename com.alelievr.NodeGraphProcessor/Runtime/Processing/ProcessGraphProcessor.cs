using System.Collections.Generic;
using System.Linq;

namespace GraphProcessor
{
    public class ProcessGraphProcessor : BaseRuntimeGraphProcessor
    {
        private List<RuntimeBaseNode> processList;

        public override void InitRuntimeGraph(RuntimeGraph _graph)
        {
            base.InitRuntimeGraph(_graph);
            processList = _graph.Guid2Nodes.Values.ToList().OrderBy(a => a.Order).ToList();
        }

        public override void Run()
        {
            int count = processList.Count;
            for (int i = 0; i < count; i++)
            {
                processList[i].OnProcess();
            }
        }
    }
}