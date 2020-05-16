using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeMovementState : IPlayerStateHandler
{
    public bool EnterTransitionConditions(PlayerStateManager.PlayerState currentState) {
        return true;
    }

    public void Enter() {
        PlayerStateManager.Instance.OnFreeMovementEnter.Invoke();
        PlayerController.Instance.PlayerAnimator.SetBool("FreeMovement", true);
    }

    public void Execute() {
        PlayerController.Instance.FreeMovementUpdate();
    }

    public void Exit() {
        PlayerStateManager.Instance.OnFreeMovementExit.Invoke();
        PlayerController.Instance.PlayerAnimator.SetBool("FreeMovement", false);
        //PlayerController.Instance.PlayerAnimator.SetFloat("HorizontalDirection", 0);
    }
}
