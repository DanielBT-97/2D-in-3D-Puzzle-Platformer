using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausedGameState : IPlayerStateHandler
{
    public bool EnterTransitionConditions(PlayerStateManager.PlayerState currentState) {
        return true;
    }

    public void Enter() {
        PlayerStateManager.Instance.OnPausedGameEnter.Invoke();
    }

    public void Execute() {
        throw new System.NotImplementedException();
    }

    public void Exit() {
        PlayerStateManager.Instance.OnPausedGameExit.Invoke();
    }
}
