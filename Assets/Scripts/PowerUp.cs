using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int id;
    public float rotationSpeed = 2.0f;
    public string model;
    public GameObject obj;

    private void Update()
    {
        this.obj.transform.Rotate(new Vector3(0.0f, 0.0f, this.rotationSpeed));
    }
}



