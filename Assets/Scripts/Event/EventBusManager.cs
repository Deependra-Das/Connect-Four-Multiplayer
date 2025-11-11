using ConnectFourMultiplayer.Utilities;
using System.Collections.Generic;
using UnityEngine.Events;

namespace ConnectFourMultiplayer.Event
{
    public class EventBusManager : GenericMonoSingleton<EventBusManager>
    {
        private readonly Dictionary<string, UnityEvent<object[]>> _eventDictionary = new Dictionary<string, UnityEvent<object[]>>();

        public void Subscribe(EventNameEnum eventName, UnityAction<object[]> listener)
        {
            if (!_eventDictionary.TryGetValue(eventName.ToString(), out var thisEvent))
            {
                thisEvent = new UnityEvent<object[]>();
                _eventDictionary.Add(eventName.ToString(), thisEvent);
            }
            thisEvent.AddListener(listener);
        }

        public void Unsubscribe(EventNameEnum eventName, UnityAction<object[]> listener)
        {
            if (_eventDictionary.TryGetValue(eventName.ToString(), out var thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public void Raise(EventNameEnum eventName, params object[] parameters)
        {
            if (_eventDictionary.TryGetValue(eventName.ToString(), out var thisEvent))
            {
                if (parameters == null || parameters.Length == 0)
                {
                    thisEvent.Invoke(new object[0]);
                }
                else
                {
                    thisEvent.Invoke(parameters);
                }
            }
        }

        public void RaiseNoParams(EventNameEnum eventName)
        {
            if (_eventDictionary.TryGetValue(eventName.ToString(), out var thisEvent))
            {
                thisEvent.Invoke(new object[0]);
            }
        }
    }
}