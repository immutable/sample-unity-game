using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    ///     A class used to control a player in a Runner
    ///     game. Includes logic for player movement as well as
    ///     other gameplay logic.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private const float k_MinimumScale = 0.1f;

        private const float k_HalfWidth = 0.5f;
        private static readonly string s_Speed = "Speed";

        [SerializeField] private Animator m_Animator;

        [SerializeField] private SkinnedMeshRenderer m_SkinnedMeshRenderer;

        [SerializeField] private PlayerSpeedPreset m_PlayerSpeed = PlayerSpeedPreset.Medium;

        [SerializeField] private float m_CustomPlayerSpeed = 10.0f;

        [SerializeField] private float m_AccelerationSpeed = 10.0f;

        [SerializeField] private float m_DecelerationSpeed = 20.0f;

        [SerializeField] private float m_HorizontalSpeedFactor = 0.5f;

        [SerializeField] private float m_ScaleVelocity = 2.0f;

        [SerializeField] private bool m_AutoMoveForward = true;

        [SerializeField] private GameObject m_Fox;

        [SerializeField] private Texture m_foxOriginalTexture;
        [SerializeField] private Texture m_foxBlueTexture;
        [SerializeField] private Texture m_foxGradientTexture;
        private bool m_HasInput;

        private Vector3 m_LastPosition;
        private Vector3 m_Scale;
        private Vector3 m_StartPosition;

        private float m_XPos;
        private float m_ZPos;

        /// <summary> Returns the PlayerController. </summary>
        public static PlayerController Instance { get; private set; }

        /// <summary> The player's root Transform component. </summary>
        public Transform Transform { get; private set; }

        /// <summary> The player's current speed. </summary>
        public float Speed { get; private set; }

        /// <summary> The player's target speed. </summary>
        public float TargetSpeed { get; private set; }

        /// <summary> The player's minimum possible local scale. </summary>
        public float MinimumScale => k_MinimumScale;

        /// <summary> The player's current local scale. </summary>
        public Vector3 Scale => m_Scale;

        /// <summary> The player's target local scale. </summary>
        public Vector3 TargetScale { get; private set; }

        /// <summary> The player's default local scale. </summary>
        public Vector3 DefaultScale { get; private set; }

        /// <summary> The player's default local height. </summary>
        public float StartHeight { get; private set; }

        /// <summary> The player's default local height. </summary>
        public float TargetPosition { get; private set; }

        /// <summary> The player's maximum X position. </summary>
        public float MaxXPosition { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Initialize();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;

            // Update Scale

            if (!Approximately(Transform.localScale, TargetScale))
            {
                m_Scale = Vector3.Lerp(m_Scale, TargetScale, deltaTime * m_ScaleVelocity);
                Transform.localScale = m_Scale;
            }

            // Update Speed

            if (!m_AutoMoveForward && !m_HasInput)
                Decelerate(deltaTime, 0.0f);
            else if (TargetSpeed < Speed)
                Decelerate(deltaTime, TargetSpeed);
            else if (TargetSpeed > Speed) Accelerate(deltaTime, TargetSpeed);

            var speed = Speed * deltaTime;

            // Update position

            m_ZPos += speed;

            if (m_HasInput)
            {
                var horizontalSpeed = speed * m_HorizontalSpeedFactor;

                var newPositionTarget = Mathf.Lerp(m_XPos, TargetPosition, horizontalSpeed);
                var newPositionDifference = newPositionTarget - m_XPos;

                newPositionDifference = Mathf.Clamp(newPositionDifference, -horizontalSpeed, horizontalSpeed);

                m_XPos += newPositionDifference;
            }

            Transform.position = new Vector3(m_XPos, Transform.position.y, m_ZPos);

            if (m_Animator != null && deltaTime > 0.0f)
            {
                var distanceTravelledSinceLastFrame = (Transform.position - m_LastPosition).magnitude;
                var distancePerSecond = distanceTravelledSinceLastFrame / deltaTime;

                m_Animator.SetFloat(s_Speed, distancePerSecond);
            }

            if (Transform.position != m_LastPosition)
                Transform.forward = Vector3.Lerp(Transform.forward, (Transform.position - m_LastPosition).normalized,
                    speed);

            m_LastPosition = Transform.position;
        }

        /// <summary>
        ///     Set up all necessary values for the PlayerController.
        /// </summary>
        public void Initialize()
        {
            Transform = transform;
            m_StartPosition = Transform.position;
            DefaultScale = Transform.localScale;
            m_Scale = DefaultScale;
            TargetScale = m_Scale;

            if (m_SkinnedMeshRenderer != null)
                StartHeight = m_SkinnedMeshRenderer.bounds.size.y;
            else
                StartHeight = 1.0f;

            ResetSpeed();

            m_Fox.GetComponent<Renderer>().material.SetTexture("_BaseMap", SaveManager.Instance.UseNewSkin ? m_foxBlueTexture : m_foxOriginalTexture);
        }

        /// <summary>
        ///     Returns the current default speed based on the currently
        ///     selected PlayerSpeed preset.
        /// </summary>
        public float GetDefaultSpeed()
        {
            switch (m_PlayerSpeed)
            {
                case PlayerSpeedPreset.Slow:
                    return 5.0f;

                case PlayerSpeedPreset.Medium:
                    return 10.0f;

                case PlayerSpeedPreset.Fast:
                    return 20.0f;
            }

            return m_CustomPlayerSpeed;
        }

        /// <summary>
        ///     Adjust the player's current speed
        /// </summary>
        public void AdjustSpeed(float speed)
        {
            TargetSpeed += speed;
            TargetSpeed = Mathf.Max(0.0f, TargetSpeed);
        }

        /// <summary>
        ///     Reset the player's current speed to their default speed
        /// </summary>
        public void ResetSpeed()
        {
            Speed = 0.0f;
            TargetSpeed = GetDefaultSpeed();
        }

        /// <summary>
        ///     Adjust the player's current scale
        /// </summary>
        public void AdjustScale(float scale)
        {
            TargetScale += Vector3.one * scale;
            TargetScale = Vector3.Max(TargetScale, Vector3.one * k_MinimumScale);
        }

        /// <summary>
        ///     Reset the player's current speed to their default speed
        /// </summary>
        public void ResetScale()
        {
            m_Scale = DefaultScale;
            TargetScale = DefaultScale;
        }

        /// <summary>
        ///     Returns the player's transform component
        /// </summary>
        public Vector3 GetPlayerTop()
        {
            return Transform.position + Vector3.up * (StartHeight * m_Scale.y - StartHeight);
        }

        /// <summary>
        ///     Sets the target X position of the player
        /// </summary>
        public void SetDeltaPosition(float normalizedDeltaPosition)
        {
            if (MaxXPosition == 0.0f)
                Debug.LogError(
                    "Player cannot move because SetMaxXPosition has never been called or Level Width is 0. If you are in the LevelEditor scene, ensure a level has been loaded in the LevelEditor Window!");

            var fullWidth = MaxXPosition * 2.0f;
            TargetPosition = TargetPosition + fullWidth * normalizedDeltaPosition;
            TargetPosition = Mathf.Clamp(TargetPosition, -MaxXPosition, MaxXPosition);
            m_HasInput = true;
        }

        /// <summary>
        ///     Stops player movement
        /// </summary>
        public void CancelMovement()
        {
            m_HasInput = false;
        }

        /// <summary>
        ///     Set the level width to keep the player constrained
        /// </summary>
        public void SetMaxXPosition(float levelWidth)
        {
            // Level is centered at X = 0, so the maximum player
            // X position is half of the level width
            MaxXPosition = levelWidth * k_HalfWidth;
        }

        /// <summary>
        ///     Returns player to their starting position
        /// </summary>
        public void ResetPlayer()
        {
            Transform.position = m_StartPosition;
            m_XPos = 0.0f;
            m_ZPos = m_StartPosition.z;
            TargetPosition = 0.0f;

            m_LastPosition = Transform.position;

            m_HasInput = false;

            ResetSpeed();
            ResetScale();
        }

        private void Accelerate(float deltaTime, float targetSpeed)
        {
            Speed += deltaTime * m_AccelerationSpeed;
            Speed = Mathf.Min(Speed, targetSpeed);
        }

        private void Decelerate(float deltaTime, float targetSpeed)
        {
            Speed -= deltaTime * m_DecelerationSpeed;
            Speed = Mathf.Max(Speed, targetSpeed);
        }

        private bool Approximately(Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
        }

        private enum PlayerSpeedPreset
        {
            Slow,
            Medium,
            Fast,
            Custom
        }
    }
}