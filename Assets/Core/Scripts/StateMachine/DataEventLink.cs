using UnityEngine;

namespace HyperCasual.Core
{
    public class DataEventLink<T> : ILink, IGameEventListener
    {
        private IState m_NextState;
        private GameEvent<T> m_GameEvent;
        private bool m_EventRaised;

        public DataEventLink(GameEvent<T> gameEvent, IState nextState)
        {
            m_GameEvent = gameEvent;
            m_NextState = nextState;
        }

        public bool Validate(out IState nextState)
        {
            nextState = null;
            bool result = false;

            if (m_EventRaised)
            {
                nextState = m_NextState;
                result = true;
            }

            return result;
        }

        public void OnEventRaised()
        {
            m_EventRaised = true;
            if (m_NextState is DataState<T> dataState)
            {
                dataState.SetData(m_GameEvent.m_Data);
            }
        }

        public void Enable()
        {
            m_GameEvent.AddListener(this);
            m_EventRaised = false;
        }

        public void Disable()
        {
            m_GameEvent.RemoveListener(this);
            m_EventRaised = false;
        }
    }

}