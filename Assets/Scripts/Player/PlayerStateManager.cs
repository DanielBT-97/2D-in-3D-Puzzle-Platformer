using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStateManager : MonoBehaviour {
    //Singleton
    private static PlayerStateManager _instance = null;
    public static PlayerStateManager Instance {
        get => _instance;
    }

    #region Type Declaration
    public enum PlayerState
    {
        FreeMovement = 0, TransitionMovement = 1, PannelEditFreeze = 2, LadderMovement = 3, PausedGameFreeze = 4
    }
    #endregion

    #region Events
    [Header("State Enter Events")]
    public UnityEvent OnFreeMovementEnter;
    public UnityEvent OnTransitionModeEnter;
    public UnityEvent OnPannelEditEnter;
    public UnityEvent OnLadderMovementEnter;
    public UnityEvent OnPausedGameEnter;

    [Header("State Exit Events")]
    public UnityEvent OnFreeMovementExit;
    public UnityEvent OnTransitionModeExit;
    public UnityEvent OnPannelEditExit;
    public UnityEvent OnLadderMovementExit;
    public UnityEvent OnPausedGameExit;
    #endregion

    #region Private Variables
    //State Machine
    //For now I use 3 state registers: Current, Previous and Cached. (Cached is gonna be used to store states when I need to swap back to them. Previous is gonna be updated on every change)
    //Eliminated cached for now since ther is no use for it.
    private IPlayerStateHandler _currentPlayerState;
    private PlayerState _eCurrentPlayerState, _ePreviousPlayerState;
    private Dictionary<PlayerState, IPlayerStateHandler> _playerStateHandlersDictionary;
    #endregion

    #region Getters & Setters
    public PlayerState GetCurrentPlayerState {
        get => _eCurrentPlayerState;
    }

    public PlayerState GetPreviousPlayerState {
        get => _ePreviousPlayerState;
    }
    #endregion

    #region Serialized Variables
    #endregion

    #region Private Functions
    private void StateMachineInit() {
        _playerStateHandlersDictionary = new Dictionary<PlayerState, IPlayerStateHandler>() {
            { PlayerState.FreeMovement, new FreeMovementState() },
            { PlayerState.TransitionMovement, new TransitionMovementState() },
            { PlayerState.PannelEditFreeze, new FrozenMovementState() },
            { PlayerState.LadderMovement, new LadderMovementState() },
            { PlayerState.PausedGameFreeze, new PausedGameState() }
        };

        _eCurrentPlayerState = PlayerState.FreeMovement;
        _ePreviousPlayerState = _eCurrentPlayerState;
        _currentPlayerState = _playerStateHandlersDictionary[PlayerState.FreeMovement];
    }
    #endregion

    #region API Methods
    public bool ChangeStateRequest(PlayerState newState, bool forceChange = false) {
        bool changedState = false;

        if (_playerStateHandlersDictionary[newState].EnterTransitionConditions(_eCurrentPlayerState) || forceChange) {
            _currentPlayerState.Exit();

            _currentPlayerState = _playerStateHandlersDictionary[newState];
            _ePreviousPlayerState = _eCurrentPlayerState;
            _eCurrentPlayerState = newState;

            _playerStateHandlersDictionary[newState].Enter();
        }

        return changedState;
    }

    public bool RestoreLastStateRequest() {
        return ChangeStateRequest(_ePreviousPlayerState);
    }
    #endregion

    #region Unity Cycle
    private void Awake() {
        _instance = this;

        //State Machine Init
        StateMachineInit();
    }

    private void Update() {
        _currentPlayerState.Execute();
    }
    #endregion

    #region Courrutines
    #endregion
}