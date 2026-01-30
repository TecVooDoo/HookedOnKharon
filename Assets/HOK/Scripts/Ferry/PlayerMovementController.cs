using System.Collections.Generic;
using Dreamteck.Splines;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HOK.Ferry
{
    /// <summary>
    /// Defines the movement mode for the raft.
    /// </summary>
    public enum MovementMode
    {
        /// <summary>Free movement on the X-Z plane (Hub scenes).</summary>
        Free,
        /// <summary>Movement constrained to a spline (River scenes).</summary>
        Spline
    }

    /// <summary>
    /// Unified movement controller for the raft that handles both free movement (Hub)
    /// and spline-based movement (Rivers). Automatically detects and switches modes
    /// based on scene type or can be set manually.
    /// </summary>
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("Movement Mode")]
        [Tooltip("Current movement mode. Set automatically on scene load or manually.")]
        [SerializeField] private MovementMode currentMode = MovementMode.Free;

        [Header("Free Movement Settings (Hub)")]
        [SerializeField] private float freeMaxSpeed = 5f;
        [SerializeField] private float freeAcceleration = 3f;
        [SerializeField] private float freeDeceleration = 5f;
        [SerializeField] private float rotationSpeed = 55f;
        [Tooltip("Fixed Y position for the raft (water level).")]
        [SerializeField] private float waterLevel = 0.3f;

        [Header("Spline Movement Settings (River)")]
        [SerializeField] private float splineMaxSpeed = 1f;
        [SerializeField] private float splineAcceleration = 1f;
        [SerializeField] private float splineDeceleration = 3f;

        [Header("Spline Settings")]
        [SerializeField] private SplineComputer spline;
        [Tooltip("Starting position on spline (0-1). 0 = entrance (+X, right). 1 = dock (-X, left).")]
        [SerializeField][Range(0f, 1f)] private double startingPercent = 0.0;
        [Tooltip("How quickly the raft rotates to align with the spline direction.")]
        [SerializeField] private float splineRotationSpeed = 5f;
        [Tooltip("If true, auto-detect spline in scene on Start and switch to Spline mode.")]
        [SerializeField] private bool autoDetectSplineOnStart = true;

        [Header("Junctions")]
        [Tooltip("Junctions available on the current spline. Auto-populated if empty.")]
        [SerializeField] private List<SplineJunction> junctions = new List<SplineJunction>();

        [Header("SOAP Events (Optional)")]
        [SerializeField] private ScriptableEventNoParam onRaftStartedMoving;
        [SerializeField] private ScriptableEventNoParam onRaftStoppedMoving;
        [SerializeField] private ScriptableEventNoParam onJunctionAvailable;
        [SerializeField] private ScriptableEventNoParam onJunctionTaken;

        // Shared state
        private float currentSpeed;
        private float targetSpeed;
        private bool wasMoving;
        private Vector2 moveInput;

        // Spline-specific state
        private double currentPercent;
        private SplineJunction activeJunction;
        private SplineJunction pendingManualJunction;
        private float junctionCooldown;

        #region Public Properties

        /// <summary>Current movement mode.</summary>
        public MovementMode CurrentMode => currentMode;

        /// <summary>Whether the raft is currently moving.</summary>
        public bool IsMoving => currentMode == MovementMode.Free
            ? Mathf.Abs(currentSpeed) > 0.01f || Mathf.Abs(moveInput.x) > 0.01f
            : Mathf.Abs(currentSpeed) > 0.01f;

        /// <summary>Current forward/back speed.</summary>
        public float CurrentSpeed => currentSpeed;

        /// <summary>Current velocity as a world-space vector (Free mode only).</summary>
        public Vector3 CurrentVelocity => transform.forward * currentSpeed;

        /// <summary>Current spline (Spline mode only).</summary>
        public SplineComputer CurrentSpline => spline;

        /// <summary>Currently active junction (Spline mode only).</summary>
        public SplineJunction ActiveJunction => activeJunction;

        /// <summary>Whether a junction is available (Spline mode only).</summary>
        public bool IsJunctionAvailable => activeJunction != null;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // Auto-detect spline if enabled and no spline assigned
            if (autoDetectSplineOnStart && spline == null)
            {
                SplineComputer foundSpline = FindMainRiverSpline();
                if (foundSpline != null)
                {
                    // Found a spline - switch to spline mode
                    Debug.Log($"[PlayerMovementController] Auto-detected spline: {foundSpline.name}. Switching to Spline mode.");
                    spline = foundSpline;
                    currentMode = MovementMode.Spline;
                }
            }

            InitializeMode();
        }

        /// <summary>
        /// Finds the main river spline, prioritizing by tag "MainRiver" or name containing "River".
        /// </summary>
        private SplineComputer FindMainRiverSpline()
        {
            SplineComputer[] allSplines = FindObjectsByType<SplineComputer>(FindObjectsSortMode.None);
            if (allSplines == null || allSplines.Length == 0) return null;

            // First pass: look for spline with "MainRiver" tag
            for (int i = 0; i < allSplines.Length; i++)
            {
                if (allSplines[i].CompareTag("MainRiver"))
                {
                    return allSplines[i];
                }
            }

            // Second pass: look for spline with "River" in name (case-insensitive)
            for (int i = 0; i < allSplines.Length; i++)
            {
                if (allSplines[i].name.ToLower().Contains("river"))
                {
                    return allSplines[i];
                }
            }

            // Fallback: return first spline found
            return allSplines[0];
        }

        private void Update()
        {
            if (currentMode == MovementMode.Free)
            {
                UpdateFreeMovement();
            }
            else
            {
                UpdateSplineMovement();
            }

            CheckMovementEvents();
        }

        #endregion

        #region Mode Management

        /// <summary>
        /// Sets the movement mode and initializes appropriate state.
        /// </summary>
        public void SetMode(MovementMode mode)
        {
            currentMode = mode;
            InitializeMode();
        }

        /// <summary>
        /// Sets the mode to Spline and assigns the spline to follow.
        /// </summary>
        public void SetSplineMode(SplineComputer newSpline, double spawnPercent = 0.5)
        {
            spline = newSpline;
            currentPercent = System.Math.Clamp(spawnPercent, 0.0, 1.0);
            currentMode = MovementMode.Spline;
            currentSpeed = 0f;
            targetSpeed = 0f;

            if (spline != null)
            {
                ApplySplinePosition();
                RefreshJunctions();
            }
        }

        /// <summary>
        /// Sets the mode to Free movement.
        /// </summary>
        public void SetFreeMode()
        {
            currentMode = MovementMode.Free;
            currentSpeed = 0f;
            targetSpeed = 0f;

            // Clean up spline state
            if (activeJunction != null)
            {
                activeJunction.UpdateIndicator(false);
                activeJunction = null;
            }
            pendingManualJunction = null;

            // Ensure at water level
            Vector3 pos = transform.position;
            pos.y = waterLevel;
            transform.position = pos;
        }

        private void InitializeMode()
        {
            if (currentMode == MovementMode.Free)
            {
                // Ensure raft starts at water level
                Vector3 pos = transform.position;
                pos.y = waterLevel;
                transform.position = pos;
            }
            else if (currentMode == MovementMode.Spline && spline != null)
            {
                currentPercent = startingPercent;
                ApplySplinePosition();
                RefreshJunctions();
            }
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Called by PlayerInput component via SendMessages or UnityEvents.
        /// </summary>
        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        /// <summary>
        /// PlayerInput -> Ferry/TakeJunction (Button). Spline mode only.
        /// </summary>
        public void OnTakeJunction(InputValue value)
        {
            if (currentMode != MovementMode.Spline) return;

            // Simply call ArmManualJunction on any press - the cooldown prevents double-triggering
            if (value.isPressed)
            {
                ArmManualJunction();
            }
        }

        /// <summary>
        /// Alternative method for direct input binding.
        /// </summary>
        public void SetMoveInput(Vector2 input)
        {
            moveInput = input;
        }

        /// <summary>
        /// Alternative non-InputValue binding for TakeJunction. Spline mode only.
        /// </summary>
        public void SetTakeJunctionPressed(bool pressed)
        {
            if (currentMode != MovementMode.Spline) return;
            if (pressed)
            {
                ArmManualJunction();
            }
        }

        #endregion

        #region Free Movement

        private void UpdateFreeMovement()
        {
            UpdateFreeVelocity();
            UpdateFreeRotation();
            ApplyFreeMovement();
        }

        private void UpdateFreeVelocity()
        {
            // Movement is always forward relative to the raft's facing direction
            // moveInput.y controls forward/back speed
            float forwardSpeed = moveInput.y * freeMaxSpeed;

            float accel = Mathf.Abs(moveInput.y) > 0.01f ? freeAcceleration : freeDeceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, forwardSpeed, accel * Time.deltaTime);
        }

        private void UpdateFreeRotation()
        {
            // moveInput.x controls rotation (turning left/right)
            if (Mathf.Abs(moveInput.x) > 0.01f)
            {
                float rotationAmount = moveInput.x * rotationSpeed * Time.deltaTime;
                transform.Rotate(0f, rotationAmount, 0f);
            }
        }

        private void ApplyFreeMovement()
        {
            if (Mathf.Abs(currentSpeed) < 0.0001f) return;

            // Move forward/back in the direction the raft is facing
            Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
            Vector3 pos = transform.position + movement;
            pos.y = waterLevel; // Keep at water level

            transform.position = pos;
        }

        /// <summary>
        /// Sets the raft position directly (Free mode).
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            position.y = waterLevel;
            transform.position = position;
        }

        /// <summary>
        /// Sets the raft position on the X-Z plane, keeping Y at water level (Free mode).
        /// </summary>
        public void SetPosition(float x, float z)
        {
            transform.position = new Vector3(x, waterLevel, z);
        }

        #endregion

        #region Spline Movement

        private void UpdateSplineMovement()
        {
            if (spline == null) return;

            if (junctionCooldown > 0f)
            {
                junctionCooldown -= Time.deltaTime;
            }

            UpdateSplineSpeed();
            TryConsumePendingManualJunction();
            ApplySplineMovement();
            CheckJunctions();
        }

        private void UpdateSplineSpeed()
        {
            // Input X controls movement along the spline.
            float inputX = moveInput.x;

            if (Mathf.Abs(inputX) > 0.1f)
            {
                targetSpeed = inputX * splineMaxSpeed;
            }
            else
            {
                targetSpeed = 0f;
            }

            float accel = Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed) ? splineAcceleration : splineDeceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.deltaTime);
        }

        private void ApplySplineMovement()
        {
            if (Mathf.Abs(currentSpeed) < 0.0001f) return;

            // Convert speed to delta percent per frame.
            double delta = (currentSpeed * Time.deltaTime) / 10.0;
            currentPercent = System.Math.Clamp(currentPercent - delta, 0.0, 1.0);

            ApplySplinePosition();
        }

        private void ApplySplinePosition()
        {
            if (spline == null) return;
            SplineSample sample = spline.Evaluate(currentPercent);
            transform.position = sample.position;

            // Rotate to align with spline direction
            // Use the spline's forward direction (tangent)
            Vector3 splineForward = sample.forward;

            // Project onto XZ plane for Y-axis rotation only
            splineForward.y = 0f;
            if (splineForward.sqrMagnitude > 0.001f)
            {
                splineForward.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(splineForward, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, splineRotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Gets the current spline percent (Spline mode).
        /// </summary>
        public double GetSplinePercent() => currentPercent;

        /// <summary>
        /// Sets the spline percent and updates position (Spline mode).
        /// </summary>
        public void SetSplinePercent(double percent)
        {
            currentPercent = System.Math.Clamp(percent, 0.0, 1.0);
            ApplySplinePosition();
        }

        /// <summary>
        /// Sets the spline to follow (Spline mode).
        /// </summary>
        public void SetSpline(SplineComputer newSpline)
        {
            if (newSpline == null) return;
            spline = newSpline;
            RefreshJunctions();
        }

        /// <summary>
        /// Sets the spline and entry percent (Spline mode).
        /// </summary>
        public void SetSpline(SplineComputer newSpline, double entryPercent)
        {
            if (newSpline == null) return;
            spline = newSpline;
            currentPercent = System.Math.Clamp(entryPercent, 0.0, 1.0);
            ApplySplinePosition();
            RefreshJunctions();
        }

        #endregion

        #region Junctions (Spline Mode Only)

        private void CheckJunctions()
        {
            int travelDirection = currentSpeed > 0.01f ? 1 : (currentSpeed < -0.01f ? -1 : 0);
            SplineJunction newActiveJunction = null;

            for (int i = 0; i < junctions.Count; i++)
            {
                SplineJunction junction = junctions[i];
                if (junction != null && junction.IsInRange(currentPercent, travelDirection))
                {
                    newActiveJunction = junction;
                    break;
                }
            }

            // Keep active junction if we're stationary and still in range
            if (newActiveJunction == null && activeJunction != null && travelDirection == 0)
            {
                float distance = Mathf.Abs((float)currentPercent - activeJunction.JunctionPercent);
                if (distance <= activeJunction.ActivationRange && activeJunction.TargetSpline != null)
                {
                    newActiveJunction = activeJunction;
                }
            }

            if (newActiveJunction != activeJunction)
            {
                if (activeJunction != null)
                {
                    activeJunction.UpdateIndicator(false);
                }

                if (newActiveJunction != null)
                {
                    newActiveJunction.UpdateIndicator(true);
                    onJunctionAvailable?.Raise();
                }

                activeJunction = newActiveJunction;

                if (pendingManualJunction != null && activeJunction != pendingManualJunction)
                {
                    pendingManualJunction = null;
                }
            }

            // Auto-take junctions
            if (activeJunction != null && !activeJunction.RequiresButtonPress && junctionCooldown <= 0f)
            {
                if (Mathf.Abs(currentSpeed) > 0.01f || Mathf.Abs(moveInput.x) > 0.1f)
                {
                    TakeJunction(activeJunction);
                }
            }
        }

        private void ArmManualJunction()
        {
            if (activeJunction == null || activeJunction.TargetSpline == null) return;
            if (!activeJunction.RequiresButtonPress) return;
            if (junctionCooldown > 0f) return; // Prevent spam during cooldown

            pendingManualJunction = activeJunction;

            // Always try to take the junction immediately when button is pressed.
            // This allows taking junctions while stationary.
            TryConsumePendingManualJunction(forceImmediate: true);
        }

        private void TryConsumePendingManualJunction(bool forceImmediate = false)
        {
            if (pendingManualJunction == null || pendingManualJunction.TargetSpline == null)
            {
                pendingManualJunction = null;
                return;
            }

            bool hasMoveIntent = Mathf.Abs(moveInput.x) > 0.1f;
            if (!forceImmediate && !hasMoveIntent) return;
            if (junctionCooldown > 0f) return;

            int travelDirection = currentSpeed > 0.01f ? 1 : (currentSpeed < -0.01f ? -1 : 0);
            if (!pendingManualJunction.IsInRange(currentPercent, travelDirection))
            {
                pendingManualJunction = null;
                return;
            }

            TakeJunction(pendingManualJunction);
            pendingManualJunction = null;
        }

        private void TakeJunction(SplineJunction junction)
        {
            if (junction == null || junction.TargetSpline == null) return;

            Debug.Log($"[Junction] {junction.name} -> {junction.TargetSpline.name}");

            Vector3 currentWorldPos = transform.position;

            if (activeJunction != null)
            {
                activeJunction.UpdateIndicator(false);
                activeJunction = null;
            }

            spline = junction.TargetSpline;

            SplineSample projectedSample = new SplineSample();
            spline.Project(currentWorldPos, ref projectedSample);
            currentPercent = System.Math.Clamp(projectedSample.percent, 0.0, 1.0);

            RefreshJunctions();
            junctionCooldown = 1.5f;
            onJunctionTaken?.Raise();
        }

        private void RefreshJunctions()
        {
            if (spline == null)
            {
                Debug.LogWarning("[PlayerMovementController] RefreshJunctions called but spline is null.");
                return;
            }

            if (junctions == null)
            {
                junctions = new List<SplineJunction>();
            }

            junctions.Clear();
            SplineJunction[] foundJunctions = spline.GetComponentsInChildren<SplineJunction>();
            junctions.AddRange(foundJunctions);
        }

        #endregion

        #region Movement Events

        private void CheckMovementEvents()
        {
            bool isMoving = IsMoving;

            if (isMoving && !wasMoving)
            {
                onRaftStartedMoving?.Raise();
            }
            else if (!isMoving && wasMoving)
            {
                onRaftStoppedMoving?.Raise();
            }

            wasMoving = isMoving;
        }

        #endregion
    }
}
