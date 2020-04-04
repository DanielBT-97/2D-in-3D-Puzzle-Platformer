using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezableObject : MonoBehaviour
{
    protected bool _isFrozen = false;

    public virtual void Freeze() {
        _isFrozen = true;
    }

    public virtual void UnFreeze() {
        _isFrozen = false;
    }

    //Temporal test using input 
    protected virtual void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) && !_isFrozen) {
            Freeze();
        }

        if(Input.GetKeyDown(KeyCode.O) && _isFrozen) {
            UnFreeze();
        }
    }
}
