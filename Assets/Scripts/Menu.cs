using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    public Canvas[] scenes;
    public Image fade;
    public float fadeDuration;
    public GameObject visibility;

    private static bool _transitioning;

    public void Navigate(int index) {
        if (_transitioning) return;
        StartCoroutine(NextScene(scenes[index]));
    }
    
    private IEnumerator NextScene(Canvas next) {
        _transitioning = true;
        var time = 0f;

        while (time < fadeDuration) {
            fade.color = Color.Lerp(Color.clear, Color.black, time / fadeDuration);
            
            yield return null;
            time += Time.deltaTime;
        }
        
        fade.color = Color.black;

        next.gameObject.SetActive(true);
        visibility.SetActive(false);

        time = 0f;

        while (time < fadeDuration) {
            fade.color = Color.Lerp(Color.black, Color.clear, time / fadeDuration);
            
            yield return null;
            time += Time.deltaTime;
        }
        
        fade.color = Color.clear;
        _transitioning = false;
        gameObject.SetActive(false);
        visibility.SetActive(true);
    }
}
