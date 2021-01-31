using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour {
    public class Tree {
        public Vector3 info;
        public Renderer tree;
    }
    
    public Renderer tree;
    public Gradient colors;
    public float minSize;
    public float maxSize;
    public float scale;
    public float spawnRadius;
    public int spawnPerFrame;
    
    private readonly Dictionary<Vector2Int, List<Tree>> _trees = new Dictionary<Vector2Int, List<Tree>>();
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    public IEnumerator ClearTrees() {
        var count = 0;
        foreach (var kvp in _trees) {
            foreach (var t in kvp.Value) {
                Destroy(t.tree.gameObject);
                count++;

                if (count < spawnPerFrame * 2) continue;
                yield return null;
                count = 0;
            }
        }
        _trees.Clear();
    }
    
    public void RemoveAt(Vector3 location) {
        var key = new Vector2Int(
            Mathf.FloorToInt(Mathf.FloorToInt(location.x / maxSize) * maxSize),
            Mathf.FloorToInt(Mathf.FloorToInt(location.z / maxSize) * maxSize)
        );
        var loc = new Vector2(location.x, location.z);

        for (var x = -1; x < 2; x++) {
            for (var y = -1; y < 2; y++) {
                if (!_trees.TryGetValue(key + new Vector2Int(x, y), out var r)) continue;

                for (var i = r.Count - 1; i >= 0; i--) {
                    var point = r[i].info;
                    var dist = Vector2.Distance(point, loc);
                    if (dist > point.z + maxSize) continue;
                    Destroy(r[i].tree.gameObject);
                    r.RemoveAt(i);
                }
            }
        }
    }

    private void SpawnTree(List<Tree> cache, Vector2 location, float max) {
        var radius = Random.Range(minSize, max);
        var point = new Vector3(location.x, location.y, radius);

        var t = Instantiate(
            tree,
            new Vector3(location.x, transform.position.y, location.y),
            Quaternion.Euler(-90, Random.value * 360, 0),
            transform
        );
        t.material.SetColor(BaseColor, colors.Evaluate(Random.value));
        t.transform.localScale *= radius * scale;

        cache.Add(new Tree() {
            info = point,
            tree = t,
        });
    }

    public IEnumerator SpawnTrees() {
        var count = 0;
        while (Application.isPlaying) {
            var spawned = false;
            
            for (var i = 0; i < 15; i++) {
                var location = Random.insideUnitCircle * spawnRadius;
                var key = new Vector2Int(
                    Mathf.FloorToInt(Mathf.FloorToInt(location.x / maxSize) * maxSize),
                    Mathf.FloorToInt(Mathf.FloorToInt(location.y / maxSize) * maxSize)
                );

                var canSpawn = true;
                var max = maxSize;
                for (var x = -1; x < 2; x++) {
                    for (var y = -1; y < 2; y++) {
                        if (!_trees.TryGetValue(key + new Vector2Int(x, y), out var r)) continue;

                        foreach (var tree in r) {
                            var point = tree.info;
                            var dist = Vector2.Distance(point, location);
                            if (dist < point.z + minSize) {
                                canSpawn = false;
                                break;
                            }

                            max = Mathf.Min(max, dist - point.z);
                        }

                        if (!canSpawn) break;
                    }
                    
                    if (!canSpawn) break;
                }

                if (!canSpawn) continue;
                
                if (!_trees.TryGetValue(key, out var cache)) {
                    cache = new List<Tree>();
                    _trees.Add(key, cache);
                }
                    
                SpawnTree(cache, location, max);
                spawned = true;
                break;
            }

            if (spawned) {
                count++;
                if (count < spawnPerFrame) continue;
                count = 0;
                yield return null;
            }
            
            else break;
        }

        Debug.Log("Done");
        yield return null;
    }
}
