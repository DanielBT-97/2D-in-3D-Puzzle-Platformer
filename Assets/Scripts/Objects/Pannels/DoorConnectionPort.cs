using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorConnectionPort : MonoBehaviour {
    #region Type Declaration
    #endregion

    #region Private Variables
    private bool _hasConnection = false;
    #endregion

    #region Getters & Setters
    public bool HasConnection {
        get => _hasConnection;
    }
    #endregion

    #region Serialized Variables
    [SerializeField] private Door _doorController = null;

    [SerializeField] private Sprite _leftPlug = null, _rightPlug = null, _fullPlug = null;
    [SerializeField] private SpriteRenderer _plugRenderer = null;
    #endregion

    #region Private Functions
    #endregion

    #region API Methods
    public bool ConnectionRequested(Door otherDoor) {
        if (_hasConnection) return false;

        _doorController.TargetDoor = otherDoor;
        _plugRenderer.sprite = _doorController.GoesRight ? _leftPlug : _rightPlug;

        return true;
    }
    #endregion

    #region Unity Cycle
    #endregion

    #region Courrutines
    #endregion
}