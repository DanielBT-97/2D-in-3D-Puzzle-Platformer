using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Custom3DButton : MonoBehaviour {

    #region Events
    public UnityEvent OnButtonClick;
    public UnityEvent OnButtonRelease;
    public UnityEvent ActionCancle;
    public UnityEvent ActionPerformed;
    #endregion

    #region Type Declaration
    #endregion

    #region Private Variables
    private InputActionAsset _actionAsset;
    private InputAction _pointerPosition, _pointerClick;
    private bool _mouseHold = false;
    private GameObject _buttonPressed = null;

    private Ray _ray;
    private RaycastHit _hit;
    #endregion

    #region Getters & Setters
    #endregion

    #region Serialized Variables
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Camera _mainCamera = null;
    [SerializeField] private LayerMask _buttonLayer = 0;
    [SerializeField] private bool _allowReleaseEverywhere = true;
    #endregion

    #region Private Functions
    private void MousePress(InputAction.CallbackContext actionContext) {
        //Debug.Log("Press Action has finished");
        _ray = _mainCamera.ScreenPointToRay(_pointerPosition.ReadValue<Vector2>());
        if (Physics.Raycast(_ray, out _hit, float.PositiveInfinity, _buttonLayer)) {
            GameObject tempPressed = _hit.collider.gameObject;
            if (tempPressed == this.gameObject) {
                Debug.Log("Pressed", this);
                _mouseHold = true; _buttonPressed = this.gameObject; OnButtonClick.Invoke(); 
            }
        }
    }

    private void MouseRelease(InputAction.CallbackContext actionContext) {
        if (_mouseHold) { 
            _ray = _mainCamera.ScreenPointToRay(_pointerPosition.ReadValue<Vector2>());
            if (Physics.Raycast(_ray, out _hit, float.PositiveInfinity, _buttonLayer)) {
                //Debug.Log("Press Action has been canceled");
                if (_hit.collider.gameObject == this.gameObject || _allowReleaseEverywhere) {
                    //Released in an area that allows for action to be performed (Press Release)
                    ActionPerformed.Invoke();
                } else {
                    ActionCancle.Invoke();
                }
                _mouseHold = false;
                OnButtonRelease.Invoke();
            } else { ActionCancle.Invoke(); }
        }
    }
    #endregion

    #region API Methods
    public void DebugMessage(string message) {
        Debug.Log(message, this);
    }
    #endregion

    #region Unity Cycle
    //This is called between Awake and Start
    private void Main() {
        if(_playerInput == null) _playerInput = PlayerController.Instance.gameObject.GetComponent<PlayerInput>();
        if (_mainCamera == null) _mainCamera = Camera.main;

        _actionAsset = _playerInput.actions;
        _pointerPosition = _actionAsset.FindAction("Point");
        _pointerClick = _actionAsset.FindAction("UIClick");
        _pointerClick.performed += MousePress;
        _pointerClick.canceled += MouseRelease;
    }

    /*void Update() {
        if (_mouseHold) {
            _ray = _mainCamera.ScreenPointToRay(_pointerPosition.ReadValue<Vector2>());
            if (Physics.Raycast(_ray, out _hit, _buttonLayer)) {
                GameObject tempPressed = _hit.collider.gameObject;
                if (_buttonPressed == null) _buttonPressed = _hit.collider.gameObject;  //First frame where you click in button
                //if (_mouseHold)
                if(_buttonPressed == tempPressed) print(_hit.collider.name);
            }
        }
    }*/

    private void Update() {
        if(_mouseHold) {
            Debug.DrawLine(_ray.origin, _hit.point, Color.red, 0.5f);
        }
    }
    #endregion

    #region Courrutines
    #endregion


    [ContextMenu("Fill References")]
    private void LookForCameraAndInput() {
        _mainCamera = Camera.main;
        _playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
    }
}