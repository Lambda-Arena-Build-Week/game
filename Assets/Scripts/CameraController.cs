using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Vector3 rotation;
    private Player playerScript;

    private void OnEnable()
    {
        playerScript = target.gameObject.GetComponent<Player>();
    }
    void Update()
    {

        if (Mathf.Approximately(playerScript.rigid.position.y, 0.0f))
        {
            this.transform.position = this.target.position + this.offset;
            this.transform.rotation = Quaternion.Euler(this.rotation);
        }
    }
}
