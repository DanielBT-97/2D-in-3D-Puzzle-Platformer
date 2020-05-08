using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Pannel _pannel = null;

    [SerializeField] private Door _targetDoor = null;
    [SerializeField] private Transform _doorPosition = null; //TODO: Get a reference to the target position of the door (child inside the door that marks the end of transition)
    [SerializeField] private Transform _doorOutsidePosition = null; //Reference to a different position outside of the pannel. Should be vertical or horizontal from the _doorPosition for illusion of linear movement.

    [SerializeField] private bool _isHorizontal = true;
    [SerializeField] private bool _goesRight = true;


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
}
