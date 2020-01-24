using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class Health : MonoBehaviour
{

    public static Health instance = null;
    public int health = 100;
    
    public TextMeshProUGUI textMesh;

    void OnEnable()
    {
        if (!instance)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        this.textMesh.text = this.health.ToString();

        if (this.health > 25)
            this.textMesh.color = new Color(0.0f, 1.0f, 0.0f);
        else
            this.textMesh.color = new Color(1.0f, 0.0f, 0.0f);
    }
}
