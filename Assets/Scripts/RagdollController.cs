using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public List<RagdollPart> parts = new List<RagdollPart>();

    private void SetRagdoll(bool value)
    {
        for (int i = 0; i < parts.Count; i++)
        {
            this.parts[i].SetRagDoll(value);
        }
    }   

    public void TurnRagdollOn()
    {
        this.SetRagdoll(false);
    }

    public void TurnRagdollOff()
    {
        this.SetRagdoll(true);
    }
}