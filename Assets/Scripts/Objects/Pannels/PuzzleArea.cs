using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleArea : MonoBehaviour
{
    [SerializeField] [Tooltip ("The first one in this array should be the pannel the player starts on.")]
    private Pannel[] _pannelsInArea;
    private Pannel _startingPannel; //Just for safety an independent reference to the first pannel of each pannel puzzle area to be able to teleport the player to the start of each puzzle.

    public Pannel GetStartingPannel {
        get { return _startingPannel; }
    }
}