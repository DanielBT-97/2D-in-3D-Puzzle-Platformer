using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple script that is used to leave comments in the inspector. To be removed when not in DebugMode
public class ToolTipInspector : MonoBehaviour
{
    [TextArea]
    public string _message = string.Empty;

    void Awake()
    {
        if(!GlobalSettings.DebugMode) {
            Debug.LogException(new System.Exception("This ToolTip component should not be here for release version"), this);
        }
    }
}
