using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionMovementState : IPlayerStateHandler
{
    public bool EnterTransitionConditions(PlayerStateManager.PlayerState currentState) {
        bool canTransition = true;

        if (currentState == PlayerStateManager.PlayerState.PannelEditFreeze || currentState == PlayerStateManager.PlayerState.TransitionMovement) {
            canTransition = false;
        }

        return canTransition;
    }

    public void Enter() {
        PlayerController.Instance.PlayerRigid.velocity = Vector3.zero;
        PlayerController.Instance.PlayerRigid.useGravity = false;
        PlayerStateManager.Instance.OnTransitionModeEnter.Invoke();
    }

    public void Execute() {
        PlayerController.Instance.TransitionMovementUpdate();
    }

    public void Exit() {
        PlayerController.Instance.PlayerRigid.useGravity = true;
        PlayerStateManager.Instance.OnTransitionModeExit.Invoke();
    }
}
