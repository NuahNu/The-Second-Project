using System;
using UnityEngine;

public class AnimationEventsHandler : MonoBehaviour
{
    public event Action<string> OnAnimationEvent;

    public void AnimationEventHandler(string eventName)
    {
        OnAnimationEvent?.Invoke(eventName);
    }
}
