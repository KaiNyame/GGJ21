using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Resetter : MonoBehaviour {
    public GameObject container;
    public Image fade;
    public float fadeDuration;

    private bool _resetting;
    
    public void ResetEntities() {
        if (_resetting) return;
        StartCoroutine(ResetProcess());
    }

    private IEnumerator ResetProcess() {
        _resetting = true;
        var time = 0f;

        while (time < fadeDuration) {
            fade.color = Color.Lerp(Color.clear, Color.black, time / fadeDuration);
            
            yield return null;
            time += Time.deltaTime;
        }
        
        fade.color = Color.black;

        foreach (var entity in container.GetComponentsInChildren<Resettable>()) {
            entity.ResetEntity();
        }

        time = 0f;

        while (time < fadeDuration) {
            fade.color = Color.Lerp(Color.black, Color.clear, time / fadeDuration);
            
            yield return null;
            time += Time.deltaTime;
        }
        
        fade.color = Color.clear;
        _resetting = false;
    }
}