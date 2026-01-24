using Obvious.Soap;
using UnityEngine;

namespace HOK.Core
{
    /// <summary>
    /// Persistent game manager that survives scene loads.
    /// Uses SOAP for state management and event communication.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("SOAP Variables")]
        [Tooltip("Current game state as int (maps to GameState enum)")]
        [SerializeField] private IntVariable currentStateVariable;

        [Header("SOAP Events")]
        [Tooltip("Fired when game state changes. Passes new state as int.")]
        [SerializeField] private ScriptableEventInt onGameStateChanged;

        public GameState CurrentState => (GameState)currentStateVariable.Value;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeState();
        }

        private void InitializeState()
        {
            if (currentStateVariable != null)
            {
                currentStateVariable.Value = (int)GameState.OffDuty;
            }
        }

        public void SetState(GameState newState)
        {
            if (currentStateVariable == null)
            {
                Debug.LogWarning("[GameManager] CurrentStateVariable not assigned.");
                return;
            }

            int newStateInt = (int)newState;
            if (currentStateVariable.Value == newStateInt) return;

            GameState previousState = (GameState)currentStateVariable.Value;
            currentStateVariable.Value = newStateInt;

            Debug.Log($"[GameManager] State changed: {previousState} -> {newState}");

            if (onGameStateChanged != null)
            {
                onGameStateChanged.Raise(newStateInt);
            }
        }
    }
}
