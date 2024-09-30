using System;
using System.Collections;

namespace HyperCasual.Core
{
    /// <summary>
    ///     A generic empty state
    /// </summary>
    public class State : AbstractState
    {
        private readonly Action m_OnExecute;

        /// <param name="onExecute">An event that is invoked when the state is executed</param>
        public State(Action onExecute)
        {
            m_OnExecute = onExecute;
        }

        public override IEnumerator Execute()
        {
            yield return null;
            m_OnExecute?.Invoke();
        }
    }
}