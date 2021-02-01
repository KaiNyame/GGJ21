using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FontSize : MonoBehaviour {
    public float percentage;
    public Text text;

    void Update() {
        var target = Mathf.RoundToInt((Screen.width + Screen.height) * 0.5f * percentage);
        if (text != null && text.fontSize != target) text.fontSize = target;
    }
}
