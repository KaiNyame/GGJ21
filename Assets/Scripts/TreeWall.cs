using UnityEngine;

public class TreeWall : MonoBehaviour {
    public float radius;
    public Renderer render;
    public Gradient colors;
    public float minScale;
    public float maxScale;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    public void Start() {
        var t = transform;
        var p = Random.insideUnitCircle * radius;
        t.localPosition += new Vector3(p.x, 0, p.y);
        t.localEulerAngles = new Vector3(-90, Random.value * 360, 0);
        t.localScale *= Mathf.Lerp(minScale, maxScale, Random.value);
        render.material.SetColor(BaseColor, colors.Evaluate(Random.value));
    }

}