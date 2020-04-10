using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base script from which every object in game that may move during gameplay (that is not parented to a pannel) is gonna inherit from.
/// The idea is to have a script that has an event which this parent script subscribes to at the start of the game so that I don't have to manually add them. The manager script is a Singleton for ease of use.
/// For now only the player is a Freezable Object but that may change if I decide to add more objects.
/// </summary>
public class FreezableObject : MonoBehaviour
{
    protected bool _isFrozen = false;
    public bool IsFrozen {
        get => _isFrozen;
    }

    public virtual void Freeze() {
        _isFrozen = true;
    }

    public virtual void UnFreeze() {
        _isFrozen = false;
    }

    //Temporal test using input 
    protected virtual void Update()
    {
        //if(Input.GetKeyDown(KeyCode.P) && !_isFrozen) {
        //    Freeze();
        //}

        //if(Input.GetKeyDown(KeyCode.O) && _isFrozen) {
        //    UnFreeze();
        //}
    }
}
