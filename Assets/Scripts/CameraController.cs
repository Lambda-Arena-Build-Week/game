using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Vector3 rotation;

    void Update()
    {
        this.transform.position = this.target.position + this.offset;
        this.transform.rotation = Quaternion.Euler(this.rotation);
    }
}
