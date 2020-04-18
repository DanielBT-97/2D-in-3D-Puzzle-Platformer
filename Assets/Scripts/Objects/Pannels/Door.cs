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

    private void OnTriggerEnter(Collider other) {
        Debug.Log("TRIGGER");
        PlayerTransitionManager.Instance.DoorEntered(this);
    }
}
