using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollPart : MonoBehaviour
{
    public  Rigidbody rigid = null;

    public void SetRagDoll(bool value)
    {
        this.rigid.isKinematic = value;
    }
}
