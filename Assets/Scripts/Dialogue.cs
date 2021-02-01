using System;
using System.Collections;
using Sound;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public string track;
    public TrackList tracks;
    public Canvas gameplay;
    public Canvas credits;
    public bool ending;
    

    private string[] _dialogue;
    private DialogueState[] _states;
    public int current;
    private bool _process = true;

    public void Awake() {
        _dialogue = useEng ? eng : jpn;
        _states = states;
        text.font = useEng ? engFont : jpnFont;
        UpdateDialogue();
        input.onActionTriggered += OnAction;
    }

    private void OnAction(InputAction.CallbackContext context) {
        if (!context.performed || !_process) return;
        switch (context.action.name) {
            case "Next":
                current++;
                if (current >= _dialogue.Length) {
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

    public void UpdateDialogue() {
        text.text = _dialogue[current];
        if (_states[current].boyTalking) {
            boyTriangle.enabled = true;
            manTriangle.enabled = false;
        }
        else {
            boyTriangle.enabled = false;
            manTriangle.enabled = true;
        }
    }

    public void SetDialogue(string[] newEng, string[] newJpn, DialogueState[] newStates) {
        _dialogue = useEng ? newEng : newJpn;
        _states = newStates;
        current = 0;
        UpdateDialogue();
    }

    private IEnumerator ToGame() {
        fade.raycastTarget = true;
        if (!ending) BackgroundMusic.Mute();
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
        if (ending) credits.gameObject.SetActive(true);
        else {
            gameplay.gameObject.SetActive(true);
            generator.index += 1;
            yield return generator.Generator();
        }

        time = 0f;

        while (time < fadeDuration) {
            fade.color = Color.Lerp(Color.black, Color.clear, time / fadeDuration);
            
            yield return null;
            time += Time.deltaTime;
        }
        
        fade.color = Color.clear;
        gameObject.SetActive(false);
        visibility.SetActive(true);

        if (!ending) {
            BackgroundMusic.ClearQueue();
            BackgroundMusic.UnMute();
            tracks.QueueTrack(track);
        }
        else {
            SetDialogue(eng, jpn, states);
            ending = false;
            generator.index = 0;
            LevelChecker.sheepCount = 0;
        }

        _process = true;
        input.enabled = true;
        fade.raycastTarget = false;
    }
}
