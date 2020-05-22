using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Custom3DButton : MonoBehaviour {

    #region Events
    public UnityEvent OnButtonClick;
    public UnityEvent OnButtonRelease;
    #endregion

    #region Type Declaration
    #endregion

    #region Private Variables
    #endregion

    #region Getters & Setters
    #endregion

    #region Serialized Variables
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Camera _mainCamera = null;
    [SerializeField] private LayerMask _buttonLayer = 0;
    
    private InputActionAsset _actionAsset;
    private InputAction _pointerPosition, _pointerClick;
    private bool _mouseHold = false;

    private Ray _ray;
    private RaycastHit _hit;
    #endregion

    #region Private Functions
    private void MousePress(InputAction.CallbackContext actionContext) {
        //Debug.Log("Press Action has finished");
        _mouseHold = true;
        OnButtonClick.Invoke();
    }

    private void MouseRelease(InputAction.CallbackContext actionContext) {
        //Debug.Log("Press Action has been canceled");
        _mouseHold = false;
        OnButtonRelease.Invoke();
    }
    #endregion

    #region API Methods
    #endregion

    #region Unity Cycle
    private void Awake() {
        if(_playerInput == null) _playerInput = PlayerController.Instance.gameObject.GetComponent<PlayerInput>();
        if (_mainCamera == null) _mainCamera = Camera.main;

        _actionAsset = _playerInput.actions;
        _pointerPosition = _actionAsset.FindAction("Point");
        _pointerClick = _actionAsset.FindAction("UIClick");
        _pointerClick.performed += MousePress;
        _pointerClick.canceled += MouseRelease;
    }

    void Update() {
        if (_mouseHold) {
            _ray = _mainCamera.ScreenPointToRay(_pointerPosition.ReadValue<Vector2>());
            if (Physics.Raycast(_ray, out _hit, _buttonLayer)) {
                //if (_mouseHold)
                print(_hit.collider.name);
            }
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