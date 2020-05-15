using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This script controls the transition movement for the player while in the TransitionModeState.
/// The player has to enter a door and be able to move towards the other end of the connection (for both doors and ladders)
/// To avoid mid transition changes the path is created and saved in an array of nodes that cannot be updated until you exit the transition mode (since it does not use a reference to transforms).
/// 
/// Two triggers are assigned to doors: 
///     The first one (big one the same size of the sprite) that makes the door call DoorEntered() and register itself as the door that originates the transition.
///     The second one (small one outside the pannel) is the one that activates the state change in the player making him enter TransitionModeState.
/// (Since the player cannot enter TransitionModeState when he is already in that state there is no danger of recreating/reseting the path when crossing the second trigger mid transition on either doors)
/// 
/// The path is composed of 4 points:
///     0: Entered door position inside the pannel. This position marks one end of the path.
///     1: Entered door position outside the pannel. This one is used to simulate the 2D movement look when exiting a pannel or entering a door.
///     2: Target door position outside the pannel. Same as 1 but for the other door.
///     3: Target door position inside the pannel. This position marks the other end of the path.
/// This makes makes the player exit and enter the pannel as if the movement was purely 2D when in reality it is moving in 3D space.
/// 
/// To move through the path I use the player's input to add to the lerp value. 
/// Each door has two booleans: GoesRight & IsHorizontal. 
/// IsHorizontal is used to know if the path nodes need to be adjusted verticaly or horizontaly.
/// GoesRight is used to know what input value (1 or -1) is considered forward.
/// 
/// TODO DANI: [TransitionProblems]
/// Possible problems: 
///     1. The player moves in 3D but even when moving he still renders since the player is set to render ontop of every other sprite so a Stencil Buffer needs to be used.
///        Even with a Stencil Buffer the player will render when going through another pannel so some kind of code needs to set what pannels write to the stencil buffer (only current and target pannel)
///     2. Collisions should not be a probleme since we are moving the player using position and not the rigidbody but a manager needs to be done for activating and deactivating colisions on all pannels but the current and target ones.
///     
/// Possible solutions:
///     1. + Make it so that you cannot stack pannels on top of eachother like in The Pedestrian.
///        + Might make the player not really move in 3D but just teleport to the nodes 1 and 2 and just use _currentPosition for the checks instead of the player's. Use this position as the camera follow target.
///        ==> This makes the transition safer than having the player really move in the 3D space. However, it doesn't mean that I don't have to make the Stencil Testing or the collision management part if I still 
///            let the player stack pannels on top of eachother. 
///        
///     2. The manager for collisions might be tricky for two reasons:
///        - If I want to make a level editor I need to be able to dynamicly know what objects to disable and enable to manage the collisions.
///        - Collisions are tied to the render since they are childs of the same parent object (Platform => Child 1 = Render | Child 2 = Colliders)
///          If I want to be able to disable colliders but not renders a ton of references need to be set.
///        
///        I can think of three things to do:
///         (a) Brute force it and pass the references for every object to a each pannel and loop around it. Can do in for build but for level editor it would be really taxing (but possible) to do.
///         (b) Seperate each object into two parts: Render and Collision. Each part is a seperated object in the hierarchy. This has the same problem as (a) when saving a level
///             but in runtime you do not have to loop around so many references, you can just disable the Collisions parent in each pannel and forget about it.
///         (c) Use layers to disable collision. Same as (a) but doesn't disable objects but just changes the layers to one that does not interact with the player. Might be better for performance (?)
///         
///        Reminder: When a pannel has no connections leading to it all collisions should be disabled. Only pannels that are directly connected to the pannel the player is has its collisions active.
///                  Even when a connection is done it must be valid in order to activate collisions, which means the path must lead to the starting pannel AND the angle the line between 
///                  the doors inside a connection must be between -45 and 45 degrees using the direction the door leads.  
///                  => This prevents paths from crossing through the pannels composing the connection. (although would not matter if the player is teleported instead of moving in 3D).
/// </summary>
public class PlayerTransitionManager : MonoBehaviour {
    //Singleton
    private static PlayerTransitionManager _instance = null;
    public static PlayerTransitionManager Instance { get => _instance; }

    #region Type Declaration
    //[System.Serializable]
    private class PathNode {
        public Vector3 _position;
        public Quaternion _rotation;
        public Door _door;
    }
    #endregion

    #region Private Variables
    [SerializeField] Door _doorEntered = null;  //Door entered. This is the door that starts the path.
    private Pannel _nextPannel = null;  //The pannel you will be entering.
    private int _forwardDirection = 1;  //This int should always be either 1 or -1. When activating the transition mode the player has to be pushing some direction that would be forward. This registers that.
    private float _transitionSpeedMultiplier = 1f;
    private bool _togglePlayerRender = true;
    #endregion

    #region Getters & Setters
    #endregion

    #region Serialized Variables
    private PathNode[] _path = new PathNode[4];    //The path the player will follow to transition between pannels.
    [SerializeField] private Vector3 _currentPosition = Vector3.zero; //Essentialy a backup for where the player should be every frame of the transition.
    [SerializeField] private float _lerpValue = 0f; //Used as the lerp value to move between path nodes using a Lerp function. (0 -> 1)
    [SerializeField] private int _previousNode = 0, _nextNode = 0;  //Current nodes to move from and to. (0 -> 3)
    #endregion

    #region Private Functions
    /// <summary>
    /// This function changes the nodes the player is moving between.
    /// </summary>
    /// <param name="direction">Direction we want to move. 1 == Forward | -1 == Backward</param>
    private void NextNode(int direction) {
        int nodeReached = _previousNode + Mathf.RoundToInt(_lerpValue);
        IntermediaryNodeReached(nodeReached, nodeReached + direction);
        _lerpValue = (direction == 1) ? 0 : 1;  //If going right (forwards) the next starting value should be 0 | If going left (backwards) the next starting value should be 1
        _previousNode += direction;
        _nextNode += direction;
    }

    /// <summary>
    /// This is a simple function that return the closest Lerp value between the currently used nodes.
    /// Used at the start of the transition mode because the player will always be between nodes 0 and 1 the first iteration of the movement. To avoid a teleport we move the lerp forward to match the player's position.
    /// (Since the path is adjusted to be aligned with the player's position upon start it will always be collinear with the nodes 0 and 1 so orthogonal projection is not needed)
    /// </summary>
    /// <param name="playerPosition">Current player position.</param>
    /// <returns>Starting lerp value.</returns>
    private float FindClosestLerpInPath(Vector3 playerPosition) {
        float newLerp = 0f;

        float totalDistance = Vector3.Distance(_path[_nextNode]._position, _path[_previousNode]._position);
        float toPlayerDistance = Vector3.Distance(playerPosition, _path[_previousNode]._position);
        newLerp = toPlayerDistance / totalDistance;

        return newLerp;
    }

    /// <summary>
    /// Main movement function that takes the wanted movement direction to move between the current two nodes.
    /// Takes the player's transition speed to move at the same speed independently of distance between nodes.
    /// </summary>
    /// <param name="inputMovement">Wanted movement direction (1 = Forward | -1 = Backward).</param>
    /// <returns>New position for the player.</returns>
    private Vector3 GetTargetPosition(int inputMovement) {
        float distance = Vector3.Distance(_path[_previousNode]._position, _path[_nextNode]._position);
        float newSpeed = (distance / (PlayerController.Instance.TransitionSpeed * _transitionSpeedMultiplier));
        if (newSpeed != 0) _lerpValue += inputMovement * (Time.deltaTime / (newSpeed));
        Vector3 targetPosition = Vector3.Lerp(_path[_previousNode]._position, _path[_nextNode]._position, _lerpValue);
        return targetPosition;
    }

    private void IntermediaryNodeReached(int nodeReachedIndex, int targetNodeIndex) {
        _togglePlayerRender = !_togglePlayerRender;
        PlayerController.Instance.Renderer.gameObject.SetActive(_togglePlayerRender);
        //Debug.Log("NODE REACHED: " + nodeReachedIndex + "TARGET NODE: " + targetNodeIndex);
        PlayerController.Instance.TargetPannel = _path[nodeReachedIndex]._door.GetPannel.transform;
        _transitionSpeedMultiplier = (nodeReachedIndex + targetNodeIndex == 3) ? 2.5f : 1;    //sum == 3 is beacuse the indexes can only be adjacent and the result 3 is only possible if player is traveling between intermidiate nodes (1 and 2).
        //TODO DANI: Use variable dependent of distance between intermediate nodes (1 and 2)
    }
    #endregion

    #region API Methods
    /// <summary>
    /// This function creates the path the player will follow to move through a connection between two doors/ladders.
    /// Takes _doorEntered as one end of the connections and uses the info stored in it to create the path.
    /// Both ends need to be set for a connection to be opened.
    /// </summary>
    public void CreateNewPathWithRegistered() {
        if (_doorEntered == null || _doorEntered.TargetDoor == null) {
            Debug.LogError("New path has been requested by player but the door had no connection but still was opened. Something has gone wrong. Click this to see what door originated request.", _doorEntered.gameObject);
            return;
        }   //No next door assigned.

        //Debug.Log("CREATE PATH");

        //Registers which direction forward should be. If the door goes right 1 is forward if not the -1 is forward.
        //This makes it so that I don't need to differentiate between different doors when moving.
        _forwardDirection = _doorEntered.GoesRight ? 1 : -1;

        Door nextDoor = _doorEntered.TargetDoor;

        //The idea here is to move the path points taking into account the player's position to avoid having it move up and down when following the path. (Weird mecanical/teleporting look)
        //Doors' paths are adjusted in the Y-Axis to match the player's vertical position when starting the transition.
        //Ladders' paths are adjusted in the X-Axis to match the player's horizontal position when starting the transition.
        Vector3 positionDifference = Vector3.zero;
        if (_doorEntered.IsHorizontal) { //Right or Left
            positionDifference.y = PlayerController.Instance.PlayerTrans.position.y - _doorEntered.DoorPosition.position.y;
        } else { //Down or Up
            positionDifference.x = PlayerController.Instance.PlayerTrans.position.x - _doorEntered.DoorPosition.position.x;
        }

        _path[0]._position = _doorEntered.DoorPosition.position + positionDifference;
        _path[0]._rotation = _doorEntered.DoorPosition.rotation;
        _path[0]._door = _doorEntered;

        _path[1]._position = _doorEntered.OutsideDoorPosition.position + positionDifference;
        _path[1]._rotation = _doorEntered.OutsideDoorPosition.rotation;
        _path[1]._door = _doorEntered;

        _path[2]._position = nextDoor.OutsideDoorPosition.position + positionDifference;
        _path[2]._rotation = nextDoor.OutsideDoorPosition.rotation;
        _path[2]._door = nextDoor;

        _path[3]._position = nextDoor.DoorPosition.position + positionDifference;
        _path[3]._rotation = nextDoor.DoorPosition.rotation;
        _path[3]._door = nextDoor;

        _currentPosition = _path[0]._position;
        _nextPannel = nextDoor.GetPannel;

        //_lerpValue = 0;
        _previousNode = 0;
        _nextNode = 1;
        _transitionSpeedMultiplier = 1f;

        _lerpValue = Mathf.Clamp(FindClosestLerpInPath(PlayerController.Instance.PlayerTrans.position), 0, 1);
    }

    
    /// <summary>
    /// Register the door the player has entered.
    /// </summary>
    /// <param name="door">Door used as the start of the path.</param>
    public void DoorEntered(Door door) {
        _doorEntered = door;
    }

    /// <summary>
    /// Method that manages the player's movement during TransitionModeState.
    /// If the "door" is horizontal it will use the X-Axis input otherwise it will use the Y-Axis.
    /// </summary>
    /// <param name="input">This frame input.</param>
    public void TransitionModeMovement(Vector2 input) {
        int desiredMovement = Mathf.RoundToInt(_path[0]._door.IsHorizontal ? input.x : input.y);
        
        desiredMovement *= _forwardDirection;   //This makes sure that positive is always forward without being dependent of right or left input.

        //If the player wants to move between current nodes.
        if ((PlayerController.Instance.PlayerTrans.position != _path[_nextNode]._position && desiredMovement > 0) || (PlayerController.Instance.PlayerTrans.position != _path[_previousNode]._position && desiredMovement < 0) || desiredMovement == 0) {
            _currentPosition = GetTargetPosition(desiredMovement);
            PlayerController.Instance.PlayerTrans.position = _currentPosition;
        } else {
            //If player is at a node's position and wants to go to the next set of two nodes.
            if ((_nextNode < _path.Length - 1 && desiredMovement > 0) || (_previousNode > 0 && desiredMovement < 0)) {
                NextNode(desiredMovement);
            } else { //If want to move to next set of nodes BUT at the end of either sides of the path revert to previous player state (FreeMovement || LadderMovement) => END OF TRANSITION MODE
                //Debug.Log("END TRANSITION: DESIRED = " + desiredMovement + "Next Node = " + _nextNode + "Previous Node = " + _previousNode);
                PlayerStateManager.Instance.RestoreLastStateRequest(forceChange: !_path[3]._door.IsHorizontal);
            }
        }
    }
    #endregion

    #region Unity Cycle
    private void Awake() {
        _instance = this;
        _path = new PathNode[4] { new PathNode(), new PathNode(), new PathNode(), new PathNode() };
    }
    #endregion

    #region Courrutines
    #endregion
}