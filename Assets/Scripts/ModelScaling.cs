using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ModelScaling : MonoBehaviour {
    public float factor = 4f;
    
    void Update() {
        transform.localScale = Vector3.one * (Screen.width * factor);
    }
}
