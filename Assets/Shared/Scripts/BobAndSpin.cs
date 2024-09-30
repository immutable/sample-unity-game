using UnityEngine;

namespace HyperCasual.Core
{
    public class BobAndSpin : MonoBehaviour
    {
        public bool UsePositionBasedOffset = true;
        public float PositionBasedScale = 2.0f;

        public bool Bob = true;
        public float BobSpeed = 5.0f;
        public float BobHeight = 0.2f;

        public bool Spin = true;
        public float SpinSpeed = 180.0f;
        private Vector3 m_StartPosition;
        private Quaternion m_StartRotation;

        private Transform m_Transform;

        private void Awake()
        {
            m_Transform = transform;
            m_StartPosition = m_Transform.position;
            m_StartRotation = m_Transform.rotation;
        }

        private void Update()
        {
            var offset = UsePositionBasedOffset ? m_StartPosition.z * PositionBasedScale + Time.time : Time.time;

            if (Bob) m_Transform.position = m_StartPosition + Vector3.up * Mathf.Sin(offset * BobSpeed) * BobHeight;

            if (Spin) m_Transform.rotation = m_StartRotation * Quaternion.AngleAxis(offset * SpinSpeed, Vector3.up);
        }
    }
}