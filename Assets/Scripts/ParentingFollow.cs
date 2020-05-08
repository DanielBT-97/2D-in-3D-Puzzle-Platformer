using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentingFollow : MonoBehaviour
{
    private Vector3 _finalPosition;   //This is the realtive position
    private Vector3 _relativeOffset = Vector3.zero;
    public Transform _chilledObject;
    public Transform _parentObject;

    public bool _followParent = false;

    private IEnumerator _followParentCorroutine = null;
    
    private void Awake() {
        _followParentCorroutine = FollowParent();
    }

    public void ActivateParenting(Transform targetPannel) {
        //TODO DANI: If targetPannel is null go look for it in the PannelManager that should have both the starting pannel as well as the current pannel of the player.
        _relativeOffset = _parentObject.InverseTransformVector(_chilledObject.position - _parentObject.position); //This gets the vector from the pannel to the player in World Space. Convert it to LocalSpace in order to be usable in any orientation.
        _followParent = true;
        StartCoroutine(_followParentCorroutine);
    }

    IEnumerator FollowParent() {
        while(_followParent) {
            _finalPosition = _parentObject.position + (_relativeOffset.x * _parentObject.right) + (_relativeOffset.y * _parentObject.up) + (_relativeOffset.z * _parentObject.forward);  //Add to the pannel's position the local offset of the pannel using the pannel's orientation.
        
            //Set the position of the slave object to this relative position
            _chilledObject.position = _finalPosition;
            yield return null;
        }

        yield return null;
    }

    public void StopParenting() {
        _followParent = false;
        StopCoroutine(_followParentCorroutine);
    }
}
