using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelChecker : MonoBehaviour {
    public static int sheepCount;
    public LevelGenerator generator;
    public Player player;
    public AudioSource portal;
    public AudioSource complete;
    public List<Goal> goals = new List<Goal>();
    public Image fade;
    public float fadeDuration;
    public float delay;
    public float yeetDuration;
    public float angularSpeed;
    public float speed;

    private float _t;
    private int _completed;

    public void Update() {
        if (goals.Count == 0) return;
        
        var done = true;
        foreach (var goal in goals) done = done && goal.complete;

        if (!done) return;
        
        player.enabled = false;
        foreach (var goal in goals) goal.mode = 2;

        if (_t < delay) {
            _t += Time.deltaTime;
            return;
        }

        sheepCount += goals.Count;
        _t = 0;
        goals.Clear();
        StartCoroutine(CompleteLevel());
    }

    public IEnumerator CompleteLevel() {
        portal.Play();

        var sheep = generator.GetComponentsInChildren<Sheep>();
        var angularSpeeds = new Vector3[sheep.Length];

        for (var i = 0; i < sheep.Length; i++) {
            angularSpeeds[i] = Random.onUnitSphere * angularSpeed;
        }

        var time = 0f;

        while (time < yeetDuration) {
            for (var i = 0; i < sheep.Length; i++) {
                var p = time / yeetDuration;
                sheep[i].transform.position += Vector3.up * speed * p * Time.deltaTime;
                sheep[i].transform.eulerAngles += angularSpeeds[i] * p * Time.deltaTime;
            }

            yield return null;
            time += Time.deltaTime;
        }
        
        var currentGoals = generator.GetComponentsInChildren<Goal>();

        var minDist = float.MaxValue;
        var playerDir = Vector3.forward;
        Goal closestGoal = null;

        foreach (var goal in currentGoals) {
            var d = goal.transform.position - player.transform.position;
            if (d.magnitude < minDist) {
                minDist = d.magnitude;
                playerDir = d.normalized;
                closestGoal = goal;
            }
        }

        if (Vector3.Angle(playerDir, Vector3Int.forward) < 5) {
            player.Move(Vector3Int.forward);
        }
        
        else if (Vector3.Angle(playerDir, Vector3Int.back) < 5) {
            player.Move(Vector3Int.back);
        }
        
        else if (Vector3.Angle(playerDir, Vector3Int.left) < 5) {
            player.Move(Vector3Int.left);
        }
        
        else if (Vector3.Angle(playerDir, Vector3Int.right) < 5) {
            player.Move(Vector3Int.right);
        }

        yield return new WaitForSeconds(player.duration);
        
        if (closestGoal != null) closestGoal.mode = 3;
        complete.Play();
        
        yield return new WaitForSeconds(1f);

        time = 0f;

        while (time < fadeDuration) {
            fade.color = Color.Lerp(Color.clear, Color.black, time / fadeDuration);
            
            yield return null;
            time += Time.deltaTime;
        }
        
        fade.color = Color.black;
        
        generator.index += 1;
        yield return generator.Generator();
        
        time = 0f;

        while (time < fadeDuration) {
            fade.color = Color.Lerp(Color.black, Color.clear, time / fadeDuration);
            
            yield return null;
            time += Time.deltaTime;
        }
        
        fade.color = Color.clear;
    }
}
