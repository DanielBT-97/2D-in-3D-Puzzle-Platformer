using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenMovementState : IPlayerStateHandler
{
    public bool EnterTransitionConditions(PlayerStateManager.PlayerState currentState) {
        bool canTransition = true;

        if (currentState == PlayerStateManager.PlayerState.TransitionMovement || PlayerController.Instance.IsFrozen) {
            canTransition = false;
        }

        return canTransition;
    }

    public void Enter() {
        PlayerStateManager.Instance.OnPannelEditEnter.Invoke();
    }

    public void Execute() {
        throw new System.NotImplementedException();
    }

    public void Exit() {
        PlayerStateManager.Instance.OnPannelEditExit.Invoke();
    }
}
