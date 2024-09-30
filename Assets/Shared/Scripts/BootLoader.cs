using UnityEngine;

namespace HyperCasual.Gameplay
{
    /// <summary>
    ///     Instantiates and initializes a SequenceManager on Start
    /// </summary>
    public class BootLoader : MonoBehaviour
    {
        [SerializeField] private SequenceManager m_SequenceManagerPrefab;

        private void Start()
        {
            Instantiate(m_SequenceManagerPrefab);
            SequenceManager.Instance.Initialize();
        }
    }
}