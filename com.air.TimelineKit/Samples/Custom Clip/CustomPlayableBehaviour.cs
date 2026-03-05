using TimelineKit;
using UnityEngine.Playables;

/// <summary>
/// Example clip behaviour. The base class PlayableBehaviourEx already routes all callbacks
/// to _editorBehaviour in edit mode, so you only need to override methods where you want
/// to add runtime logic. Remove overrides you don't need.
/// </summary>
public class CustomPlayableBehaviour : PlayableBehaviourEx
{
    private string _message;

    public string Message
    {
        set => _message = value;
    }

    public CustomPlayableBehaviour()
    {
#if UNITY_EDITOR
        _editorBehaviour = new CustomEditorBehaviour();
#endif
    }

    // Override only the methods where you have runtime logic to add.
    // Example:
    //
    // public override void OnBehaviourPlay(Playable playable, FrameData info)
    // {
    //     base.OnBehaviourPlay(playable, info); // handles editor routing
    //     // your runtime logic here
    // }
}
