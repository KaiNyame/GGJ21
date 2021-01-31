using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour {
    [Serializable]
    public struct DialogueState {
        public bool boyTalking;
    }
    
    public static bool useEng;

    [Multiline]
    public string[] eng;
    [Multiline]
    public string[] jpn;

    public DialogueState[] states;

    public Font jpnFont;
    public Font engFont;
    
    public PlayerInput input;
    public Text text;
    public Image boyTriangle;
    public Image manTriangle;
    public Image fade;
    public GameObject visibility;
    public float fadeDuration;
    public bool useEnglish;
    public LevelGenerator generator;

    private string[] _dialogue;
    private int _current;
    private bool _process = true;

    public void Awake() {
        _dialogue = useEng ? eng : jpn;
        text.font = useEng ? engFont : jpnFont;
        UpdateDialogue();
        input.onActionTriggered += OnAction;
    }

    private void OnAction(InputAction.CallbackContext context) {
        if (!context.performed || !_process) return;
        switch (context.action.name) {
            case "Next":
                _current++;
                if (_current >= _dialogue.Length) {
                    StartCoroutine(ToGame());
                }
                else UpdateDialogue();
                break;
            case "End":
                StartCoroutine(ToGame());
                break;
            default:
                break;
        }
    }

    private void UpdateDialogue() {
        text.text = _dialogue[_current];
        if (states[_current].boyTalking) {
            boyTriangle.enabled = true;
            manTriangle.enabled = false;
        }
        else {
            boyTriangle.enabled = false;
            manTriangle.enabled = true;
        }
    }

    private IEnumerator ToGame() {
        _process = false;
        input.enabled = false;
        var time = 0f;

        while (time < fadeDuration) {
            fade.color = Color.Lerp(Color.clear, Color.black, time / fadeDuration);
            
            yield return null;
            time += Time.deltaTime;
        }
        
        fade.color = Color.black;
        visibility.SetActive(false);
        
        generator.index += 1;
        yield return generator.Generator();
        
        time = 0f;

        while (time < fadeDuration) {
            fade.color = Color.Lerp(Color.black, Color.clear, time / fadeDuration);
            
            yield return null;
            time += Time.deltaTime;
        }
        
        fade.color = Color.clear;
        Destroy(gameObject);
    }
}
