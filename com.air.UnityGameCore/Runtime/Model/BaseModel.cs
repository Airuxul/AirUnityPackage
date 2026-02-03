namespace Air.UnityGameCore.Runtime.Model
{
    public abstract class BaseModel
    {
        public void Update(BaseModel model)
        {
            if (GetType().Name == model.GetType().Name)
            {
                
            }
        }
    }
}