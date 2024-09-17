using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     Ends the game on collision, forcing a win state.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Collider))]
    public class FinishLine : Spawnable
    {
        private const string k_PlayerTag = "Player";

        private void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag(k_PlayerTag)) GameManager.Instance.Win();
        }
    }
}