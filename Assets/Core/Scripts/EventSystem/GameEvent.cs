using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;
using HyperCasual.Runner;

namespace HyperCasual.Core
{
    /// <summary>
    /// Generic game event
    /// </summary>
    public class GameEvent<T> : AbstractGameEvent
    {
        public T m_Data { get; private set; }

        public void Raise(T data)
        {
            m_Data = data;
            base.Raise();
        }

        public override void Reset()
        {
        }
    }
}