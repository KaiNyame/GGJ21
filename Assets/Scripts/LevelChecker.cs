using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChecker : MonoBehaviour {
    public LevelGenerator generator;
    public List<Goal> goals = new List<Goal>();
    public float delay;
    
    private float _t;

    public void Update() {
        if (goals.Count == 0) return;
        
        var done = true;
        foreach (var goal in goals) done = done && goal.complete;

        if (!done) return;

        foreach (var goal in goals) goal.mode = 2;

        if (_t < delay) {
            _t += Time.deltaTime;
            return;
        }

        _t = 0;
        generator.index += 1;
        generator.GenerateLevel();
    }
}
