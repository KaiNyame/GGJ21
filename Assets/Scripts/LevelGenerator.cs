using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    public GetLevelData levelData;
    public int index;
    public GameObject player;
    public GameObject sheep;
    public GameObject wall;
    public GameObject goal;

    private readonly List<GameObject> _scene = new List<GameObject>();

    [ContextMenu("Generate")]
    public void GenerateLevel() {
        foreach (var obj in _scene) {
            Destroy(obj);
        }

        var set = levelData.levelTiles[index];

        var size = new Vector2Int(set.Width, set.Height);
        var center = size / 2;
        
        for (var x = 0; x < set.Width; x++) {
            for (var y = 0; y < set.Height; y++) {
                var pos = new Vector3(x - center.x, transform.position.y, y - center.y);
                var objs = new List<GameObject>();
                
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
                
                foreach(var obj in objs) _scene.Add(Instantiate(obj, pos, Quaternion.identity));
            }
        }
    }
}
