using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {
    public LayerMask sheep;
    public bool complete;

    private void OnTriggerEnter(Collider other) {
        if ((sheep.value & (1 << other.gameObject.layer)) != 0) {
            complete = true;
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if ((sheep.value & (1 << other.gameObject.layer)) != 0) {
            complete = false;
        }
    }
}
