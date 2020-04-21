using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMovementState : IPlayerStateHandler
{
    public bool EnterTransitionConditions(PlayerStateManager.PlayerState currentState) {
        bool canTransition = true;

        if (currentState == PlayerStateManager.PlayerState.TransitionMovement) {
            canTransition = false;
        }

        return canTransition;
    }

    public void Enter() {
        PlayerController.Instance.PlayerRigid.velocity = Vector3.zero;
        PlayerController.Instance.PlayerRigid.useGravity = false;
        PlayerStateManager.Instance.OnLadderMovementEnter.Invoke();
    }

    public void Execute() {
        PlayerController.Instance.LadderMovementUpdate();
    }

    public void Exit() {
        PlayerController.Instance.PlayerRigid.useGravity = true;
        PlayerStateManager.Instance.OnLadderMovementExit.Invoke();
    }

}
