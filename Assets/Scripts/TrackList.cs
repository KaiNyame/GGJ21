using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sound {
    [CreateAssetMenu(fileName = "Track List", menuName = "Track List")]
    public class TrackList : ScriptableObject {
        [Serializable]
        public struct Music {
            public string name;
            public AudioClip audio;
            public bool loops;
        }

        public List<Music> tracks;

        public void QueueTrack(string trackName) {
            foreach (var track in tracks) {
                if (!track.name.Equals(trackName)) continue;
                
                BackgroundMusic.QueueTrack(track);
                return;
            }
            
            Debug.LogError($"Unable to find \"{trackName}\" in track list");
        }

        public void SwitchTrack(string trackName) {
            foreach (var track in tracks) {
                if (!track.name.Equals(trackName)) continue;
                
                BackgroundMusic.SwitchTrack(track);
                return;
            }
            
            Debug.LogError($"Unable to find \"{trackName}\" in track list");
        }
    }
}
