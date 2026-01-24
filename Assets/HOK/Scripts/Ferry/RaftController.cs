using Dreamteck.Splines;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HOK.Ferry
{
    /// <summary>
    /// Controls raft movement along a river spline. 
    /// Directly samples SplineComputer for position, maintaining level orientation.
    /// </summary>
    public class RaftController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float acceleration = 3f;
        [SerializeField] private float deceleration = 5f;

        [Header("References")]
        [SerializeField] private SplineComputer spline;

        [Header("SOAP Events (Optional)")]
        [SerializeField] private ScriptableEventNoParam onRaftStartedMoving;
        [SerializeField] private ScriptableEventNoParam onRaftStoppedMoving;

        private float currentSpeed;
        private float targetSpeed;
        private bool wasMoving;
        private Vector2 moveInput;
        private double currentPercent;

        private void Start()
        {
            if (spline != null)
            {
                // Initialize position on spline at start
                ApplySplinePosition();
            }
        }

        private void Update()
        {
            if (spline == null)
            {
                return;
            }

            UpdateSpeed();
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

        private void UpdateSpeed()
        {
            // X axis controls movement along spline (side-scrolling)
            // D/Right = positive input = move toward +Z (higher spline percent)
            // A/Left = negative input = move toward -Z (lower spline percent)
            targetSpeed = moveInput.x * maxSpeed;

            if (Mathf.Abs(targetSpeed) > 0.01f)
            {
                // Accelerating
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
            }
            else
            {
                // Decelerating
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
            }
        }

        private void ApplyMovement()
        {
            // Convert speed to percent change per second
            float splineLength = spline.CalculateLength();
            if (splineLength <= 0f)
            {
                return;
            }

            if (Mathf.Abs(currentSpeed) > 0.001f)
            {
                // Positive speed = increase percent = move toward higher Z (end of spline)
                // Negative speed = decrease percent = move toward lower Z (start of spline)
                float percentPerSecond = currentSpeed / splineLength;
                currentPercent += percentPerSecond * Time.deltaTime;

                // Clamp to valid range
                currentPercent = System.Math.Clamp(currentPercent, 0.0, 1.0);
            }

            ApplySplinePosition();
        }

        private void ApplySplinePosition()
        {
            // Sample the spline at current percent - position only
            SplineSample sample = spline.Evaluate(currentPercent);

            // Apply position only - side-scrolling game, raft stays fixed rotation
            transform.position = sample.position;
        }

        private void CheckMovementEvents()
        {
            bool isMoving = Mathf.Abs(currentSpeed) > 0.01f;

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
        /// Gets the current position along the spline (0-1).
        /// </summary>
        public double GetSplinePercent()
        {
            return currentPercent;
        }

        /// <summary>
        /// Sets the raft position along the spline (0-1).
        /// </summary>
        public void SetSplinePercent(double percent)
        {
            currentPercent = System.Math.Clamp(percent, 0.0, 1.0);
            if (spline != null)
            {
                ApplySplinePosition();
            }
        }

        /// <summary>
        /// Assigns a new spline for the raft to follow.
        /// </summary>
        public void SetSpline(SplineComputer newSpline)
        {
            spline = newSpline;
            if (spline != null)
            {
                ApplySplinePosition();
            }
        }

        /// <summary>
        /// Checks if the raft is currently moving.
        /// </summary>
        public bool IsMoving => Mathf.Abs(currentSpeed) > 0.01f;

        /// <summary>
        /// Gets the current movement speed.
        /// </summary>
        public float CurrentSpeed => currentSpeed;
    }
}
