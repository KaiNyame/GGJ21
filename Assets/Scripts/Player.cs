using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Resettable {
    public LayerMask obstacle;
    public float duration;
    public AnimationCurve walkAnim;
    public AnimationCurve pushAnim;
    public AnimationCurve animR;
    public PlayerInput input;
    public Animator animator;
    public float maxSpeed;
    
    private readonly Collider[] _colliders = new Collider[1];
    private bool _moving;
    private InputAction _move;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Pushing = Animator.StringToHash("Pushing");

    private float contactTime;
    private Vector2 start;

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

    public void Awake() {
        _move = input.actions.FindActionMap("Player").FindAction("Move");
        input.onActionTriggered += OnAction;
    }

    private void OnAction(InputAction.CallbackContext context) {
        if (!enabled) return;
        if (!context.action.name.Equals("Contact")) return;
        
        if (context.started) {
            start = context.ReadValue<Vector2>();
            contactTime = Time.unscaledTime;
        }
        else if (context.canceled && Time.unscaledTime - contactTime < 0.45f) {
            var swipe = (context.ReadValue<Vector2>() - start).normalized;
            if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y)) Move(new Vector3Int(Mathf.RoundToInt(swipe.x), 0, 0));
            else Move(new Vector3Int(0, 0, Mathf.RoundToInt(swipe.y)));
        }
    }

    public void Update() {
        var stick = _move.ReadValue<Vector2>() * 1.5f;
        stick.x = Mathf.Clamp(stick.x, -1, 1);
        stick.y = Mathf.Clamp(stick.y, -1, 1);
        
        if (Mathf.Abs(stick.x) > Mathf.Abs(stick.y)) Move(new Vector3Int(Mathf.RoundToInt(stick.x), 0, 0));
        else Move(new Vector3Int(0, 0, Mathf.RoundToInt(stick.y)));
    }
    
    public void Move(Vector3Int dir) {
        if (dir == Vector3Int.zero) return;
        if (_moving) return;
        if (Physics.OverlapSphereNonAlloc(transform.position + dir, 0.4f, _colliders, obstacle.value) > 0) {
            if (!_colliders[0].TryGetComponent<Sheep>(out var s) || !s.Move(dir)) return;
            animator.SetFloat(Pushing, 1.0f);
            StartCoroutine(MoveAnim(dir, pushAnim));
        }
        else {
            animator.SetFloat(Pushing, 0.0f);
            StartCoroutine(MoveAnim(dir, walkAnim));
        }
    }

    private IEnumerator MoveAnim(Vector3Int dir, AnimationCurve anim) {
        _moving = true;
        
        var t = transform;
        var angle = dir.x != 0
            ? (dir.x < 0 ? -90f : 90f)
            : (dir.z > 0 ? 0f : 180f);

        var sP = t.position;
        var sR = t.rotation;
        
        var eP = sP + dir;
        var eR = Quaternion.Euler(0, angle, 0);

        var time = 0f;

        while (time < duration) {
            time += Time.deltaTime;

            var pos = t.position;
            var posE = Vector3.Lerp(sP, eP, anim.Evaluate(time / duration));
            t.position = posE;
            t.rotation = Quaternion.Lerp(sR, eR, animR.Evaluate(time / duration));
            
            animator.SetFloat(Speed, Vector3.Distance(pos, posE) / Time.deltaTime / maxSpeed);
            
            yield return null;
        }

        _moving = false;
        yield return null;
    }
}