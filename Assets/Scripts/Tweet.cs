using UnityEngine;

public class Tweet : MonoBehaviour {
    public TextAsset contentEng;
    public TextAsset contentJpn;

#if !UNITY_EDITOR && UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    static extern string TweetFromUnity(string rawMessage);
#endif

    public void Share() {
        var msg = (Dialogue.useEng ? contentEng : contentJpn).text;
        msg = msg.Replace("\n", "%0a").Replace("#", "%23");

#if !UNITY_EDITOR && UNITY_WEBGL
        TweetFromUnity(msg);
#endif
    }
}