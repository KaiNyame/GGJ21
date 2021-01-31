using System.Collections;
using UnityEngine;

public class Sheep : Resettable {
    public LayerMask obstacle;
    public AnimationCurve animP;
    public AnimationCurve animR;
    public AnimationCurve animY;
    public AudioSource boop;
    public Animator animator;
    public float duration;
    public uint turns;
    public float squashSpeed;
    public float squashFactor;
    public Transform squasher;
    public float flailTransitionSpeed;
    public float headFlailMixSpeed;
    public float basePitch;
    public int maxSteps = 1;

    private readonly Collider[] _colliders = new Collider[1];
    private float _s = -1;
    private float _cS;
    private bool _canMove = true;
    private float _seed;
    private int _step;
    
    private static readonly int Flailing = Animator.StringToHash("Flailing");
    private static readonly int Flail = Animator.StringToHash("Flail");
    private static readonly int HeadFlail = Animator.StringToHash("HeadFlail");
    private static readonly int LegFlail = Animator.StringToHash("LegFlail");

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
        if (!_canMove) return false;
        if (Physics.OverlapSphereNonAlloc(transform.position + dir, 0.4f, _colliders, obstacle.value) > 0) return false;
        StartCoroutine(SheepAnim(dir));
        return true;
    }

    private IEnumerator SheepAnim(Vector3Int dir) {
        _canMove = false;
        var high = _s > 0;
        _s = 0;
        
        var t = transform;
        var axis = dir.x != 0
            ? (dir.x < 0 ? Vector3.forward : Vector3.back)
            : (dir.z > 0 ? Vector3.right : Vector3.left);

        var sP = t.position;
        var sR = t.rotation;
        
        var eP = sP + dir;
        var r = (sR * Quaternion.Euler(t.InverseTransformDirection(axis * (90 * turns)))).eulerAngles;
        var eR = Quaternion.Euler(Mathf.Round(r.x / 90) * 90, Mathf.Round(r.y / 90) * 90, Mathf.Round(r.z / 90) * 90);

        var time = 0f;

        while (time < duration) {
            time += Time.deltaTime;

            t.position = Vector3.Lerp(sP, eP + Vector3.up * animY.Evaluate(time / duration), animP.Evaluate(time / duration));
            t.rotation = Quaternion.Lerp(sR, eR, animR.Evaluate(time / duration));

            yield return null;
        }
        
        t.position = eP;
        t.rotation = eR;

        _s = 1;
        _canMove = true;
        if (high) {
            _step = Mathf.Clamp(_step + 1, 0, maxSteps);
            boop.pitch = basePitch + (0.1f * _step);
        }
        else {
            boop.pitch = basePitch;
            _step = 1;
        }
        
        boop.Play();
        yield return null;
    }

    public void Awake() {
        _seed = Random.value;
    }

    public void LateUpdate() {
        var tS = transform.InverseTransformDirection(new Vector3(1, squashFactor, 1));
        tS = new Vector3(Mathf.Abs(tS.x), Mathf.Abs(tS.y), Mathf.Abs(tS.z));
        var tP = transform.InverseTransformDirection(Vector3.down) * ((1f - squashFactor) * 0.8f);

        if (_s > 0 && Vector3.Distance(tS, squasher.localScale) < 0.01f) {
            _s = -1;
        }

        squasher.localScale = Vector3.Slerp(
            squasher.localScale,
            _s > 0 ? tS : Vector3.one,
            Time.deltaTime * squashSpeed
        );

        squasher.localPosition = Vector3.Lerp(
            squasher.localPosition,
            _s > 0 ? tP : Vector3.zero,
            Time.deltaTime * squashSpeed
        );

        var flailing = Vector3.Angle(transform.up, Vector3.up) > 5;
        animator.SetBool(Flailing, flailing);
        var delta = Time.deltaTime * flailTransitionSpeed;
        if (flailing) {
            var bT = Mathf.Lerp(0.2f, 0.4f, Mathf.PerlinNoise(Time.time * headFlailMixSpeed, _seed));
            animator.SetFloat(Flail, Mathf.Lerp(animator.GetFloat(Flail), bT, delta));
            
            if (Vector3.Angle(transform.forward, Vector3.down) > 5) animator.SetFloat(HeadFlail, Mathf.Lerp(animator.GetFloat(HeadFlail), 1, delta));
            else animator.SetFloat(HeadFlail, Mathf.Lerp(animator.GetFloat(HeadFlail), 0, delta));
            animator.SetFloat(LegFlail, Mathf.Lerp(animator.GetFloat(LegFlail), 1, delta));
        }
        else {
            animator.SetFloat(Flail, Mathf.Lerp(animator.GetFloat(Flail), 0, delta));
            animator.SetFloat(HeadFlail, Mathf.Lerp(animator.GetFloat(HeadFlail), 0, delta));
            animator.SetFloat(LegFlail, Mathf.Lerp(animator.GetFloat(LegFlail), 0, delta));
        }
    }
}