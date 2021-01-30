using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontSize : MonoBehaviour {
    public float percentage;
    
    void Start() {
        GetComponent<Text>().fontSize = Mathf.RoundToInt((Screen.width + Screen.height) * 0.5f * percentage);
    }
}
