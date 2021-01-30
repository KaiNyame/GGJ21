using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    public GetLevelData levelData;
    public LevelChecker checker;
    public int index;
    public GameObject player;
    public GameObject sheep;
    public GameObject wall;
    public Goal goal;

    private readonly List<UnityEngine.Object> _scene = new List<UnityEngine.Object>();

    [ContextMenu("Generate")]
    public void GenerateLevel() {
        checker.goals.Clear();
        foreach (var obj in _scene) {
            if (obj is Goal g) Destroy(g.gameObject);
            else Destroy(obj);
        }
        _scene.Clear();

        var set = levelData.levelTiles[index];

        var size = new Vector2Int(set.Width, set.Height);
        var center = size / 2;
        
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
                        objs.Add(sheep);
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
                    var newObj = Instantiate(obj, pos, Quaternion.identity);
                    _scene.Add(newObj);
                    if (newObj is Goal g) checker.goals.Add(g);
                }
            }
        }
    }
}
