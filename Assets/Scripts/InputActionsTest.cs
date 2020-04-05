using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionsTest : MonoBehaviour
{

    public PlayerInput _playerInput;
    private InputActionAsset _actionAsset;
    public PlayerController _playerController;

    private void Awake() {
        _actionAsset = _playerInput.actions;
    }

    private void OnEnable() {
        _actionAsset.FindAction("Move").started += MoveAction;
        _actionAsset.FindAction("Move").canceled += MoveAction;
        _actionAsset.FindAction("Jump").performed += SimpleDebug;
    }

    private void OnDisable() {
        _actionAsset.FindAction("Move").started -= MoveAction;
        _actionAsset.FindAction("Move").canceled -= MoveAction;
        _actionAsset.FindAction("Jump").performed -= SimpleDebug;
    }

    private void SimpleDebug(InputAction.CallbackContext actionContext) {
        Debug.Log(actionContext.action.phase);
    }

    private void MoveAction(InputAction.CallbackContext callbackContext) {
        _playerController.MovementUpdate(callbackContext.action.ReadValue<Vector2>());
    }
}
