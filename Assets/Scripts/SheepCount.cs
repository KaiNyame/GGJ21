using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SheepCount : MonoBehaviour {
    public Text text;
    public float delay;

    private int _prev;
    private float _t;
    
    void Update() {
        if (_prev < LevelChecker.sheepCount) {
            _t += Time.deltaTime;
            if (_t < delay) return;
            _prev++;
            _t = 0;
        }
        text.text = _prev.ToString("D3");
    }
}
