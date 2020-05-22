using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private const int CLOSED = 0;
    private const int OPENED = 1;

    #region Public & Serialized Variables
    [SerializeField] private Pannel _pannel = null;

    [SerializeField] private Door _targetDoor = null;
    [SerializeField] private Transform _doorPosition = null; //TODO: Get a reference to the target position of the door (child inside the door that marks the end of transition)
    [SerializeField] private Transform _doorOutsidePosition = null; //Reference to a different position outside of the pannel. Should be vertical or horizontal from the _doorPosition for illusion of linear movement.

    [SerializeField] private bool _isHorizontal = true;
    [SerializeField] private bool _goesRight = true;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _doorSprites;
    #endregion

    #region Private Variables
    private bool _isOpened = false;
    private int _doorState = CLOSED;
    #endregion

    #region Getters & Setters
    public Pannel GetPannel {
        get => _pannel;
    }

    public Door TargetDoor {
        get { return _targetDoor; }
        set { _targetDoor = value; }
    }

    public Transform DoorPosition {
        get => _doorPosition;
    }
    
    public Transform OutsideDoorPosition {
        get => _doorOutsidePosition;
    }

    public bool IsHorizontal { get => _isHorizontal; }

    public bool GoesRight { get => _goesRight; }

    public static bool IsPlayerInside { get; set; }

    public bool IsOpened {
        get => _isOpened;
        set {
            _isOpened = value;
            //Change state.
            if (_isOpened) { _doorState = OPENED; }
            else { _doorState = CLOSED; }
            ChangeSprite(_doorState);
        }
    }
    #endregion

    #region Unity Cycle
    private void Awake() {
        IsPlayerInside = false;
    }

    private void OnTriggerEnter(Collider other) {
        //If this door is a ladder enter ladder mode. When door has a closed state a collider needs to exist instead of the triggers => Only enter ladder mode when pressing up or down.
        if (!IsHorizontal) {
            PlayerStateManager.Instance.ChangeStateRequest(PlayerStateManager.PlayerState.LadderMovement);
        }
        PlayerTransitionManager.Instance.DoorEntered(this);
        IsPlayerInside = true;
    }

    private void OnTriggerExit(Collider other) {
        if (!IsHorizontal && PlayerStateManager.Instance.GetCurrentPlayerState == PlayerStateManager.PlayerState.LadderMovement) {
            PlayerStateManager.Instance.ChangeStateRequest(PlayerStateManager.PlayerState.FreeMovement);
        }
        IsPlayerInside = false;
    }
    #endregion

    #region API Methods
    public void BreakConnection() {
        _targetDoor = null;
        IsOpened = false;
    }

    public void CreateConnection(Door targetDoor) {
        _targetDoor = targetDoor;
        IsOpened = true;
    }

    public bool CheckConnectionAngle(float angleLimit) {
        if (_targetDoor == null) { return false; }

        bool isValid = false;
        if (Vector3.Angle(_doorOutsidePosition.position, _targetDoor.OutsideDoorPosition.position) > angleLimit) {
            isValid = true;
        }

        return isValid;
    }
    #endregion

    #region Private Functions
    private void ChangeSprite(int currentState) {
        _spriteRenderer.sprite = _doorSprites[currentState];
    }

    [ContextMenu("ToggleDoor")]
    private void TogleDoor() {
        _isOpened = !_isOpened;
        ChangeSprite(_isOpened ? OPENED : CLOSED);
    }
    #endregion
}
