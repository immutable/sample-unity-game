using System;
using System.Collections;
using UnityEngine;

namespace HyperCasual.Core
{
    /// <summary>
    /// A generic state with data
    /// </summary>
    public class DataState<T> : AbstractState
    {
        readonly Action<T> m_OnExecute;
        public T m_Data { get; private set; }

        /// <param name="onExecute">An event that is invoked when the state is executed</param>
        public DataState(Action<T> onExecute)
        {
            m_OnExecute = onExecute;
        }

        public void SetData(T data)
        {
            m_Data = data;
        }

        public override IEnumerator Execute()
        {
            yield return null;
            m_OnExecute?.Invoke(m_Data);
        }
    }
}