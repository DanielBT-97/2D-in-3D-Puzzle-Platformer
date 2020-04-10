using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMovementState : IPlayerStateHandler
{
    public bool EnterTransitionConditions(PlayerStateManager.PlayerState currentState) {
        return true;
    }

    public void Enter() {
        PlayerStateManager.Instance.OnLadderMovementEnter.Invoke();
    }

    public void Execute() {
        throw new System.NotImplementedException();
    }

    public void Exit() {
        PlayerStateManager.Instance.OnLadderMovementExit.Invoke();
    }

}
