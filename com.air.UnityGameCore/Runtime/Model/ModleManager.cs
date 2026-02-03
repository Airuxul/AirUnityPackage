using System.Collections.Generic;

namespace Air.UnityGameCore.Runtime.Model
{
    public class ModleManager
    {
        private Dictionary<string, BaseModel> _modelDict = new();
        
        public void UpdateModel<T>(T model) where T : BaseModel
        {
            var typeName = model.GetType().Name;
            if (_modelDict.TryAdd(typeName, model))
            {
                
            }
            
        }
    }
}