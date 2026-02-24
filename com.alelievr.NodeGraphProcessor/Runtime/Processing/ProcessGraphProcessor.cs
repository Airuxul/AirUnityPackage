using System.Collections.Generic;
using System.Linq;

namespace GraphProcessor
{
    public class ProcessGraphProcessor : BaseRuntimeGraphProcessor
    {
        private List<RuntimeBaseNode> processList;
        
        public ProcessGraphProcessor(RuntimeGraph graph) : base(graph)
        {
            processList = graph.Guid2Nodes.Values.ToList().OrderBy(n => n.Order).ToList();
        }
        
        public override void Run()
        {
            int count = processList.Count;
            for (int i = 0; i < count; i++)
            {
                var node = processList[i];
                node.OnProcess();
            }
        }
    }
}