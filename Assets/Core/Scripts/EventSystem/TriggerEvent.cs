using UnityEngine;

namespace HyperCasual.Core
{
    /// <summary>
    ///     Raises an event on trigger collision
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TriggerEvent : MonoBehaviour
    {
        private const string k_PlayerTag = "Player";

        [SerializeField] private AbstractGameEvent m_Event;

        private void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag(k_PlayerTag))
                if (m_Event != null)
                    m_Event.Raise();
        }
    }
}