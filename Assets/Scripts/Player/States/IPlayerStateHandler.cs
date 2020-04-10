using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerStateHandler
{
    bool EnterTransitionConditions(PlayerStateManager.PlayerState currentState);

    void Enter();

    void Execute();

    void Exit();
}
