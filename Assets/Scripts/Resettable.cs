using System;
using UnityEngine;

public class Resettable : MonoBehaviour {
    public Vector3 oP;
    public Quaternion oR;
    
    public void Start() {
        oP = transform.position;
        oR = transform.rotation;
    }

    public void ResetEntity() {
        transform.position = oP;
        transform.rotation = oR;
    }
}