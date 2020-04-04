using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    public Rigidbody _rigid = null;
    public Vector3 _rotationSpeed = Vector3.zero;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        //Time.timeScale = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        //_rigid.AddTorque(_rotationSpeed, ForceMode.VelocityChange);
        this.transform.rotation *= Quaternion.AngleAxis(_rotationSpeed.magnitude, _rotationSpeed);
    }
}
