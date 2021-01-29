using System.Collections;
using UnityEngine;

public class Sheep : MonoBehaviour {
    public LayerMask obstacle;
    public AnimationCurve animP;
    public AnimationCurve animR;
    public float duration;
    public uint turns;
    
    private readonly Collider[] _colliders = new Collider[1];

    [ContextMenu("Move Up")]
    private void MoveUp() {
        Move(Vector3Int.forward);
    }
    
    [ContextMenu("Move Down")]
    private void MoveDown() {
        Move(Vector3Int.back);
    }
    
    [ContextMenu("Move Left")]
    private void MoveLeft() {
        Move(Vector3Int.left);
    }
    
    [ContextMenu("Move Right")]
    private void MoveRight() {
        Move(Vector3Int.right);
    }
    
    public bool Move(Vector3Int dir) {
        if (Physics.OverlapSphereNonAlloc(transform.position + dir, 0.4f, _colliders) > 0) return false;
        StartCoroutine(SheepAnim(dir));
        return true;
    }

    private IEnumerator SheepAnim(Vector3Int dir) {
        var t = transform;
        var axis = dir.x != 0
            ? (dir.x < 0 ? Vector3.forward : Vector3.back)
            : (dir.z > 0 ? Vector3.right : Vector3.left);

        var sP = t.position;
        var sR = t.rotation;
        
        var eP = sP + dir;
        var eR = sR * Quaternion.Euler(t.InverseTransformDirection(axis * (90 * turns)));

        var time = 0f;

        while (time < duration) {
            time += Time.deltaTime;

            t.position = Vector3.Lerp(sP, eP, animP.Evaluate(time / duration));
            t.rotation = Quaternion.Lerp(sR, eR, animR.Evaluate(time / duration));
            
            yield return null;
        }

        yield return null;
    }
}