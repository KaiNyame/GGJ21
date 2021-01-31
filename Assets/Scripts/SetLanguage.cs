using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetLanguage : MonoBehaviour {
    public Toggle toggle;
    
    void Update() {
        Dialogue.useEng = !toggle.isOn;
    }
}
