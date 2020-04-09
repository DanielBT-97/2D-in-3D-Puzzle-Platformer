using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PannelManager : MonoBehaviour
{
    //Singleton
    private static PannelManager _instance = null;
    public static PannelManager Instance {
        get { return _instance; }
    }

    #region Private Variables
    [SerializeField]
    private Pannel _currentPlayerPannel = null;
    #endregion

    #region Getters & Setters
    public Pannel CurrentPannel {
        get { return _currentPlayerPannel; }
        set { _currentPlayerPannel = value; }
    }
    #endregion

    #region Functions
    #endregion

    #region API Methods
    #endregion

    #region Unity Cycle
    private void Awake() {
        _instance = this;
    }
    #endregion

}
