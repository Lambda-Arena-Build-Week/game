using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int id;
    public float rotationSpeed = 2.0f;
    public string model;

    private void Update()
    {
        this.transform.Rotate(new Vector3(0.0f, this.rotationSpeed, 0.0f));
    }
}



