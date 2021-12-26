using System;

namespace BubbleGameEvent
{
    public enum EventType
    {
        HideColor
    }

    public class EventListener
    {
        public static EventListener Instance
        {
            private set { }
            get
            {
                if (instance == null)
                    instance = new EventListener();

                return instance;
            }
        }
        private static EventListener instance;
        public Action<EventType> Observer;

        public void OnEventFire(EventType type)
        {
            Observer?.Invoke(type);
        }
    }
}