﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalInputInformation : MonoBehaviour {

    private static GlobalInputInformation _instance = null;
    public static GlobalInputInformation Instance {
        get => _instance;
    }

    #region Type Declaration
    #endregion

    #region Private Variables
    private InputActionAsset _actionAsset;
    private InputAction _pointerPosition, _pointerClick;
    #endregion

    #region Getters & Setters
    public Vector2 GetMousePositionScreen {
        get {
            return _pointerPosition.ReadValue<Vector2>();
        }
    }

    

    public Camera MainCamera {
        get => _mainCamera;
    }
    #endregion

    #region Serialized Variables
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Camera _mainCamera = null;
    #endregion

    #region Private Functions
    private void MousePress(InputAction.CallbackContext actionContext) {
        //Debug.Log("Press Action has finished");
        //OnButtonClick.Invoke();
    }
    #endregion

    #region API Methods
    public Vector2 GetMousePositionWorld(float zValue) {
        Vector3 currentPos = GetMousePositionScreen;
        currentPos.z = zValue - (_mainCamera.transform.position.z - _mainCamera.nearClipPlane);
        currentPos = _mainCamera.ScreenToWorldPoint(currentPos);

        return currentPos;
    }
    #endregion

    #region Unity Cycle
    private void Awake() {
        _instance = this;
    }

    private void Main() {
        if (_playerInput == null) _playerInput = PlayerController.Instance.gameObject.GetComponent<PlayerInput>();
        if (_mainCamera == null) _mainCamera = Camera.main;

        _actionAsset = _playerInput.actions;
        _pointerPosition = _actionAsset.FindAction("Point");
        _pointerClick = _actionAsset.FindAction("UIClick");
        //_pointerClick.performed += MousePress;
        //_pointerClick.canceled += MouseRelease;
    }
    #endregion

    #region Courrutines
    #endregion
}