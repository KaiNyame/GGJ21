using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Sound {
    public class BackgroundMusic : Singleton<BackgroundMusic> {
        public float muteSpeed = 7f;
        
        private readonly Queue<TrackList.Music> _queue = new Queue<TrackList.Music>();
        private readonly List<AudioSource> _sources = new List<AudioSource>();
        private int _primary;
        private bool _hasSources;
        private bool _mute;
        public static float Scalar = 0.5f;

        public static void QueueTrack(TrackList.Music track) {
            var inst = Instance;
            if (!inst._hasSources) {
                while (inst._sources.Count < 2) {
                    inst._sources.Add(inst.gameObject.AddComponent<AudioSource>());
                }
                inst._hasSources = true;
                
                inst._sources[0].volume = Scalar;
            }
            
            var source = inst._sources[inst._primary];
            if (!source.isPlaying) {
                source.clip = track.audio;
                source.loop = track.loops;
                source.Play();
            }
            else inst._queue.Enqueue(track);
            
        }

        public static void ClearQueue() {
            var inst = Instance;
            inst._queue.Clear();
            inst._sources[inst._primary].Stop();
        }

        public static void SwitchTrack(TrackList.Music track) {
            var inst = Instance;
            inst._primary = inst._primary == 0 ? 1 : 0;
            
            var source = inst._sources[inst._primary];
            source.Stop();
            source.clip = track.audio;
            source.loop = track.loops;
            source.Play();
        }

        public static void Mute() {
            Instance._mute = true;
        }

        public static void UnMute() {
            Instance._mute = false;
        }

        public void Update() {
            if (!_hasSources) return;

            var source = _sources[_primary];
            var alt = _sources[_primary == 0 ? 1 : 0];
            
            alt.volume = Mathf.Lerp(alt.volume, 0f, Time.deltaTime * muteSpeed);
            if (Mathf.Approximately(alt.volume, 0)) alt.Stop();
            
            if (_mute) {
                source.volume = Mathf.Lerp(source.volume, 0f, Time.deltaTime * muteSpeed);
                return;
            }
            
            source.volume = Mathf.Lerp(source.volume, Scalar, Time.deltaTime * muteSpeed);

            if (_queue.Count == 0) return;
            if (source.isPlaying) return;
            if (source.loop && source.clip != null) {
                source.Play();
                return;
            }

            var next = _queue.Dequeue();
            source.clip = next.audio;
            source.loop = next.loops;
            source.Play();
        }
    }
}

