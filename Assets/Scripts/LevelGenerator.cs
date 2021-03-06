using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour {
    public GetLevelData levelData;
    public LevelChecker checker;
    public TreeSpawner trees;
    public int index;
    public Player player;
    public GameObject sheep;
    public GameObject shawp;
    public GameObject wall;
    public Goal goal;

    private readonly List<UnityEngine.Object> _scene = new List<UnityEngine.Object>();

    [ContextMenu("Generate")]
    public void GenerateLevel() {
        StartCoroutine(Generator());
    }

    public IEnumerator ClearDungeon() {
        var count = 0;
        foreach (var obj in _scene) {
            if (obj is Goal g) Destroy(g.gameObject);
            else if (obj is Player p) Destroy(p.gameObject);
            else Destroy(obj);
            count++;
            if (count < 10) continue;
            count = 0;
            yield return null;
        }
        _scene.Clear();
    }
    
    public IEnumerator Generator() {
        yield return trees.ClearTrees();
        yield return trees.SpawnTrees();
        checker.goals.Clear();
        yield return ClearDungeon();
        

        var set = levelData.levelTiles[index];

        var size = new Vector2Int(set.Width, set.Height);
        var center = size / 2;

        var count = 0;
        for (var x = 0; x < set.Width; x++) {
            for (var y = 0; y < set.Height; y++) {
                var pos = new Vector3(x - center.x, transform.position.y, y - center.y);
                var objs = new List<UnityEngine.Object>();
                
                switch (set[x, y]) {
                    case GetLevelData.Tile.Wall:
                        objs.Add(wall);
                        break;
                    case GetLevelData.Tile.Player:
                        objs.Add(player);
                        break;
                    case GetLevelData.Tile.Sheep:
                        if (Random.value < Mathf.Lerp(0.005f, 0.1f, (float) LevelChecker.sheepCount / 100)) objs.Add(shawp);
                        else objs.Add(sheep);
                        break;
                    case GetLevelData.Tile.Goal:
                        objs.Add(goal);
                        break;
                    case GetLevelData.Tile.PlayerGoal:
                        objs.Add(player);
                        objs.Add(goal);
                        break;
                    case GetLevelData.Tile.SheepGoal:
                        objs.Add(sheep);
                        objs.Add(goal);
                        break;
                    case GetLevelData.Tile.Empty:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var obj in objs) {
                    var newObj = Instantiate(obj, pos, Quaternion.identity, transform);
                    _scene.Add(newObj);
                    if (newObj is Goal g) checker.goals.Add(g);
                    else if (newObj is Player p) checker.player = p;
                    count++;
                }
                
                trees.RemoveAt(pos);

                if (count >= 5) {
                    count = 0;
                    yield return null;
                }
            }
        }
    }
}
