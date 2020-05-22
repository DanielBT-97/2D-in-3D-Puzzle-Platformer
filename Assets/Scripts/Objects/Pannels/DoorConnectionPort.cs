using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorConnectionPort : MonoBehaviour {
    #region Type Declaration
    #endregion

    #region Private Variables
    private bool _hasConnection = false;
    private bool _isDragging = false;
    private GameObject _otherPort = null;

    private Vector3 _previousMousePosition = Vector2.zero;
    #endregion

    #region Getters & Setters
    public bool HasConnection {
        get => _hasConnection;
    }
    #endregion

    #region Serialized Variables
    [SerializeField] private Door _doorController = null;
    [SerializeField] private Transform _connectionPointer = null;
    [SerializeField] private SpriteRenderer _connectionPointerRenderer = null;

    [SerializeField] private Sprite _leftPlug = null, _rightPlug = null, _fullPlug = null;
    [SerializeField] private SpriteRenderer _plugRenderer = null;

    private Vector2 _offsetToMove = Vector2.zero;
    public bool _transformToWorld = true;
    #endregion

    #region Private Functions
    private void ResetConnectionPort() {
        ResetConnectionPointer();
        ResetPlug();
    }

    private void ResetPlug() {
        _plugRenderer.sprite = _fullPlug;
    }

    private void ResetConnectionPointer() {
        _connectionPointer.position = _plugRenderer.transform.position;
        _connectionPointerRenderer.enabled = false;
    }
    #endregion

    #region API Methods
    public bool ConnectionRequested(Door otherDoor) {
        if (_hasConnection) {
            ResetConnectionPointer();
            return false; 
        }

        _doorController.TargetDoor = otherDoor;
        _plugRenderer.sprite = _doorController.GoesRight ? _leftPlug : _rightPlug;
        _hasConnection = true;

        return true;
    }

    public void ConnectionBroken() {
        Debug.Log("ConnectionBroken");
        _hasConnection = false;
        ResetConnectionPort();
    }

    public void ConnectEditStarted() {
        _previousMousePosition = GlobalInputInformation.Instance.GetMousePositionWorld(this.transform.position.z);
        _isDragging = true;
        _connectionPointerRenderer.enabled = true;
    }

    public void ConnectionEditEnded() {
        _isDragging = false;
        if (_otherPort == null) { ConnectionBroken(); return; }

        if (_otherPort != this) {
            Debug.Log("SELECTED OTHER DOOR: " + _otherPort, _otherPort);
            Debug.Log("THIS DOOR: ", this);
        } else { Debug.Log("SELECTED SAME DOOR: " + _otherPort, _otherPort); Debug.Log("THIS DOOR: ", this); ConnectionBroken(); }
    }
    #endregion

    #region Unity Cycle
    private void Update() {
        if (_isDragging) {
            Camera mainCamera = GlobalInputInformation.Instance.MainCamera;
            //Debug.Log("Dragging");

            Vector3 currentPos = GlobalInputInformation.Instance.GetMousePositionWorld(this.transform.position.z);
            _offsetToMove = currentPos - _previousMousePosition;
            if (_transformToWorld)_offsetToMove = _connectionPointer.TransformVector(_offsetToMove);
            _connectionPointer.position = new Vector3(_connectionPointer.position.x + _offsetToMove.x, _connectionPointer.position.y + _offsetToMove.y, _connectionPointer.position.z);
            
            _previousMousePosition = currentPos;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "ConnectionPointer") _otherPort = other.gameObject;
        //Other Pointer is here:
        //Flag it
        //When mouse release
    }
    #endregion

    #region Courrutines
    #endregion
}