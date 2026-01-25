using Obvious.Soap;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HOK.Ferry
{
    /// <summary>
    /// Controls raft movement in free-navigation areas (Hub).
    /// Uses direct world-space movement on the X-Z plane instead of spline sampling.
    /// </summary>
    public class FreeMovementController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float acceleration = 3f;
        [SerializeField] private float deceleration = 5f;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 55f;

        [Header("Position Settings")]
        [Tooltip("Fixed Y position for the raft (water level).")]
        [SerializeField] private float waterLevel = 0.3f;

        [Header("SOAP Events (Optional)")]
        [SerializeField] private ScriptableEventNoParam onRaftStartedMoving;
        [SerializeField] private ScriptableEventNoParam onRaftStoppedMoving;

        private float currentSpeed;
        private bool wasMoving;
        private Vector2 moveInput;

        private void Start()
        {
            // Ensure raft starts at water level
            Vector3 pos = transform.position;
            pos.y = waterLevel;
            transform.position = pos;
        }

        private void Update()
        {
            UpdateVelocity();
            UpdateRotation();
            ApplyMovement();
            CheckMovementEvents();
        }

        /// <summary>
        /// Called by PlayerInput component via SendMessages or UnityEvents.
        /// </summary>
        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        /// <summary>
        /// Alternative method for direct input binding.
        /// </summary>
        public void SetMoveInput(Vector2 input)
        {
            moveInput = input;
        }

        private void UpdateVelocity()
        {
            // Movement is always forward relative to the raft's facing direction
            // moveInput.y controls forward/back speed
            float forwardSpeed = moveInput.y * maxSpeed;

            float accel = Mathf.Abs(moveInput.y) > 0.01f ? acceleration : deceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, forwardSpeed, accel * Time.deltaTime);
        }

        private void UpdateRotation()
        {
            // moveInput.x controls rotation (turning left/right)
            if (Mathf.Abs(moveInput.x) > 0.01f)
            {
                float rotationAmount = moveInput.x * rotationSpeed * Time.deltaTime;
                transform.Rotate(0f, rotationAmount, 0f);
            }
        }

        private void ApplyMovement()
        {
            if (Mathf.Abs(currentSpeed) < 0.0001f)
            {
                return;
            }

            // Move forward/back in the direction the raft is facing
            Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
            Vector3 pos = transform.position + movement;
            pos.y = waterLevel; // Keep at water level

            transform.position = pos;
        }

        private void CheckMovementEvents()
        {
            bool isMoving = Mathf.Abs(currentSpeed) > 0.01f || Mathf.Abs(moveInput.x) > 0.01f;

            if (isMoving && !wasMoving)
            {
                if (onRaftStartedMoving != null)
                {
                    onRaftStartedMoving.Raise();
                }
            }
            else if (!isMoving && wasMoving)
            {
                if (onRaftStoppedMoving != null)
                {
                    onRaftStoppedMoving.Raise();
                }
            }

            wasMoving = isMoving;
        }

        /// <summary>
        /// Sets the raft position directly.
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            position.y = waterLevel;
            transform.position = position;
        }

        /// <summary>
        /// Sets the raft position on the X-Z plane, keeping Y at water level.
        /// </summary>
        public void SetPosition(float x, float z)
        {
            transform.position = new Vector3(x, waterLevel, z);
        }

        /// <summary>
        /// Checks if the raft is currently moving or turning.
        /// </summary>
        public bool IsMoving => Mathf.Abs(currentSpeed) > 0.01f || Mathf.Abs(moveInput.x) > 0.01f;

        /// <summary>
        /// Gets the current forward/back speed.
        /// </summary>
        public float CurrentSpeed => currentSpeed;

        /// <summary>
        /// Gets the current velocity as a world-space vector.
        /// </summary>
        public Vector3 CurrentVelocity => transform.forward * currentSpeed;
    }
}
