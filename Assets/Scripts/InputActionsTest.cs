using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputActionsTest : MonoBehaviour
{
    public PlayerInput _playerInput;
    public PlayerController _playerController;

    private InputActionAsset _actionAsset;
    private InputAction _movementAction, _jumpAction;
    private InputAction _testAction;
    private bool _isObjectActive = false;
    //private bool _movementUpdated = false;

    private void Awake() {
        _actionAsset = _playerInput.actions;
        _movementAction = _actionAsset.FindAction("Move");
        _jumpAction = _actionAsset.FindAction("Jump");
        _testAction = _actionAsset.FindAction("Test");
        
    }

    private void OnEnable() {
        //_isObjectActive = true;
        //_testAction.started += ActionDebug;
        //_testAction.performed += ActionDebug;
        //_testAction.canceled += ActionDebug;
        //_testAction.started += context => TestNonMatchingFunction(context.ReadValue<float>());
        //_movementAction.started += ActionDebug;
        //_movementAction.canceled += ActionDebug;
        //_movementAction.performed += ActionDebug;
        InputSystem.onDeviceChange += DeviceChangeDebug;
        InputSystem.onLayoutChange += SchemeLayoutChangeDebug;
        InputUser.onChange += ControlSchemeChangedDebug;
    }

    private void OnDisable() {
        //_isObjectActive = false;
        //_testAction.started -= ActionDebug;
        //_testAction.performed -= ActionDebug;
        //_testAction.canceled -= ActionDebug;
        //_movementAction.started -= ActionDebug;
        //_movementAction.canceled -= ActionDebug;
        //_movementAction.performed -= ActionDebug;
        InputSystem.onDeviceChange -= DeviceChangeDebug;
        InputSystem.onLayoutChange -= SchemeLayoutChangeDebug;
        InputUser.onChange -= ControlSchemeChangedDebug;
    }

    private void SimpleDebug(InputAction.CallbackContext actionContext) {
        Debug.Log(actionContext.action.phase);
    }

    /// <summary>
    /// This function is called when there is a change in the control schemes. 
    ///     There can be 4 different changes, to diferenciate between them use the parameter "change"
    ///     In this case we want to know when a control scheme has been changed, know which one (using the name of the control scheme specified in the InputAsset we created => "Gamepad")
    ///     Execute an event to which UI scripts would be subscribed to, this way they can change the UI button representation accordingly
    ///     !!! If only string representation for the control is needed you can simply use the Display Name of the device control (InputSystem Debugger has the list of controls with their respective info)    !!!
    ///     !!! When a control scheme is changed it will automaticaly change the display from that point forwards                                                                                               !!! 
    ///     !!! --> OFC this makes that already displayed info will not get updated so you might want to combine the event below PLUS the Display Name instead of changing sprites you only change a string     !!!
    /// </summary>
    /// <param name="user"></param>
    /// <param name="change"></param>
    /// <param name="device"></param>
    private void ControlSchemeChangedDebug(InputUser user, InputUserChange change, InputDevice device) {
        if (change == InputUserChange.ControlSchemeChanged) { 
            Debug.Log("Device changed to: " + user.controlScheme.Value.name);
            //Notify UI to change the sprites of the buttons on screen.
            /*  Example of if to be done in the UI Manager
            if (user.controlScheme.Value.name.Equals("Gamepad")) {
                //Change sprites to Gamepad controls
            }
            else {
                //Change sprites to Mouse&Keyboard controls
            } 
            */
        }
    }

    private void DeviceChangeDebug(InputDevice device, InputDeviceChange deviceChange) {
        Debug.Log("Device" + device);
        Debug.Log("Device Change: " + deviceChange);
    }

    private void SchemeLayoutChangeDebug(string layout, InputControlLayoutChange layoutChange) {
        Debug.Log("Layout" + layout);
        Debug.Log("Layout Change: " + layoutChange);
    }

    /// <summary>
    /// Hold Interaction: 
    /// !!! Not to be confused with GetButton() - Hold interaction is used when you want the player to hold a button for a set amount of time => Usable for a confirmation, classic radial UI bar that goes fills while holding the key. !!!
    ///     Started event => When the button gets pressed beyond the PressPoint value (usable for triggers or buttons with a 1-Axis 0 -> 1 value)
    ///     Performed event => When the button has been held for the amount of seconds specified in the HoldTime setting of the interaction.
    ///     Canceled => When the button has been released. Does not distinguish between release before or after the held was performed.
    /// </summary>
    /// <param name="actionContext"></param>
    private void ActionDebug(InputAction.CallbackContext actionContext) {
        if (actionContext.started) Debug.Log("Hold Action has started");
        if (actionContext.performed) Debug.Log("Hold Action has finished");
        if (actionContext.canceled) Debug.Log("Hold Action has been canceled");
    }

    private void MoveAction(InputAction.CallbackContext callbackContext) {
        //_playerController.MovementUpdate(callbackContext.action.ReadValue<Vector2>());
    }

    /// <summary>
    /// When you need to call a function directly from the event that has parameters that do not match the CallbackContext required by the event you can use the following:
    ///     _testAction.started += context => TestNonMatchingFunction(context.ReadValue<float>());
    ///     This will call TestNonMatchingFunction passing the value of the action at the momento of the event is called as a parameter.
    ///     
    ///     The problem with this is that you cannot unsubscribe the lamba function doing the following: _testAction.started -= context => TestNonMatchingFunction(context.ReadValue<float>());
    ///     Why? Because when subscribing it creates a anonymous method, the same happens when unsubscribing. Since both copies are different objects they do not match and the unsubscription does not work.
    ///     Workaround => Check inside the function if the object should execute the function or not. In this case you don't need to do that since the object unsubscribes when disabled which removes the scripts from executing.
    ///                   But using the bool inside the function can be used in other cases where you want to unsubscribe outside of OnDisable.
    /// </summary>
    /// <param name="value"></param>
    private void TestNonMatchingFunction(float value) {
        if (!_isObjectActive) return;
        Debug.Log("TestNonMathing value: " + value);
    }

    /// <summary>
    /// Here I check for the current value of the action once per frame. This is usefull for the movement since I want compatibility for the controller which does not return an event while being used.
    /// Could optimize to check the values between started and canceled => Problems managing both A and D buttons and would need to manage both keys started and canceled seperately. (Use Press and Release)
    /// </summary>
    private void Update() {
        _playerController.MovementUpdate(_movementAction.ReadValue<Vector2>());

        if (Input.GetKeyDown(KeyCode.P)) {
            PlayerStateManager.Instance.ChangeStateRequest(PlayerStateManager.PlayerState.PannelEditFreeze);
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            PlayerStateManager.Instance.RestoreLastStateRequest();
        }
    }
}
