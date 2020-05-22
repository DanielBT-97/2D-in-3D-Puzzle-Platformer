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
        Debug.Log("Press Action has finished");
        _mouseHold = true;
        OnButtonClick.Invoke();
        //if (actionContext.performed) {  } else if (actionContext.canceled) {  }
    }

    private void MouseRelease(InputAction.CallbackContext actionContext) {
        Debug.Log("Press Action has been canceled");
        _mouseHold = false;
        OnButtonRelease.Invoke();
    }
    #endregion

    #region API Methods
    #endregion

    #region Unity Cycle
    private void Awake() {
        _actionAsset = _playerInput.actions;
        _pointerPosition = _actionAsset.FindAction("Point");
        _pointerClick = _actionAsset.FindAction("UIClick");
        _pointerClick.performed += MousePress;
        _pointerClick.canceled += MouseRelease;
    }
    #endregion

    #region Courrutines
    #endregion

    void Update() {
        if (_mouseHold) {
            _ray = _mainCamera.ScreenPointToRay(_pointerPosition.ReadValue<Vector2>());
            if (Physics.Raycast(_ray, out _hit, _buttonLayer)) {
                //if (_mouseHold)
                print(_hit.collider.name);
            }
        }
    }
}