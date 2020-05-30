using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorConnectionLine : MonoBehaviour {
    #region Type Declaration
    #endregion

    #region Private Variables
    #endregion

    #region Getters & Setters
    public Transform TargetDoorPort {
        set { 
            _targetDoor = value;
            _targetOtherDoorPort = (_targetDoor != null);
        }
    }

    public Transform PlugTransform {
        get => _pointA;
    }
    #endregion

    #region Serialized Variables
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _pointA, _pointB;
    #endregion

    #region Private Functions
    private bool _targetOtherDoorPort = false;
    private Transform _targetDoor = null;
    #endregion

    #region API Methods
    public void ResetLine() {
        _targetDoor = null;
        _targetOtherDoorPort = false;
        HideLine();
    }

    public void HideLine() {
        Debug.Log("HIDE");
        _lineRenderer.enabled = false;
    }

    public void ShowLine() {
        Debug.Log("SHOW");
        _lineRenderer.enabled = true;
    }
    #endregion

    #region Unity Cycle
    private void Update() {
        _lineRenderer.SetPosition(0, _pointA.position);
        _lineRenderer.SetPosition(1, !_targetOtherDoorPort ? _pointB.position : _targetDoor.position);
    }
    #endregion

    #region Courrutines
    #endregion
}