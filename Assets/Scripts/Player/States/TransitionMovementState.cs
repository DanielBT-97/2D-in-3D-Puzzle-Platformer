using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionMovementState : IPlayerStateHandler
{
    public bool EnterTransitionConditions(PlayerStateManager.PlayerState currentState) {
        bool canTransition = true;

        if (currentState == PlayerStateManager.PlayerState.PannelEditFreeze) {
            canTransition = false;
        }

        return canTransition;
    }

    public void Enter() {
        PlayerStateManager.Instance.OnTransitionModeEnter.Invoke();
    }

    public void Execute() {
        throw new System.NotImplementedException();
    }

    public void Exit() {
        PlayerStateManager.Instance.OnTransitionModeExit.Invoke();
    }
}
