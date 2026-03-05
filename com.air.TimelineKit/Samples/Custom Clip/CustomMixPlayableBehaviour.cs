using TimelineKit;
using UnityEngine.Playables;

/// <summary>
/// Example track mixer behaviour.
/// Iterates active clip inputs and applies weighted blend logic.
/// Replace ProcessFrame with your own implementation.
/// </summary>
public class CustomMixPlayableBehaviour : PlayableBehaviourEx
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0f) continue;

            // Apply weighted blend logic here using playable.GetInput(i) and weight.
        }
    }
}
