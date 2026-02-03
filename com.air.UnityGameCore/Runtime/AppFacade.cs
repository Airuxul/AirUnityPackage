using Air.GameCore;
using Air.UnityGameCore.Runtime.Event;
using Air.UnityGameCore.Runtime.Resource;
using Air.UnityGameCore.Runtime.UI;

public class AppFacade : Singleton<AppFacade>
{
    public readonly EventManager EventManager;
    public readonly IResManager ResManager;
    public readonly UIManager UIManager;
    
    public AppFacade()
    {
        EventManager = new EventManager();
        ResManager = new UnityResManager();
        UIManager = new UIManager(ResManager, EventManager);
    }
}
