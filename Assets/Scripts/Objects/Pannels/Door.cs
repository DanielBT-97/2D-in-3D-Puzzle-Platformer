using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private Pannel _pannel = null;

    [SerializeField]
    private Door _targetDoor = null;

    public Door TargetDoor {
        get { return _targetDoor; }
        set { _targetDoor = value; }
    }
}
