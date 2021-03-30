using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThanksNote : MonoBehaviour {
    public Image img;
    
    public Sprite engImg;
    public Sprite jpnImg;
    public Renderer grass;
    public Texture2D background;

    public Text[] engText;
    public Text[] jpnText;
    private static readonly int MainTex = Shader.PropertyToID("_BaseMap");

    public void Start() {
        img.sprite = Dialogue.useEng ? engImg : jpnImg;
        
        foreach (var t in engText) {
            t.enabled = Dialogue.useEng;
        }
        
        foreach (var t in jpnText) {
            t.enabled = !Dialogue.useEng;
        }
        
        grass.material.SetTexture(MainTex, background);
    }
}
