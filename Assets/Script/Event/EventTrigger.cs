using UnityEngine;
using BubbleGameEvent;

public class EventTrigger : MonoBehaviour
{
    public void FireEvent(BubbleGameEvent.EventType type)
    {
        EventListener.Instance.OnEventFire(type);
    }
}
