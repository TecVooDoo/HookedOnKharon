using System.Collections.Generic;
using HOK.Core;
using HOK.Data;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HOK.Fishing
{
    /// <summary>
    /// Main fishing controller that manages the fishing state machine, input handling,
    /// and coordinates all fishing subsystems (casting, hook timing, reeling).
    /// </summary>
    public class FishingController : MonoBehaviour
    {
        [Header("SOAP Variables")]
        [Tooltip("Current fishing state as int (maps to FishingState enum).")]
        [SerializeField] private IntVariable currentFishingStateVariable;
        [Tooltip("Current line tension (0-1).")]
        [SerializeField] private FloatVariable lineTensionVariable;
        [Tooltip("Current hook depth below water surface (0-1).")]
        [SerializeField] private FloatVariable hookDepthVariable;
        [Tooltip("Current Scorch proximity (0-1).")]
        [SerializeField] private FloatVariable scorchProximityVariable;

        [Header("SOAP Events - Input")]
        [Tooltip("Game state changed event from GameManager.")]
        [SerializeField] private ScriptableEventInt onGameStateChanged;

        [Header("SOAP Events - Output")]
        [SerializeField] private ScriptableEventInt onFishingStateChanged;
        [SerializeField] private ScriptableEventNoParam onCastStarted;
        [SerializeField] private ScriptableEventNoParam onFishBite;
        [SerializeField] private ScriptableEventNoParam onFishHooked;
        [SerializeField] private ScriptableEventNoParam onFishCaught;
        [SerializeField] private ScriptableEventNoParam onFishLost;

        [Header("Fish Pool")]
        [Tooltip("Available fish definitions for this fishing spot.")]
        [SerializeField] private List<FishPoolEntry> fishPool = new List<FishPoolEntry>();

        [Header("Cast Settings")]
        [Tooltip("Minimum cast distance in world units.")]
        [SerializeField] private float minCastDistance = 2f;
        [Tooltip("Maximum cast distance in world units.")]
        [SerializeField] private float maxCastDistance = 10f;
        [Tooltip("Duration of cast animation in seconds.")]
        [SerializeField] private float castDuration = 0.5f;
        [Tooltip("How deep the hook sinks after cast (0-1).")]
        [SerializeField] private float defaultHookDepth = 0.3f;

        [Header("Hook Timing Settings")]
        [Tooltip("Duration of hook window in seconds (base, modified by fish).")]
        [SerializeField] private float baseHookWindowDuration = 1.0f;

        [Header("Tension Settings")]
        [Tooltip("Rate at which tension increases while reeling.")]
        [SerializeField] private float reelTensionRate = 0.3f;
        [Tooltip("Rate at which tension decreases when not reeling (fish pulls line).")]
        [SerializeField] private float slackTensionRate = 0.15f;
        [Tooltip("Tension threshold for line break (sustained).")]
        [SerializeField] private float tensionBreakThreshold = 0.95f;
        [Tooltip("Duration tension must be at break threshold to snap line.")]
        [SerializeField] private float tensionBreakDuration = 0.5f;
        [Tooltip("Tension threshold below which fish escapes (sustained).")]
        [SerializeField] private float tensionEscapeThreshold = 0.1f;
        [Tooltip("Duration tension must be below escape threshold for fish to escape.")]
        [SerializeField] private float tensionEscapeDuration = 2.0f;

        [Header("Fish Stamina")]
        [Tooltip("Base stamina for fish (depletes during reeling).")]
        [SerializeField] private float baseFishStamina = 10f;
        [Tooltip("Rate at which fish stamina depletes while reeling in safe zone.")]
        [SerializeField] private float staminaDepletionRate = 1f;

        [Header("References")]
        [Tooltip("Transform where the fishing line originates (rod tip).")]
        [SerializeField] private Transform rodTipTransform;
        [Tooltip("Transform representing the hook/lure position.")]
        [SerializeField] private Transform hookTransform;

        // State machine
        private FishingState currentState = FishingState.Inactive;

        // Input state
        private Vector2 aimInput;
        private bool castPressed;
        private bool reelHeld;
        private bool hookPressed;

        // Cast state
        private Vector3 castTargetPosition;
        private float castTimer;

        // Bite state
        private float biteTimer;
        private float hookWindowTimer;
        private FishDefinition currentTargetFish;

        // Hooked state
        private FishDefinition hookedFish;
        private float fishStamina;
        private float tensionBreakTimer;
        private float tensionEscapeTimer;
        private float nextStruggleTime;

        // Structs
        [System.Serializable]
        public struct FishPoolEntry
        {
            public FishDefinition fish;
            [Range(0f, 100f)] public float spawnWeight;
        }

        #region Public Properties

        /// <summary>Current fishing state.</summary>
        public FishingState CurrentState => currentState;

        /// <summary>Whether fishing is active (not Inactive state).</summary>
        public bool IsFishing => currentState != FishingState.Inactive;

        /// <summary>The currently hooked fish, if any.</summary>
        public FishDefinition HookedFish => hookedFish;

        /// <summary>Current line tension (0-1).</summary>
        public float LineTension => lineTensionVariable != null ? lineTensionVariable.Value : 0f;

        /// <summary>Current hook depth (0-1).</summary>
        public float HookDepth => hookDepthVariable != null ? hookDepthVariable.Value : 0f;

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            if (onGameStateChanged != null)
            {
                onGameStateChanged.OnRaised += HandleGameStateChanged;
            }
        }

        private void OnDisable()
        {
            if (onGameStateChanged != null)
            {
                onGameStateChanged.OnRaised -= HandleGameStateChanged;
            }
        }

        private void Update()
        {
            if (currentState == FishingState.Inactive)
            {
                return;
            }

            switch (currentState)
            {
                case FishingState.Idle:
                    UpdateIdle();
                    break;
                case FishingState.Casting:
                    UpdateCasting();
                    break;
                case FishingState.LineInWater:
                    UpdateLineInWater();
                    break;
                case FishingState.FishBiting:
                    UpdateFishBiting();
                    break;
                case FishingState.Hooked:
                    UpdateHooked();
                    break;
                case FishingState.CatchResolution:
                    UpdateCatchResolution();
                    break;
            }
        }

        #endregion

        #region State Machine

        private void SetFishingState(FishingState newState)
        {
            if (currentState == newState) return;

            FishingState previousState = currentState;
            currentState = newState;

            // Update SOAP variable
            if (currentFishingStateVariable != null)
            {
                currentFishingStateVariable.Value = (int)newState;
            }

            Debug.Log($"[FishingController] State: {previousState} -> {newState}");

            // Raise event
            onFishingStateChanged?.Raise((int)newState);

            // State entry logic
            OnStateEnter(newState);
        }

        private void OnStateEnter(FishingState state)
        {
            switch (state)
            {
                case FishingState.Idle:
                    ResetFishingState();
                    break;
                case FishingState.Casting:
                    StartCast();
                    break;
                case FishingState.LineInWater:
                    StartBiteTimer();
                    break;
                case FishingState.FishBiting:
                    StartHookWindow();
                    break;
                case FishingState.Hooked:
                    StartReeling();
                    break;
                case FishingState.CatchResolution:
                    // Resolution handled by UpdateCatchResolution
                    break;
            }
        }

        private void HandleGameStateChanged(int newGameState)
        {
            GameState gameState = (GameState)newGameState;

            if (gameState == GameState.Fishing)
            {
                EnterFishingMode();
            }
            else if (currentState != FishingState.Inactive)
            {
                ExitFishingMode();
            }
        }

        private void EnterFishingMode()
        {
            Debug.Log("[FishingController] Entering fishing mode.");
            SetFishingState(FishingState.Idle);
        }

        private void ExitFishingMode()
        {
            Debug.Log("[FishingController] Exiting fishing mode.");
            SetFishingState(FishingState.Inactive);
            ResetFishingState();
        }

        private void ResetFishingState()
        {
            // Reset input state
            castPressed = false;
            reelHeld = false;
            hookPressed = false;

            // Reset fishing state
            hookedFish = null;
            currentTargetFish = null;
            fishStamina = 0f;
            tensionBreakTimer = 0f;
            tensionEscapeTimer = 0f;

            // Reset SOAP variables
            if (lineTensionVariable != null) lineTensionVariable.Value = 0f;
            if (hookDepthVariable != null) hookDepthVariable.Value = 0f;
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Called by PlayerInput for Cast action (button press).
        /// </summary>
        public void OnCast(InputValue value)
        {
            castPressed = value.isPressed;

            if (castPressed && currentState == FishingState.Idle)
            {
                SetFishingState(FishingState.Casting);
            }
        }

        /// <summary>
        /// Called by PlayerInput for Reel action (button hold).
        /// </summary>
        public void OnReel(InputValue value)
        {
            reelHeld = value.isPressed;
        }

        /// <summary>
        /// Called by PlayerInput for Hook action (button press).
        /// </summary>
        public void OnHook(InputValue value)
        {
            hookPressed = value.isPressed;

            if (hookPressed && currentState == FishingState.FishBiting)
            {
                AttemptHook();
            }
        }

        /// <summary>
        /// Called by PlayerInput for AimDirection action (Vector2).
        /// </summary>
        public void OnAimDirection(InputValue value)
        {
            aimInput = value.Get<Vector2>();
        }

        /// <summary>
        /// Triggers the start of fishing mode (call from Ferry action map).
        /// </summary>
        public void OnStartFishing(InputValue value)
        {
            if (!value.isPressed) return;

            // Only allow starting fishing if we're in OffDuty state
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.OffDuty)
            {
                // Check if raft is stationary (you may need to get this from PlayerMovementController)
                GameManager.Instance.SetState(GameState.Fishing);
            }
        }

        /// <summary>
        /// Exits fishing mode and returns to OffDuty.
        /// </summary>
        public void OnCancelFishing(InputValue value)
        {
            if (!value.isPressed) return;

            if (currentState != FishingState.Inactive)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetState(GameState.OffDuty);
                }
            }
        }

        #endregion

        #region Idle State

        private void UpdateIdle()
        {
            // Update aim direction visualization (handled by separate component)
            // Cast is initiated via OnCast input
        }

        #endregion

        #region Casting State

        private void StartCast()
        {
            castTimer = 0f;

            // Calculate cast target position based on aim
            Vector3 castDirection = CalculateCastDirection();
            float castDistance = Mathf.Lerp(minCastDistance, maxCastDistance, 0.5f); // TODO: Use cast power
            castTargetPosition = transform.position + castDirection * castDistance;

            onCastStarted?.Raise();
        }

        private void UpdateCasting()
        {
            castTimer += Time.deltaTime;

            // Animate hook position (lerp from rod tip to target)
            float t = Mathf.Clamp01(castTimer / castDuration);
            if (hookTransform != null && rodTipTransform != null)
            {
                // Arc trajectory
                Vector3 startPos = rodTipTransform.position;
                Vector3 endPos = castTargetPosition;
                Vector3 midPos = Vector3.Lerp(startPos, endPos, 0.5f);
                midPos.y += 2f; // Arc height

                // Quadratic bezier
                Vector3 currentPos = CalculateBezierPoint(t, startPos, midPos, endPos);
                hookTransform.position = currentPos;
            }

            if (castTimer >= castDuration)
            {
                // Cast complete - hook hits water
                if (hookTransform != null)
                {
                    hookTransform.position = castTargetPosition;
                }

                // Set initial hook depth
                if (hookDepthVariable != null)
                {
                    hookDepthVariable.Value = defaultHookDepth;
                }

                SetFishingState(FishingState.LineInWater);
            }
        }

        private Vector3 CalculateCastDirection()
        {
            // Default forward direction
            Vector3 direction = transform.forward;

            // Modify based on aim input if available
            if (aimInput.sqrMagnitude > 0.01f)
            {
                // Convert 2D aim to 3D direction on XZ plane
                direction = new Vector3(aimInput.x, 0f, aimInput.y).normalized;
                if (direction.sqrMagnitude < 0.01f)
                {
                    direction = transform.forward;
                }
            }

            return direction;
        }

        private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1f - t;
            return u * u * p0 + 2f * u * t * p1 + t * t * p2;
        }

        #endregion

        #region LineInWater State

        private void StartBiteTimer()
        {
            // Select a fish to target
            currentTargetFish = SelectFishFromPool();

            if (currentTargetFish != null)
            {
                biteTimer = currentTargetFish.GetRandomBiteTime();
                Debug.Log($"[FishingController] Waiting for bite: {currentTargetFish.FishName} (timer: {biteTimer:F1}s)");
            }
            else
            {
                // No fish available - use default timer
                biteTimer = Random.Range(5f, 15f);
                Debug.Log("[FishingController] No fish in pool, using default bite timer.");
            }
        }

        private void UpdateLineInWater()
        {
            biteTimer -= Time.deltaTime;

            // Slowly sink the hook deeper over time
            if (hookDepthVariable != null)
            {
                float newDepth = Mathf.MoveTowards(hookDepthVariable.Value, 1f, 0.02f * Time.deltaTime);
                hookDepthVariable.Value = newDepth;
            }

            if (biteTimer <= 0f)
            {
                // Fish bite!
                SetFishingState(FishingState.FishBiting);
            }
        }

        private FishDefinition SelectFishFromPool()
        {
            if (fishPool == null || fishPool.Count == 0) return null;

            float scorchProx = scorchProximityVariable != null ? scorchProximityVariable.Value : 0f;

            // Build weighted list of eligible fish
            float totalWeight = 0f;
            List<FishPoolEntry> eligibleFish = new List<FishPoolEntry>();

            for (int i = 0; i < fishPool.Count; i++)
            {
                FishPoolEntry entry = fishPool[i];
                if (entry.fish == null) continue;

                // Check Scorch proximity requirement
                if (!entry.fish.CanSpawnWithScorchProximity(scorchProx)) continue;

                eligibleFish.Add(entry);
                totalWeight += entry.spawnWeight;
            }

            if (eligibleFish.Count == 0 || totalWeight <= 0f) return null;

            // Weighted random selection
            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < eligibleFish.Count; i++)
            {
                cumulative += eligibleFish[i].spawnWeight;
                if (roll <= cumulative)
                {
                    return eligibleFish[i].fish;
                }
            }

            return eligibleFish[eligibleFish.Count - 1].fish;
        }

        #endregion

        #region FishBiting State

        private void StartHookWindow()
        {
            if (currentTargetFish != null)
            {
                hookWindowTimer = currentTargetFish.BiteWindowDuration;
            }
            else
            {
                hookWindowTimer = baseHookWindowDuration;
            }

            onFishBite?.Raise();
            Debug.Log($"[FishingController] Fish biting! Hook window: {hookWindowTimer:F2}s");
        }

        private void UpdateFishBiting()
        {
            hookWindowTimer -= Time.deltaTime;

            if (hookWindowTimer <= 0f)
            {
                // Missed the hook window
                Debug.Log("[FishingController] Hook window missed! Fish escaped.");
                onFishLost?.Raise();

                // Reset to LineInWater to try again
                SetFishingState(FishingState.LineInWater);
            }
        }

        private void AttemptHook()
        {
            if (currentState != FishingState.FishBiting) return;

            // Success! Fish is hooked
            hookedFish = currentTargetFish;
            Debug.Log($"[FishingController] Hooked: {(hookedFish != null ? hookedFish.FishName : "Unknown fish")}");

            onFishHooked?.Raise();
            SetFishingState(FishingState.Hooked);
        }

        #endregion

        #region Hooked State (Reeling)

        private void StartReeling()
        {
            // Initialize tension
            if (lineTensionVariable != null)
            {
                lineTensionVariable.Value = 0.5f; // Start at mid tension
            }

            // Initialize fish stamina
            if (hookedFish != null)
            {
                fishStamina = baseFishStamina * (1f + hookedFish.BaseCatchDifficulty);
            }
            else
            {
                fishStamina = baseFishStamina;
            }

            tensionBreakTimer = 0f;
            tensionEscapeTimer = 0f;
            nextStruggleTime = Random.Range(1f, 3f);
        }

        private void UpdateHooked()
        {
            UpdateTension();
            UpdateFishStamina();
            CheckCatchConditions();
        }

        private void UpdateTension()
        {
            if (lineTensionVariable == null) return;

            float tension = lineTensionVariable.Value;

            // Fish pull (always increases tension unless player gives slack)
            float fishPull = (hookedFish != null ? hookedFish.EscapeStrength : 1f) * 0.1f * Time.deltaTime;

            // Player input
            float playerInput = 0f;
            if (reelHeld)
            {
                // Reeling increases tension
                float reelRate = reelTensionRate * (hookedFish != null ? hookedFish.TensionMultiplier : 1f);
                playerInput = reelRate * Time.deltaTime;
            }
            else
            {
                // Not reeling - tension decreases (giving slack)
                playerInput = -slackTensionRate * Time.deltaTime;
            }

            // Fish struggles (random tension spikes)
            nextStruggleTime -= Time.deltaTime;
            if (nextStruggleTime <= 0f)
            {
                float struggleIntensity = hookedFish != null ? hookedFish.EscapeStrength * 0.15f : 0.1f;
                fishPull += Random.Range(struggleIntensity * 0.5f, struggleIntensity);
                nextStruggleTime = Random.Range(1f, 3f);
            }

            // Apply tension changes
            tension = Mathf.Clamp01(tension + fishPull + playerInput);
            lineTensionVariable.Value = tension;

            // Update hook depth based on tension
            // Low tension = hook sinks, high tension = hook rises
            if (hookDepthVariable != null)
            {
                float depthChange = (tension - 0.5f) * -0.1f * Time.deltaTime;
                hookDepthVariable.Value = Mathf.Clamp01(hookDepthVariable.Value + depthChange);
            }
        }

        private void UpdateFishStamina()
        {
            if (lineTensionVariable == null) return;

            float tension = lineTensionVariable.Value;

            // Fish loses stamina when tension is in the "safe zone" (0.3 - 0.6)
            // and player is reeling
            if (reelHeld && tension >= 0.3f && tension <= 0.6f)
            {
                fishStamina -= staminaDepletionRate * Time.deltaTime;
            }
        }

        private void CheckCatchConditions()
        {
            if (lineTensionVariable == null) return;

            float tension = lineTensionVariable.Value;

            // Check for line break (tension too high)
            if (tension >= tensionBreakThreshold)
            {
                tensionBreakTimer += Time.deltaTime;
                if (tensionBreakTimer >= tensionBreakDuration)
                {
                    Debug.Log("[FishingController] Line snapped! Fish escaped.");
                    FishEscaped();
                    return;
                }
            }
            else
            {
                tensionBreakTimer = 0f;
            }

            // Check for fish escape (tension too low while fish struggles)
            if (tension <= tensionEscapeThreshold)
            {
                tensionEscapeTimer += Time.deltaTime;
                if (tensionEscapeTimer >= tensionEscapeDuration)
                {
                    Debug.Log("[FishingController] Too much slack! Fish escaped.");
                    FishEscaped();
                    return;
                }
            }
            else
            {
                tensionEscapeTimer = 0f;
            }

            // Check for successful catch (fish stamina depleted)
            if (fishStamina <= 0f)
            {
                Debug.Log($"[FishingController] Caught: {(hookedFish != null ? hookedFish.FishName : "Unknown fish")}");
                FishCaught();
            }
        }

        private void FishCaught()
        {
            onFishCaught?.Raise();
            SetFishingState(FishingState.CatchResolution);
        }

        private void FishEscaped()
        {
            hookedFish = null;
            onFishLost?.Raise();
            SetFishingState(FishingState.CatchResolution);
        }

        #endregion

        #region CatchResolution State

        private void UpdateCatchResolution()
        {
            // TODO: Play catch/escape animation
            // For now, immediately return to Idle

            // Reset hook position
            if (hookTransform != null && rodTipTransform != null)
            {
                hookTransform.position = rodTipTransform.position;
            }

            SetFishingState(FishingState.Idle);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Manually start fishing mode (for testing or alternative input).
        /// </summary>
        public void StartFishing()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Fishing);
            }
        }

        /// <summary>
        /// Manually exit fishing mode.
        /// </summary>
        public void StopFishing()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.OffDuty);
            }
        }

        /// <summary>
        /// Set the Scorch proximity value (called by Scorch companion).
        /// </summary>
        public void SetScorchProximity(float proximity)
        {
            if (scorchProximityVariable != null)
            {
                scorchProximityVariable.Value = Mathf.Clamp01(proximity);
            }
        }

        #endregion
    }
}
