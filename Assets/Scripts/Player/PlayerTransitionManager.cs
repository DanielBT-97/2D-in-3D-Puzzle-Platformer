using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// For now this works going from left to right.
/// 
/// Problem 1:  When trying to enter from the right door the system does everything correct except the comparisons are flawed.
///             The code assumes the player will be moving forward in the path when moving right but forward is going left when entering from the right.
///             
/// Solution 1: Make different doors for right and left using a bool. Make different doors for vertical and horizontal using a bool
///             In case I want to make the map editor I just need to have some way for the user to specify if the door is going right or left upon creation.
///             I use that right or left bool to set an int to (1 || -1) which is used as a multiplier for the input before making the movement checks and computations.
///             => This makes so that desiredMovement == 1 will always mean forward in the path no matter the actual input or direction I'm moving.
/// </summary>
public class PlayerTransitionManager : MonoBehaviour {
    //Singleton
    private static PlayerTransitionManager _instance = null;
    public static PlayerTransitionManager Instance { get => _instance; }

    #region Type Declaration
    [System.Serializable]
    private class PathNode {
        public Vector3 _position;
        public Quaternion _rotation;
        public Door _door;
    }
    #endregion

    #region Private Variables
    [SerializeField] Door _doorEntered = null;
    private Pannel _nextPannel = null;
    private int _forwardDirection = 1;  //This int should always be either 1 or -1. When activating the transition mode the player has to be pushing some direction that would be forward. This registers that.
    #endregion

    #region Getters & Setters
    #endregion

    #region Serialized Variables
    [SerializeField] private PathNode[] _path = new PathNode[4];
    [SerializeField] private Vector3 _currentPosition = Vector3.zero;
    [SerializeField] private float _lerpValue = 0f;
    [SerializeField] private int _previousNode = 0, _nextNode = 0;
    #endregion

    #region Private Functions
    private void NextNode(int direction) {
        _lerpValue = (direction == 1) ? 0 : 1;  //If going right (forwards) the next starting value should be 0 | If going left (backwards) the next starting value should be 1
        _previousNode += direction;
        _nextNode += direction;
    }

    private float FindClosestLerpInPath(Vector3 playerPosition) {
        float newLerp = 0f;

        float totalDistance = Vector3.Distance(_path[_nextNode]._position, _path[_previousNode]._position);
        float toPlayerDistance = Vector3.Distance(playerPosition, _path[_previousNode]._position);
        newLerp = toPlayerDistance / totalDistance;

        return newLerp;
    }
    #endregion

    #region API Methods
    public void CreateNewPathWithRegistered() {
        Debug.Log("CREATE A NEW PATH");
        if (_doorEntered.TargetDoor == null) {
            Debug.LogError("New path has been requested by player but the door had no connection but still was opened. Something has gone wrong. Click this to see what door.", _doorEntered.gameObject);
            return;
        }   //No next door assigned.

        if (_doorEntered.IsHorizontal) _forwardDirection = _doorEntered.GoesRight ? 1 : -1;
        else _forwardDirection = _doorEntered.GoesRight ? 1 : -1;

        Door nextDoor = _doorEntered.TargetDoor;
        Vector3 positionDifference = Vector3.zero;
        if (_doorEntered.IsHorizontal) { //Right or Left
            positionDifference.y = PlayerController.Instance.PlayerTrans.position.y - _doorEntered.DoorPosition.position.y;
            //_path[0]._position = new Vector3(_doorEntered.DoorPosition.position.x, PlayerController.Instance.PlayerTrans.position.y, _doorEntered.DoorPosition.position.z);    //Maybe use the players position as the starting point?
            //_path[1]._position = new Vector3(_doorEntered.OutsideDoorPosition.position.x, PlayerController.Instance.PlayerTrans.position.y, _doorEntered.OutsideDoorPosition.position.z);    //Maybe use the players position as the starting point?
        } else { //Down or Up
            //positionDifference.x = PlayerController.Instance.PlayerTrans.position.x - _doorEntered.OutsideDoorPosition.position.x;
            //_path[0]._position = new Vector3(PlayerController.Instance.PlayerTrans.position.x, _doorEntered.DoorPosition.position.y, _doorEntered.DoorPosition.position.z);    //Maybe use the players position as the starting point?
            //_path[1]._position = new Vector3(PlayerController.Instance.PlayerTrans.position.x, _doorEntered.OutsideDoorPosition.position.y, _doorEntered.OutsideDoorPosition.position.z);    //Maybe use the players position as the starting point?
        }
        _path[0]._position = _doorEntered.DoorPosition.position + positionDifference;
        _path[0]._rotation = _doorEntered.DoorPosition.rotation;    //Maybe use the players position as the starting point?
        _path[0]._door = _doorEntered;

        _path[1]._position = _doorEntered.OutsideDoorPosition.position + positionDifference;
        _path[1]._rotation = _doorEntered.OutsideDoorPosition.rotation;    //Maybe use the players position as the starting point?
        _path[1]._door = _doorEntered;

        _path[2]._position = nextDoor.OutsideDoorPosition.position + positionDifference;
        _path[2]._rotation = nextDoor.OutsideDoorPosition.rotation;
        _path[2]._door = nextDoor;

        _path[3]._position = nextDoor.DoorPosition.position + positionDifference;
        _path[3]._rotation = nextDoor.DoorPosition.rotation;
        _path[3]._door = nextDoor;

        _currentPosition = _path[0]._position;
        _nextPannel = nextDoor.GetPannel;

        _lerpValue = 0;
        _previousNode = 0;
        _nextNode = 1;

        _lerpValue = Mathf.Clamp(FindClosestLerpInPath(PlayerController.Instance.PlayerTrans.position), 0, 1);
    }

    /*public void CreateNewPath(Door doorEntered) {
        if (doorEntered.TargetDoor == null) {
            Debug.LogError("New path has been requested by player but the door had no connection but still was opened. Something has gone wrong. Click this to see what door.", doorEntered.gameObject);
            return;
        }   //No next door assigned.

        if (doorEntered.IsHorizontal) _forwardDirection = Mathf.FloorToInt(PlayerController.Instance.MovementInput.x);
        else _forwardDirection = Mathf.FloorToInt(PlayerController.Instance.MovementInput.y);

        Door nextDoor = doorEntered.TargetDoor;
        _path[0]._position = PlayerController.Instance.PlayerTrans.position;    //Maybe use the players position as the starting point?
        _path[0]._rotation = PlayerController.Instance.PlayerTrans.rotation;    //Maybe use the players position as the starting point?
        _path[0]._door = doorEntered;    //Maybe use the players position as the starting point?

        _path[1]._position = doorEntered.OutsideDoorPosition.position;    //Maybe use the players position as the starting point?
        _path[1]._rotation = doorEntered.OutsideDoorPosition.rotation;    //Maybe use the players position as the starting point?
        _path[1]._door = doorEntered;    //Maybe use the players position as the starting point?

        _path[2]._position = nextDoor.OutsideDoorPosition.position;
        _path[2]._rotation = nextDoor.OutsideDoorPosition.rotation;
        _path[2]._door = nextDoor;

        _path[3]._position = nextDoor.DoorPosition.position;
        _path[3]._rotation = nextDoor.DoorPosition.rotation;
        _path[3]._door = nextDoor;

        _currentPosition = _path[0]._position;
        _nextPannel = nextDoor.GetPannel;
        _lerpValue = 0;
        _previousNode = 0;
        _nextNode = 1;
    }*/

    /// <summary>
    /// This function should return the position the player should be heading towards in a straight line.
    /// It uses the information from the connection created beforehand between two doors or ladders.
    /// A connection is independent between vertical or horizontal. The only difference being what axis of the input we use to determine the movement. [X-Axis for doors | Y-Axis for ladders]
    /// </summary>
    /// <param name="inputMovement"></param>
    /// <returns></returns>
    public Transform GetTargetPosition(Vector2 inputMovement) {
        Transform targetPosition = null;

        return targetPosition;
    }

    public Vector3 GetTargetPosition(int inputMovement) {
        float distance = Vector3.Distance(_path[_previousNode]._position, _path[_nextNode]._position);
        float newSpeed = (distance / PlayerController.Instance.TransitionSpeed);
        if(newSpeed != 0) _lerpValue += inputMovement * (Time.deltaTime / (newSpeed));
        Vector3 targetPosition = Vector3.Lerp(_path[_previousNode]._position, _path[_nextNode]._position, _lerpValue);
        return targetPosition;
    }

    public void DoorEntered(Door door) {
        _doorEntered = door;
    }

    public void TransitionModeMovement(Vector2 input) {
        int desiredMovement = Mathf.RoundToInt(_doorEntered.IsHorizontal ? input.x : input.y);
        
        desiredMovement *= _forwardDirection;   //This makes sure that positive is always forward without being dependent of right or left.
        if ((PlayerController.Instance.PlayerTrans.position != _path[_nextNode]._position && desiredMovement > 0) || (PlayerController.Instance.PlayerTrans.position != _path[_previousNode]._position && desiredMovement < 0) || desiredMovement == 0) {
            _currentPosition = GetTargetPosition(desiredMovement);
            PlayerController.Instance.PlayerTrans.position = _currentPosition;
        } else {
            if ((_nextNode < _path.Length - 1 && desiredMovement > 0) || (_previousNode > 0 && desiredMovement < 0)) {
                NextNode(desiredMovement);
            } else {
                Debug.Log("END TRANSITION: DESIRED = " + desiredMovement + "Next Node = " + _nextNode + "Previous Node = " + _previousNode);
                PlayerStateManager.Instance.RestoreLastStateRequest();
            }
        }
    }
    #endregion

    #region Unity Cycle
    private void Awake() {
        _instance = this;
    }

    /*private void Update() {
        if (PlayerStateManager.Instance.GetCurrentPlayerState != PlayerStateManager.PlayerState.TransitionMovement) return;

        int desiredMovement = 0;
        if (Input.GetKey(KeyCode.D)) {
            //Got right
            ++desiredMovement;
        }
        if (Input.GetKey(KeyCode.A)) {
            //Got right
            --desiredMovement;
        }
        desiredMovement *= _forwardDirection;   //This makes sure that positive is always forward without being dependent of right or left.
        if ((PlayerController.Instance.PlayerTrans.position != _path[_nextNode]._position && desiredMovement > 0) || (PlayerController.Instance.PlayerTrans.position != _path[_previousNode]._position && desiredMovement < 0) || desiredMovement == 0) {
            _currentPosition = GetTargetPosition(desiredMovement);
            PlayerController.Instance.PlayerTrans.position = _currentPosition;
        } else {
            if ((_nextNode < _path.Length - 1 && desiredMovement > 0) || (_previousNode > 0 && desiredMovement < 0)) {
                NextNode(desiredMovement);
            } else {
                Debug.Log("END TRANSITION: DESIRED = " + desiredMovement + "Next Node = " + _nextNode + "Previous Node = " + _previousNode);
                PlayerStateManager.Instance.RestoreLastStateRequest();
            }
        }
    }*/
    #endregion

    #region Courrutines
    #endregion
}