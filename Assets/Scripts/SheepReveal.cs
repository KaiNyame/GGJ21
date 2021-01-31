using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepReveal : MonoBehaviour {
    public LayerMask player;
    public ParticleSystem poof;
    public GameObject sheep;
    public AudioSource baah;

    public void Awake() {
        sheep.SetActive(false);
    }

    public void OnTriggerStay(Collider other) {
        if (!gameObject.activeSelf) return;
        
        if ((player.value & (1 << other.gameObject.layer)) == 0) return;
        gameObject.SetActive(false);
        sheep.SetActive(true);
        baah.Play();
        poof.Play();
    }
}
