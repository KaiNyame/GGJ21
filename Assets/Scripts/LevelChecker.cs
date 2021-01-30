using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChecker : MonoBehaviour {
    public List<Goal> goals = new List<Goal>();

    public void Update() {
        if (goals.Count == 0) return;
        
        var done = true;
        foreach (var goal in goals) done = done && goal.complete;

        if (!done) return;
        
        foreach (var goal in goals) goal.mode = 2;
        enabled = false;
    }
}
