using Dreamteck.Splines;
using UnityEngine;

namespace HOK.Ferry
{
    /// <summary>
    /// Marks a junction point on a spline where the raft can branch to an alternate route.
    /// Place this on a GameObject with a SplineComputer, or reference the spline directly.
    /// </summary>
    public class SplineJunction : MonoBehaviour
    {
        [Header("Junction Position")]
        [Tooltip("Position on the source spline where this junction is located (0-1)")]
        [SerializeField] [Range(0f, 1f)] private float junctionPercent = 0.5f;

        [Tooltip("How close the raft must be to trigger junction availability (in percent)")]
        [SerializeField] [Range(0.01f, 0.2f)] private float activationRange = 0.05f;

        [Header("Target Route")]
        [Tooltip("The spline to switch to when taking this junction")]
        [SerializeField] private SplineComputer targetSpline;

        [Tooltip("Position on the target spline where the raft will enter (0-1)")]
        [SerializeField] [Range(0f, 1f)] private float targetEntryPercent = 0f;

        [Tooltip("Direction the raft should be traveling to use this junction (1 = forward, -1 = backward, 0 = either)")]
        [SerializeField] private int requiredDirection = 0;

        [Header("Visual Indicator")]
        [Tooltip("Optional GameObject to show/hide when junction is available")]
        [SerializeField] private GameObject indicatorObject;

        [Header("State")]
        [SerializeField] private bool isAvailable = true;

        [Tooltip("If true, this junction triggers automatically when the raft reaches spline percent 0 (dead-end return)")]
        [SerializeField] private bool autoReturnAtStart = false;

        /// <summary>
        /// The spline this junction belongs to. Cached from parent or sibling.
        /// </summary>
        public SplineComputer SourceSpline { get; private set; }

        /// <summary>
        /// Position on the source spline (0-1).
        /// </summary>
        public float JunctionPercent => junctionPercent;

        /// <summary>
        /// Range around junction percent where it can be activated.
        /// </summary>
        public float ActivationRange => activationRange;

        /// <summary>
        /// The spline to transition to.
        /// </summary>
        public SplineComputer TargetSpline => targetSpline;

        /// <summary>
        /// Entry position on target spline (0-1).
        /// </summary>
        public float TargetEntryPercent => targetEntryPercent;

        /// <summary>
        /// Required travel direction (-1, 0, or 1).
        /// </summary>
        public int RequiredDirection => requiredDirection;

        /// <summary>
        /// Whether this junction is currently usable.
        /// </summary>
        public bool IsAvailable
        {
            get => isAvailable;
            set
            {
                isAvailable = value;
                UpdateIndicator(false);
            }
        }

        /// <summary>
        /// If true, this junction auto-triggers when raft reaches spline percent 0.
        /// Used for dead-end branches that return to main river.
        /// </summary>
        public bool AutoReturnAtStart => autoReturnAtStart;

        private void Awake()
        {
            // Try to find source spline on this object or parent
            SourceSpline = GetComponent<SplineComputer>();
            if (SourceSpline == null)
            {
                SourceSpline = GetComponentInParent<SplineComputer>();
            }

            if (SourceSpline == null)
            {
                Debug.LogError($"SplineJunction '{name}' could not find a SplineComputer. " +
                    "Place this component on or under a GameObject with SplineComputer.", this);
            }

            UpdateIndicator(false);
        }

        /// <summary>
        /// Checks if the raft is within range to use this junction.
        /// </summary>
        /// <param name="currentPercent">Raft's current position on the spline (0-1)</param>
        /// <param name="travelDirection">Raft's current travel direction (-1, 0, or 1)</param>
        /// <returns>True if junction can be activated</returns>
        public bool IsInRange(double currentPercent, int travelDirection)
        {
            if (!isAvailable || targetSpline == null)
            {
                return false;
            }

            // Check direction requirement
            if (requiredDirection != 0 && travelDirection != 0 && requiredDirection != travelDirection)
            {
                return false;
            }

            // Check distance from junction point
            float distance = Mathf.Abs((float)currentPercent - junctionPercent);
            return distance <= activationRange;
        }

        /// <summary>
        /// Shows or hides the visual indicator.
        /// </summary>
        public void UpdateIndicator(bool showActive)
        {
            if (indicatorObject != null)
            {
                indicatorObject.SetActive(isAvailable && showActive);
            }
        }

        /// <summary>
        /// Gets the world position of this junction on the source spline.
        /// </summary>
        public Vector3 GetWorldPosition()
        {
            if (SourceSpline == null)
            {
                return transform.position;
            }

            SplineSample sample = SourceSpline.Evaluate(junctionPercent);
            return sample.position;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw junction point
            SplineComputer spline = GetComponent<SplineComputer>();
            if (spline == null)
            {
                spline = GetComponentInParent<SplineComputer>();
            }

            if (spline != null)
            {
                SplineSample sample = spline.Evaluate(junctionPercent);

                // Junction point
                Gizmos.color = isAvailable ? Color.green : Color.red;
                Gizmos.DrawWireSphere(sample.position, 0.5f);

                // Activation range
                Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
                float rangeStart = Mathf.Max(0f, junctionPercent - activationRange);
                float rangeEnd = Mathf.Min(1f, junctionPercent + activationRange);

                SplineSample startSample = spline.Evaluate(rangeStart);
                SplineSample endSample = spline.Evaluate(rangeEnd);

                Gizmos.DrawLine(startSample.position, endSample.position);

                // Arrow to target
                if (targetSpline != null)
                {
                    SplineSample targetSample = targetSpline.Evaluate(targetEntryPercent);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(sample.position, targetSample.position);
                    Gizmos.DrawWireSphere(targetSample.position, 0.3f);
                }
            }
        }
    }
}
