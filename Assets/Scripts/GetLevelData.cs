using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

[CreateAssetMenu(menuName = "Level Text File")]
public class GetLevelData: ScriptableObject {
    private static readonly Regex levelPattern = new Regex(@"^[#@.*+$\s]{3,}(?: ?)$", RegexOptions.Multiline);
    private static readonly Regex newLines = new Regex(@"\r?\n", RegexOptions.Multiline);
    public TextAsset[] levels;
    public int levelCap;
    
    [Serializable]
    public class TileRow {
        [SerializeField]
        private List<Tile> _arr = new List<Tile>();

        public TileRow() {}
        public TileRow(int width) {
            _arr = new List<Tile>(new Tile[width]);
        }

        public Tile this[int i] {
            get => _arr[i];
            set => _arr[i] = value;
        }

        public int Width => _arr.Count;
    }
    [Serializable]
    public class TileSet {
        [SerializeField]
        private List<TileRow> _arr = new List<TileRow>();
        //empty constructer what
        public TileSet(){}
        public TileSet(int width, int height) {
            _arr = new List<TileRow>(height);
            for (var y = 0; y < height; y++) {
                _arr.Add(new TileRow(width));
            }
        }

        public TileRow this[int i] {
            get => _arr[i];
            set => _arr[i] = value;
        }

        public Tile this[int x, int y] {
            get => _arr[y][x];
            set => _arr[y][x] = value;
        }

        public int Width => _arr[0].Width;
        
        public int Height => _arr.Count;
    }
    
    public enum Tile {
        Empty, Wall, Player, Sheep, Goal, PlayerGoal, SheepGoal
    }

    public List<TileSet> levelTiles;
    [ContextMenu("process files")]
    public void ProcessFiles() {
        levelTiles.Clear();
        foreach (var level in levels) {
            var obtained = 0;
            foreach (var match in levelPattern.Matches(level.text)) {
                if (obtained >= levelCap) {
                    break;
                }
                var levelString = ((Match) match).Value;
                var rows = newLines.Split(levelString);
                var tileSet = new TileSet(rows[0].Length, rows.Length);
                for (var y = 0; y < rows.Length; y++) {
                    var row = rows[y];
                    for (var x = 0; x < row.Length; x++) {
                        switch (row[x]) {
                            case ' ':
                                tileSet[x, y] = Tile.Empty;
                                break;
                            case '.':
                                tileSet[x, y] = Tile.Goal;
                                break;
                            case '*':
                                tileSet[x, y] = Tile.SheepGoal;
                                break;
                            case '$':
                                tileSet[x, y] = Tile.Sheep;
                                break;
                            case '@':
                                tileSet[x, y] = Tile.Player;
                                break;
                            case '+':
                                tileSet[x, y] = Tile.PlayerGoal;
                                break;
                            case '#':
                                tileSet[x, y] = Tile.Wall;
                                break;
                            default:
                                break;
                        }
                    }
                }
                levelTiles.Add(tileSet);
                obtained++;
            }
        }

        var interlaced = new List<TileSet>(levelTiles);
        for (var i = 0; i < levelCap; i++) {
            for (var j = 0; j < levels.Length; j++) {
                interlaced[i * levels.Length + j] = levelTiles[j * levelCap + i];
            }
        }

        levelTiles = interlaced;
    }
}