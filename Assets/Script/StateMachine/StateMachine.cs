using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
    public static StateMachine Instance;
    public State Current { get { return _current; } }
    public Queue<Card> CardsdToPlay;
    public Queue<Unit> Units;
    public Unit CurrentUnit;

    State _current;
    bool _busy;

    [Header("UI References")]
    [SerializeField] private GameObject _battleUI;
    [SerializeField] private GameObject _rewardScreenUI;
    [SerializeField] private float _transitionDelay = 1.5f;

    public event System.Action<Unit> OnUnitChanged;

    private Unit _currentUnitProperty;
    public Unit CurrentUnitProperty
    {
        get => _currentUnitProperty;
        set
        {
            _currentUnitProperty = value;
            OnUnitChanged?.Invoke(_currentUnitProperty);
        }
    }

    void Awake()
    {
        Instance = this;
        CardsdToPlay = new Queue<Card>();
        Units = new Queue<Unit>(); // Initialize the Units queue
        Application.targetFrameRate = 60;

        // Ensure UI starts in correct state
        InitializeUI();
    }

    void Start()
    {
        //    ChangeState<LoadState>();
    }

    private void InitializeUI()
    {
        // Show battle UI, hide reward UI by default
        if (_battleUI != null)
            _battleUI.SetActive(true);

        if (_rewardScreenUI != null)
            _rewardScreenUI.SetActive(false);
    }

    #region State Control
    public void ChangeState<T>() where T : State
    {
        State state = GetState<T>();
        if (state != null)
        {
            StartCoroutine(ChangeState(state));
        }
    }

    public T GetState<T>() where T : State
    {
        T target = GetComponent<T>();
        if (target == null)
        {
            target = gameObject.AddComponent<T>();
        }
        return target;
    }

    IEnumerator ChangeState(State state)
    {
        if (_busy)
        {
            yield break;
        }
        _busy = true;

        if (_current != null)
        {
            yield return StartCoroutine(_current.Exit());
        }

        _current = state;

        if (_current != null)
        {
            yield return StartCoroutine(_current.Enter());
        }

        _busy = false;
    }
    #endregion

    #region UI Management
    public void ShowRewardScreen()
    {
        if (_battleUI != null)
            _battleUI.SetActive(false);

        if (_rewardScreenUI != null)
            _rewardScreenUI.SetActive(true);
        else
            Debug.LogError("Reward Screen UI reference not set!");
    }

    public void HideRewardScreen()
    {
        if (_rewardScreenUI != null)
            _rewardScreenUI.SetActive(false);

        if (_battleUI != null)
            _battleUI.SetActive(true);
    }

    public void ToggleUI(bool showBattleUI)
    {
        if (_battleUI != null)
            _battleUI.SetActive(showBattleUI);

        if (_rewardScreenUI != null)
            _rewardScreenUI.SetActive(!showBattleUI);
    }
    #endregion

    #region Cleanup Methods
    public void ClearAllStates()
    {
        // Clear the current state reference
        _current = null;

        // Remove all State components from this GameObject
        State[] states = GetComponents<State>();
        foreach (State state in states)
        {
            Destroy(state);
        }

        Debug.Log("All state components cleared from StateMachine");
    }

    public void ResetStateMachine()
    {
        // Clear all states
        ClearAllStates();

        // Clear queues
        CardsdToPlay.Clear();
        Units.Clear();
        CurrentUnit = null;
        CurrentUnitProperty = null;

        // Reset UI to initial state
        InitializeUI();

        Debug.Log("StateMachine fully reset");
    }

    #endregion
}