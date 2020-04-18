using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTransitionTrigger : MonoBehaviour {
    #region Type Declaration
    #endregion

    #region Private Variables
    #endregion

    #region Getters & Setters
    #endregion

    #region Serialized Variables
    #endregion

    #region Private Functions
    #endregion

    #region API Methods
    #endregion

    #region Unity Cycle
    private void OnTriggerEnter(Collider other) {
        PlayerStateManager.Instance.ChangeStateRequest(PlayerStateManager.PlayerState.TransitionMovement);
    }
    #endregion

    #region Courrutines
    #endregion
}