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
    public bool useEnglish;
    public LevelGenerator generator;

    private string[] _dialogue;
    private int _current;

    public void Awake() {
        useEng = useEnglish;
        _dialogue = useEng ? eng : jpn;
        text.font = useEng ? engFont : jpnFont;
        UpdateDialogue();
        input.onActionTriggered += OnAction;
    }

    private void OnAction(InputAction.CallbackContext context) {
        if (!context.performed) return;
        switch (context.action.name) {
            case "Next":
                _current++;
                if (_current >= _dialogue.Length) {
                    Destroy(gameObject);
                    generator.GenerateLevel();
                }
                else UpdateDialogue();
                break;
            case "End":
                Destroy(gameObject);
                generator.GenerateLevel();
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
}
