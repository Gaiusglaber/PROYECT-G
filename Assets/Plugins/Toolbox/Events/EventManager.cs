using System;
using System.Collections.Generic;

using Pathfinders.Toolbox.Singletons;

namespace Pathfinders.Toolbox.Events
{
    public class EventManager : GenericSingleton<EventManager>
    {
        #region PRIVATE_FIELDS
        private Dictionary<string, Action<EventParam>> eventDictionary;
        #endregion

        #region UNITY_CALLS
        protected override void Awake()
        {
            base.Awake();
            Init();
        }
        #endregion

        #region INITIALIZATION
        void Init()
        {
            if (eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, Action<EventParam>>();
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public static void StartListening(string eventName, Action<EventParam> listener)
        {
            Action<EventParam> thisEvent;
            if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent += listener;
                Instance.eventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                Instance.eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening(string eventName, Action<EventParam> listener)
        {
            if (Instance == null) return;

            Action<EventParam> thisEvent;
            if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent -= listener;
                Instance.eventDictionary[eventName] = thisEvent;
            }
        }

        public static void TriggerEvent(string eventName, EventParam eventParam = null)
        {
            UnityEngine.Debug.Log(eventName);
            Action<EventParam> thisEvent = null;
            if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(eventParam);
            }
        }
        #endregion
    }

    public class EventParam
    {
        
    }
}