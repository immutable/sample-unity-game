using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     A class used to manage camera movement
    ///     in a Runner game.
    /// </summary>
    [ExecuteInEditMode]
    public class CameraManager : MonoBehaviour
    {
        private static readonly Vector3 k_CenteredScale = new(0.0f, 1.0f, 1.0f);

        [SerializeField] private CameraAnglePreset m_CameraAnglePreset = CameraAnglePreset.Behind;

        [SerializeField] private Vector3 m_Offset;

        [SerializeField] private Vector3 m_LookAtOffset;

        [SerializeField] private bool m_LockCameraPosition;

        [SerializeField] private bool m_SmoothCameraFollow;

        [SerializeField] private float m_SmoothCameraFollowStrength = 10.0f;

        private readonly bool[] m_PresetLockCameraPosition =
        {
            false, // Behind
            false, // Overhead
            true, // Side
            false, // FirstPerson
            false // Custom
        };

        private readonly Vector3[] m_PresetLookAtOffsets =
        {
            new(0.0f, 2.0f, 6.0f), // Behind
            new(0.0f, 0.0f, 4.0f), // Overhead
            new(-0.5f, 1.0f, 2.0f), // Side
            new Vector4(0.0f, 1.0f, 2.0f), // FirstPerson
            Vector3.zero // Custom
        };

        private readonly Vector3[] m_PresetOffsets =
        {
            new(0.0f, 5.0f, -9.0f), // Behind
            new(0.0f, 9.0f, -5.0f), // Overhead
            new(5.0f, 5.0f, -8.0f), // Side
            new(0.0f, 1.0f, 0.0f), // FirstPerson
            Vector3.zero // Custom
        };

        private Vector3 m_PrevLookAtOffset;

        private Transform m_Transform;

        /// <summary>
        ///     Returns the CameraManager.
        /// </summary>
        public static CameraManager Instance { get; private set; }

        private void Awake()
        {
            SetupInstance();
        }

        private void LateUpdate()
        {
            if (m_Transform == null) return;

            SetCameraPositionAndOrientation(m_SmoothCameraFollow);
        }

        private void OnEnable()
        {
            SetupInstance();
        }

        private void SetupInstance()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            m_Transform = transform;
        }

        /// <summary>
        ///     Reset the camera to its starting position relative
        ///     to the player.
        /// </summary>
        public void ResetCamera()
        {
            SetCameraPositionAndOrientation(false);
        }

        private Vector3 GetCameraOffset()
        {
            return m_PresetOffsets[(int)m_CameraAnglePreset] + m_Offset;
        }

        private Vector3 GetCameraLookAtOffset()
        {
            return m_PresetLookAtOffsets[(int)m_CameraAnglePreset] + m_LookAtOffset;
        }

        private bool GetCameraLockStatus()
        {
            if (m_LockCameraPosition) return true;

            return m_PresetLockCameraPosition[(int)m_CameraAnglePreset];
        }

        private Vector3 GetPlayerPosition()
        {
            var playerPosition = Vector3.up;
            if (PlayerController.Instance != null) playerPosition = PlayerController.Instance.GetPlayerTop();

            if (GetCameraLockStatus()) playerPosition = Vector3.Scale(playerPosition, k_CenteredScale);

            return playerPosition;
        }

        private void SetCameraPositionAndOrientation(bool smoothCameraFollow)
        {
            var playerPosition = GetPlayerPosition();

            var offset = playerPosition + GetCameraOffset();
            var lookAtOffset = playerPosition + GetCameraLookAtOffset();

            if (smoothCameraFollow)
            {
                var lerpAmound = Time.deltaTime * m_SmoothCameraFollowStrength;

                m_Transform.position = Vector3.Lerp(m_Transform.position, offset, lerpAmound);
                m_Transform.LookAt(Vector3.Lerp(m_Transform.position + m_Transform.forward, lookAtOffset, lerpAmound));

                m_Transform.position = new Vector3(m_Transform.position.x, m_Transform.position.y, offset.z);
            }
            else
            {
                m_Transform.position = playerPosition + GetCameraOffset();
                m_Transform.LookAt(lookAtOffset);
            }
        }

        private enum CameraAnglePreset
        {
            Behind,
            Overhead,
            Side,
            FirstPerson,
            Custom
        }
    }
}