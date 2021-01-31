using System.Collections;
using System.Collections.Generic;
using Sound;
using UnityEngine;

public class PlayTrack : MonoBehaviour {
    public string track;
    public TrackList tracks;

    public void Awake() {
        tracks.QueueTrack(track);
    }
}
