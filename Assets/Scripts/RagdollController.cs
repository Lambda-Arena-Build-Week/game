using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private List<RagdollPart> parts;

    void Start()
    {
        this.parts = new List<RagdollPart>(this.GetComponentsInChildren<RagdollPart>());
    }

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