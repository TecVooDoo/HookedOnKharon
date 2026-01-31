using Obvious.Soap;
using UnityEngine;
using Unity.Cinemachine;

namespace HOK.Fishing
{
    /// <summary>
    /// Controls the camera during fishing, implementing Cast n' Chill style depth tracking.
    /// Camera transitions from 2.5D above-water view to near-2D side-view as hook depth increases.
    /// The waterline rises on screen as the camera descends, eventually going off the top.
    /// </summary>
    public class FishingCameraController : MonoBehaviour
    {
        [Header("SOAP Variables")]
        [Tooltip("Current fishing state - controls when fishing camera is active.")]
        [SerializeField] private IntVariable currentFishingStateVariable;
        [Tooltip("Current hook depth (0-1) - drives camera position and angle.")]
        [SerializeField] private FloatVariable hookDepthVariable;

        [Header("Camera References")]
        [Tooltip("The main Cinemachine camera to control (or leave null to find automatically).")]
        [SerializeField] private CinemachineCamera fishingCamera;
        [Tooltip("The navigation camera to return to when not fishing.")]
        [SerializeField] private CinemachineCamera navigationCamera;

        [Header("Camera Position Settings")]
        [Tooltip("Camera Y offset from water surface at minimum depth (above water view).")]
        [SerializeField] private float cameraYOffsetMin = 5f;
        [Tooltip("Camera Y offset from water surface at maximum depth (underwater view).")]
        [SerializeField] private float cameraYOffsetMax = -8f;
        [Tooltip("Water surface Y position.")]
        [SerializeField] private float waterSurfaceY = 0.3f;

        [Header("Camera Angle Settings")]
        [Tooltip("Camera X rotation (pitch) at minimum depth - angled down (2.5D view).")]
        [SerializeField] private float cameraAngleMin = 30f;
        [Tooltip("Camera X rotation (pitch) at maximum depth - flatter (near side-view).")]
        [SerializeField] private float cameraAngleMax = 5f;

        [Header("Camera Distance Settings")]
        [Tooltip("Camera Z offset (distance from raft) at minimum depth.")]
        [SerializeField] private float cameraZOffsetMin = -15f;
        [Tooltip("Camera Z offset (distance from raft) at maximum depth.")]
        [SerializeField] private float cameraZOffsetMax = -12f;

        [Header("Transition Settings")]
        [Tooltip("How fast the camera transitions to target position (higher = faster).")]
        [SerializeField] private float positionSmoothSpeed = 3f;
        [Tooltip("How fast the camera transitions to target rotation (higher = faster).")]
        [SerializeField] private float rotationSmoothSpeed = 3f;
        [Tooltip("Depth at which camera starts transitioning (for gradual entry).")]
        [SerializeField] private float transitionStartDepth = 0f;

        [Header("Follow Target")]
        [Tooltip("The transform to follow (usually the raft or Kharon).")]
        [SerializeField] private Transform followTarget;

        // Internal state
        private bool isFishingCameraActive;
        private Vector3 targetCameraPosition;
        private Quaternion targetCameraRotation;
        private float currentDepth;

        private void OnEnable()
        {
            if (hookDepthVariable != null)
            {
                hookDepthVariable.OnValueChanged += OnHookDepthChanged;
            }
        }

        private void OnDisable()
        {
            if (hookDepthVariable != null)
            {
                hookDepthVariable.OnValueChanged -= OnHookDepthChanged;
            }
        }

        private void Start()
        {
            // Try to find fishing camera if not assigned
            if (fishingCamera == null)
            {
                fishingCamera = GetComponent<CinemachineCamera>();
            }

            // Initialize depth
            currentDepth = hookDepthVariable != null ? hookDepthVariable.Value : 0f;
        }

        private void Update()
        {
            UpdateCameraActiveState();

            if (isFishingCameraActive)
            {
                UpdateCameraTarget();
                ApplyCameraSmoothing();
            }
        }

        private void OnHookDepthChanged(float newDepth)
        {
            currentDepth = newDepth;
        }

        private void UpdateCameraActiveState()
        {
            if (currentFishingStateVariable == null)
            {
                return;
            }

            FishingState state = (FishingState)currentFishingStateVariable.Value;

            // Camera is active during fishing states (not Inactive or Idle before cast)
            bool shouldBeActive = state == FishingState.Casting ||
                                  state == FishingState.LineInWater ||
                                  state == FishingState.FishBiting ||
                                  state == FishingState.Hooked ||
                                  state == FishingState.CatchResolution;

            if (shouldBeActive != isFishingCameraActive)
            {
                isFishingCameraActive = shouldBeActive;
                OnFishingCameraStateChanged();
            }
        }

        private void OnFishingCameraStateChanged()
        {
            if (isFishingCameraActive)
            {
                // Activate fishing camera
                if (fishingCamera != null)
                {
                    fishingCamera.Priority = 15;
                }
                if (navigationCamera != null)
                {
                    navigationCamera.Priority = 10;
                }

                Debug.Log("[FishingCameraController] Fishing camera activated.");
            }
            else
            {
                // Return to navigation camera
                if (fishingCamera != null)
                {
                    fishingCamera.Priority = 10;
                }
                if (navigationCamera != null)
                {
                    navigationCamera.Priority = 15;
                }

                Debug.Log("[FishingCameraController] Navigation camera activated.");
            }
        }

        private void UpdateCameraTarget()
        {
            if (followTarget == null)
            {
                return;
            }

            // Normalize depth for interpolation (accounting for transition start)
            float normalizedDepth = Mathf.InverseLerp(transitionStartDepth, 1f, currentDepth);
            normalizedDepth = Mathf.Clamp01(normalizedDepth);

            // Calculate target Y position based on depth
            // As depth increases, camera goes lower (eventually below water surface)
            float targetY = Mathf.Lerp(cameraYOffsetMin, cameraYOffsetMax, normalizedDepth);
            targetY += waterSurfaceY;

            // Calculate target Z offset (camera gets closer as depth increases)
            float targetZ = Mathf.Lerp(cameraZOffsetMin, cameraZOffsetMax, normalizedDepth);

            // Calculate target angle (flattens as depth increases for side-view)
            float targetAngle = Mathf.Lerp(cameraAngleMin, cameraAngleMax, normalizedDepth);

            // Build target position (follow target X, calculated Y, offset Z)
            Vector3 targetPos = followTarget.position;
            targetPos.y = targetY;
            targetPos.z += targetZ;

            targetCameraPosition = targetPos;
            targetCameraRotation = Quaternion.Euler(targetAngle, 0f, 0f);
        }

        private void ApplyCameraSmoothing()
        {
            if (fishingCamera == null)
            {
                return;
            }

            // Smooth position
            Vector3 currentPos = fishingCamera.transform.position;
            Vector3 newPos = Vector3.Lerp(currentPos, targetCameraPosition, positionSmoothSpeed * Time.deltaTime);
            fishingCamera.transform.position = newPos;

            // Smooth rotation
            Quaternion currentRot = fishingCamera.transform.rotation;
            Quaternion newRot = Quaternion.Slerp(currentRot, targetCameraRotation, rotationSmoothSpeed * Time.deltaTime);
            fishingCamera.transform.rotation = newRot;
        }

        /// <summary>
        /// Sets the follow target transform.
        /// </summary>
        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
        }

        /// <summary>
        /// Sets the water surface Y position.
        /// </summary>
        public void SetWaterSurface(float y)
        {
            waterSurfaceY = y;
        }

        /// <summary>
        /// Immediately snaps the camera to target position (no smoothing).
        /// </summary>
        public void SnapToTarget()
        {
            UpdateCameraTarget();

            if (fishingCamera != null)
            {
                fishingCamera.transform.position = targetCameraPosition;
                fishingCamera.transform.rotation = targetCameraRotation;
            }
        }

        /// <summary>
        /// Gets the current normalized depth being used for camera calculations.
        /// </summary>
        public float GetCurrentNormalizedDepth()
        {
            float normalizedDepth = Mathf.InverseLerp(transitionStartDepth, 1f, currentDepth);
            return Mathf.Clamp01(normalizedDepth);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (followTarget == null) return;

            // Draw camera positions at min and max depth
            Vector3 minPos = followTarget.position;
            minPos.y = waterSurfaceY + cameraYOffsetMin;
            minPos.z += cameraZOffsetMin;

            Vector3 maxPos = followTarget.position;
            maxPos.y = waterSurfaceY + cameraYOffsetMax;
            maxPos.z += cameraZOffsetMax;

            // Min depth position (above water)
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(minPos, 0.5f);
            Gizmos.DrawLine(minPos, minPos + Quaternion.Euler(cameraAngleMin, 0f, 0f) * Vector3.forward * 3f);

            // Max depth position (underwater)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(maxPos, 0.5f);
            Gizmos.DrawLine(maxPos, maxPos + Quaternion.Euler(cameraAngleMax, 0f, 0f) * Vector3.forward * 3f);

            // Water surface line
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.5f);
            Vector3 waterStart = followTarget.position + Vector3.left * 10f;
            waterStart.y = waterSurfaceY;
            Vector3 waterEnd = followTarget.position + Vector3.right * 10f;
            waterEnd.y = waterSurfaceY;
            Gizmos.DrawLine(waterStart, waterEnd);
        }
#endif
    }
}
