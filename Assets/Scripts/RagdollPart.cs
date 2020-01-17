using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollPart : MonoBehaviour
{
    private Rigidbody rigid = null;

    void Start()
    {
        this.rigid = this.GetComponent<Rigidbody>();
        this.rigid.isKinematic = true;
    }

    public void SetRagDoll(bool value)
    {
        this.rigid.isKinematic = value;
    }
}
