using System.Collections.Generic;
using Dreamteck.Splines;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HOK.Ferry
{
    /// <summary>
    /// Controls raft movement along a river spline.
    /// Directly samples SplineComputer for position, maintaining level orientation.
    /// Supports junction branching via up input.
    /// </summary>
    public class RaftController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float acceleration = 3f;
        [SerializeField] private float deceleration = 5f;

        [Header("References")]
        [SerializeField] private SplineComputer spline;

        [Tooltip("Starting position on spline (0-1). 0 = entrance (+X, right), 1 = dock (-X, left).")]
        [SerializeField] [Range(0f, 1f)] private float startingPercent = 0.1f;

        [Tooltip("Minimum percent the raft can reach (prevents clipping at spline start).")]
        [SerializeField] [Range(0f, 0.5f)] private float minPercent = 0.05f;

        [Tooltip("Maximum percent the raft can reach (prevents clipping at spline end).")]
        [SerializeField] [Range(0.5f, 1f)] private float maxPercent = 0.95f;

        [Header("Junction Settings")]
        [Tooltip("Junctions available on the current spline. Auto-populated if empty.")]
        [SerializeField] private List<SplineJunction> junctions = new List<SplineJunction>();

        [Header("SOAP Events (Optional)")]
        [SerializeField] private ScriptableEventNoParam onRaftStartedMoving;
        [SerializeField] private ScriptableEventNoParam onRaftStoppedMoving;
        [SerializeField] private ScriptableEventNoParam onJunctionAvailable;
        [SerializeField] private ScriptableEventNoParam onJunctionTaken;

        private float currentSpeed;
        private float targetSpeed;
        private bool wasMoving;
        private Vector2 moveInput;
        private double currentPercent;
        private SplineJunction activeJunction;
        private bool junctionInputPressed;
        private SplineJunction autoReturnJunction;
        private float junctionCooldown; // Prevents immediate auto-return after taking a junction

        private void Start()
        {
            if (spline != null)
            {
                // Initialize position on spline at starting percent
                currentPercent = startingPercent;
                ApplySplinePosition();
                RefreshJunctions();
            }
        }

        private void Update()
        {
            if (spline == null)
            {
                return;
            }

            // Decrement junction cooldown
            if (junctionCooldown > 0f)
            {
                junctionCooldown -= Time.deltaTime;
            }

            UpdateSpeed();
            ApplyMovement();
            CheckJunctions();
            CheckMovementEvents();
        }

        /// <summary>
        /// Called by PlayerInput component via SendMessages or UnityEvents.
        /// </summary>
        public void OnMove(InputValue value)
        {
            Vector2 input = value.Get<Vector2>();
            moveInput = input;

            // Check for junction input (up/W)
            // Trigger on press, not hold
            bool upPressed = input.y > 0.5f;
            if (upPressed && !junctionInputPressed)
            {
                TryTakeJunction();
            }
            junctionInputPressed = upPressed;
        }

        /// <summary>
        /// Alternative method for direct input binding.
        /// </summary>
        public void SetMoveInput(Vector2 input)
        {
            moveInput = input;

            bool upPressed = input.y > 0.5f;
            if (upPressed && !junctionInputPressed)
            {
                TryTakeJunction();
            }
            junctionInputPressed = upPressed;
        }

        private void UpdateSpeed()
        {
            // CORRECT Unity coordinates (camera at -Z looking +Z):
            // +X = screen RIGHT, -X = screen LEFT, +Z = screen TOP, -Z = screen BOTTOM
            // Spline: percent 0 = entrance (+X, right), percent 1 = dock (-X, left)
            // D/Right = positive input -> DECREASE percent = toward entrance (right, +X)
            // A/Left = negative input -> INCREASE percent = toward dock (left, -X)
            // Negate input so left arrow increases percent (toward dock)
            targetSpeed = -moveInput.x * maxSpeed;

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

                // Check for auto-return at spline boundaries (with cooldown to prevent immediate return after junction)
                if (currentPercent <= minPercent && autoReturnJunction != null && junctionCooldown <= 0f)
                {
                    // Hit start of spline with auto-return - take the junction
                    TakeJunction(autoReturnJunction, true);
                    return;
                }

                // Clamp to valid range (accounting for raft length)
                currentPercent = System.Math.Clamp(currentPercent, minPercent, maxPercent);
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

        private void CheckJunctions()
        {
            // When stationary, use 0 (any direction) for junction detection
            int travelDirection = currentSpeed > 0.01f ? 1 : (currentSpeed < -0.01f ? -1 : 0);
            SplineJunction newActiveJunction = null;

            // Find first available junction in range
            for (int i = 0; i < junctions.Count; i++)
            {
                SplineJunction junction = junctions[i];
                if (junction != null && junction.IsInRange(currentPercent, travelDirection))
                {
                    newActiveJunction = junction;
                    break;
                }
            }

            // Keep active junction if we're stationary and still in range (ignore direction when stopped)
            if (newActiveJunction == null && activeJunction != null && travelDirection == 0)
            {
                // Check if still in range without direction requirement
                float distance = Mathf.Abs((float)currentPercent - activeJunction.JunctionPercent);
                if (distance <= activeJunction.ActivationRange && activeJunction.TargetSpline != null)
                {
                    newActiveJunction = activeJunction;
                }
            }

            // Handle junction state changes
            if (newActiveJunction != activeJunction)
            {
                // Leaving old junction
                if (activeJunction != null)
                {
                    activeJunction.UpdateIndicator(false);
                }

                // Entering new junction
                if (newActiveJunction != null)
                {
                    newActiveJunction.UpdateIndicator(true);
                    if (onJunctionAvailable != null)
                    {
                        onJunctionAvailable.Raise();
                    }
                }

                activeJunction = newActiveJunction;
            }
        }

        private void TryTakeJunction()
        {
            if (activeJunction == null || activeJunction.TargetSpline == null)
            {
                return;
            }

            TakeJunction(activeJunction, false);
        }

        /// <summary>
        /// Takes a junction, switching to the target spline.
        /// Projects world position to find matching percent, but does NOT move the raft.
        /// The raft stays exactly where it is; only the spline reference changes.
        /// </summary>
        /// <param name="junction">The junction to take</param>
        /// <param name="isAutoReturn">If true, this is an auto-return at spline boundary</param>
        private void TakeJunction(SplineJunction junction, bool isAutoReturn)
        {
            if (junction == null || junction.TargetSpline == null)
            {
                return;
            }

            // Store current world position before switching
            Vector3 currentWorldPos = transform.position;

            // Clear current junction before switching
            if (activeJunction != null)
            {
                activeJunction.UpdateIndicator(false);
                activeJunction = null;
            }

            // Switch to the new spline
            SplineComputer targetSpline = junction.TargetSpline;
            spline = targetSpline;

            // Project current world position onto the new spline to find the closest percent
            // This determines where we are on the new spline's parameterization
            SplineSample projectedSample = new SplineSample();
            spline.Project(currentWorldPos, ref projectedSample);
            currentPercent = projectedSample.percent;

            // Clamp to valid range
            currentPercent = System.Math.Clamp(currentPercent, 0.0, 1.0);

            // DO NOT call ApplySplinePosition() here!
            // The raft stays exactly where it is - no teleport.
            // Normal movement in Update() will smoothly move the raft along the new spline.

            // Refresh junctions for new spline
            RefreshJunctions();

            // Set cooldown to prevent immediate auto-return (gives player time to move away from boundary)
            junctionCooldown = 0.5f;

            if (onJunctionTaken != null)
            {
                onJunctionTaken.Raise();
            }
        }

        /// <summary>
        /// Finds all SplineJunction components that are children of the current spline.
        /// </summary>
        private void RefreshJunctions()
        {
            junctions.Clear();
            autoReturnJunction = null;

            if (spline == null)
            {
                return;
            }

            // Find junctions on the spline GameObject and its children
            // We check if they're children of this spline rather than relying on SourceSpline property
            // because SourceSpline is set in Awake() which may not have run yet
            SplineJunction[] splineJunctions = spline.GetComponentsInChildren<SplineJunction>();
            for (int i = 0; i < splineJunctions.Length; i++)
            {
                SplineJunction junction = splineJunctions[i];
                junctions.Add(junction);

                // Track auto-return junction (should only be one per spline)
                if (junction.AutoReturnAtStart)
                {
                    autoReturnJunction = junction;
                }
            }
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
        /// Respects min/max percent limits.
        /// </summary>
        public void SetSplinePercent(double percent)
        {
            currentPercent = System.Math.Clamp(percent, minPercent, maxPercent);
            if (spline != null)
            {
                ApplySplinePosition();
            }
        }

        /// <summary>
        /// Assigns a new spline for the raft to follow.
        /// Refreshes available junctions automatically.
        /// </summary>
        public void SetSpline(SplineComputer newSpline)
        {
            spline = newSpline;
            if (spline != null)
            {
                ApplySplinePosition();
                RefreshJunctions();
            }
        }

        /// <summary>
        /// Assigns a new spline and sets the entry position.
        /// </summary>
        public void SetSpline(SplineComputer newSpline, double entryPercent)
        {
            spline = newSpline;
            currentPercent = System.Math.Clamp(entryPercent, 0.0, 1.0);
            if (spline != null)
            {
                ApplySplinePosition();
                RefreshJunctions();
            }
        }

        /// <summary>
        /// Gets the currently active junction (if any).
        /// </summary>
        public SplineJunction ActiveJunction => activeJunction;

        /// <summary>
        /// Gets whether a junction is currently available.
        /// </summary>
        public bool IsJunctionAvailable => activeJunction != null;

        /// <summary>
        /// Gets the current spline the raft is following.
        /// </summary>
        public SplineComputer CurrentSpline => spline;

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
