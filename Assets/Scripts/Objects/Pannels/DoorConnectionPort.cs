using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the Port of each door.
/// Manages sprite changes for the socket, plub and pointer.
/// Also manages communicating with other doors when entering other's ports.
/// </summary>
public class DoorConnectionPort : MonoBehaviour {
    #region Type Declaration
    #endregion

    #region Private Variables
    private bool _hasConnection = false;    //Flags whether or not this port is already connected to another door.
    private bool _isDragging = false;       //Flags whether or not the mouse is being pressed after pressing in this port.
    private DoorConnectionPort _otherPort = null;   //When entering another port (or itself) this will be set to that port. Used when ending edit mode to communicate. //TODO DANI: Change from GO to DoorConnectionPort.

    private Vector3 _previousMousePosition = Vector2.zero;  //Previous frame mouse position.
    #endregion

    #region Getters & Setters
    public bool HasConnection {
        get => _hasConnection;
    }

    public Door DoorController {
        get => _doorController;
    }

    public DoorConnectionLine LineController {
        get => _connectionLineController;
    }
    #endregion

    #region Serialized Variables
    [SerializeField] private Door _doorController = null;   //This port's door controller.
    [SerializeField] private Transform _connectionPointer = null;   //This port's pointer transform.
    [SerializeField] private SpriteRenderer _connectionPointerRenderer = null;  //This port's pointer renderer.
    [SerializeField] private DoorConnectionLine _connectionLineController = null; //This is the ports line renderer.

    [SerializeField] private Sprite _leftPlug = null, _rightPlug = null, _fullPlug = null;  //This port's plugs different sprite variations.
    [SerializeField] private SpriteRenderer _plugRenderer = null;   //This port's plug renderer.

    private Vector2 _offsetToMove = Vector2.zero;   //Offset the port's pointer needs to move per frame.
    private bool _transformToWorld = true;  //Will be toggled to test when pannel areas will have different orientations.
    #endregion

    #region Private Functions
    /// <summary>
    /// Reset the door's port entirely. Used when a connection is broken or not completed after dragging the pointer.
    /// </summary>
    private void ResetConnectionPort() {
        ResetConnectionPointer();
        ResetPlug();
        _doorController.BreakConnection();
    }

    /// <summary>
    /// Reset the plug to its original sprite.
    /// </summary>
    private void ResetPlug() {
        _plugRenderer.sprite = _fullPlug;
    }

    /// <summary>
    /// Reset the connection pointer to its original position and sprite.
    /// </summary>
    private void ResetConnectionPointer() {
        _connectionPointer.position = _plugRenderer.transform.position;
        _connectionPointerRenderer.enabled = false;
        //_connectionLineController.HideLine();
    }

    /// <summary>
    /// This function takes the mouse position in 3D and makes the connection pointer follow it in order to move it around the 3D world.
    /// Two versions of the follow exist (one commented out):
    ///     Direct position follow per frame (commented out)
    ///     Follow using offset mouse movement per frame.
    /// </summary>
    private void ConnectionPointerFollow() {
        Vector3 currentPos = GlobalInputInformation.Instance.GetMousePositionWorld(this.transform.position.z);
        _offsetToMove = currentPos - _previousMousePosition;
        if (_transformToWorld) _offsetToMove = _connectionPointer.TransformVector(_offsetToMove);
        _connectionPointer.position = new Vector3(_connectionPointer.position.x + _offsetToMove.x, _connectionPointer.position.y + _offsetToMove.y, _connectionPointer.position.z); //Offset movement
        //_connectionPointer.position = new Vector3(currentPos.x, currentPos.y, _connectionPointer.position.z); //Direct position follow

        _previousMousePosition = currentPos;
    }

    private void ValidConnectionPortEntered(Collider other) {
        if (other.gameObject.TryGetComponent<DoorConnectionPort>(out DoorConnectionPort temp)) { 
            if (temp == this || temp.HasConnection || temp.DoorController.GoesRight == this.DoorController.GoesRight) { 
                return; 
            }
            _otherPort = temp;
        }
    }
    #endregion

    #region API Methods
    /// <summary>
    /// Method called when the player has clicked on the port's plug and started dragging.
    /// </summary>
    public void ConnectEditStarted() {
        if (HasConnection) {
            _otherPort.ConnectionBroken();
            ConnectionBroken();
        }

        _connectionLineController.ShowLine();
        _otherPort = null;
        _previousMousePosition = GlobalInputInformation.Instance.GetMousePositionWorld(this.transform.position.z);
        _isDragging = true;
        _plugRenderer.sprite = _doorController.GoesRight ? _leftPlug : _rightPlug;
        _connectionPointerRenderer.enabled = true;
    }

    /// <summary>
    /// Method called by other DoorConnectionPort when the player releases the mouse on top of another door's port.
    /// </summary>
    /// <param name="otherDoor">The other door requesting the connection.</param>
    /// <returns>Returns true if the connection was possible.</returns>
    public bool ConnectionRequested(DoorConnectionPort otherDoor) {
        if (_hasConnection) {
            //ResetConnectionPointer();
            return false; 
        }

        _otherPort = otherDoor;
        _doorController.TargetDoor = otherDoor.DoorController;
        _doorController.CreateConnection(otherDoor.DoorController);
        _plugRenderer.sprite = _doorController.GoesRight ? _leftPlug : _rightPlug;
        _hasConnection = true;
        _connectionLineController.HideLine();
        ResetConnectionPointer();

        return true;
    }

    /// <summary>
    /// This is used when a connection is broken and the port needs to be reset.
    /// Used when dragging is stopped without being inside another port or when a connection is deleted.
    /// </summary>
    public void ConnectionBroken() {
        Debug.Log("ConnectionBroken");
        _hasConnection = false;
        ResetConnectionPort();
        _connectionLineController.ResetLine();
    }

    /// <summary>
    /// Called when the player stops dragging and releases the mouse click.
    /// Determins what situation the edit has ended: 
    ///     1. No other port has been entered
    ///     2. Pointer has entered another port and the player has released the mouse while still being inside it.
    ///     3. The player has entered the same port it started so no connection to be made. (Failsafe verification)
    /// </summary>
    public void ConnectionEditEnded() {
        _isDragging = false;
        if (_otherPort == null || _otherPort == this) { ConnectionBroken(); return; }

        if (_otherPort.ConnectionRequested(this)) {
            _hasConnection = true;
            _doorController.CreateConnection(_doorController);
            _doorController.TargetDoor = _otherPort._doorController;
            _connectionLineController.TargetDoorPort = _otherPort.LineController.PlugTransform;
        }

        ResetConnectionPointer();
        //Debug.Log("SELECTED OTHER DOOR: " + _otherPort, _otherPort);
        //Debug.Log("THIS DOOR: ", this);
        // else { Debug.Log("SELECTED SAME DOOR: " + _otherPort, _otherPort); Debug.Log("THIS DOOR: ", this); ConnectionBroken(); }
    }
    #endregion

    #region Unity Cycle
    private void Update() {
        if (_isDragging) {
            //Debug.Log("Dragging");
            ConnectionPointerFollow();
        }
    }

    /// <summary>
    /// When a connection pointer enteres the port of this door it registers itself. Used when ending the edit of the connection.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "ConnectionPort") {
            ValidConnectionPortEntered(other);
        }  //TODO DANI: add check to avoid setting _otherPort to itself.
        //Other Pointer is here:
        //Flag it
        //When mouse release
    }

    /// <summary>
    /// When a connection pointer exits this port we reset the value of _other port to null.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {
        //if (other.tag == "ConnectionPort") _otherPort = null;
    }
    #endregion

    #region Courrutines
    #endregion
}