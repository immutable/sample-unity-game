using System.Collections;
using UnityEngine;

namespace HyperCasual.Core
{
    /// <summary>
    ///     Delays the state-machine for the set amount
    /// </summary>
    public class DelayState : AbstractState
    {
        private readonly float m_DelayInSeconds;

        /// <param name="delayInSeconds">delay in seconds</param>
        public DelayState(float delayInSeconds)
        {
            m_DelayInSeconds = delayInSeconds;
        }

        public override string Name => nameof(DelayState);

        public override IEnumerator Execute()
        {
            var startTime = Time.time;
            while (Time.time - startTime < m_DelayInSeconds) yield return null;
        }
    }
}