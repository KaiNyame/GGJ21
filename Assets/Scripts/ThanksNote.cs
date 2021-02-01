using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThanksNote : MonoBehaviour {
    public Image img;
    
    public Sprite engImg;
    public Sprite jpnImg;

    public Text[] engText;
    public Text[] jpnText;

    public void Start() {
        img.sprite = Dialogue.useEng ? engImg : jpnImg;
        
        foreach (var t in engText) {
            t.enabled = Dialogue.useEng;
        }
        
        foreach (var t in jpnText) {
            t.enabled = !Dialogue.useEng;
        }
    }
}
