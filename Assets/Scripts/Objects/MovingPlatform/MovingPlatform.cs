using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    IEnumerator LateFixedUpdate() {
        WaitForFixedUpdate waitInstruction = new WaitForFixedUpdate();
        while(true) {
            yield return waitInstruction;
            //Move plat logic
        }
    }
}
