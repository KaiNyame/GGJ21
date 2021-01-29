using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    public LayerMask obstacle;
    public float duration;
    public AnimationCurve walkAnim;
    public AnimationCurve pushAnim;
    public AnimationCurve animR;
    public PlayerInput input;
    
    private readonly Collider[] _colliders = new Collider[1];
    private bool _moving;
    private InputAction _move;
    
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
        
    }

    public void Update() {
        var stick = _move.ReadValue<Vector2>() * 1.5f;
        stick.x = Mathf.Clamp(stick.x, -1, 1);
        stick.y = Mathf.Clamp(stick.y, -1, 1);
        
        if (Mathf.Abs(stick.x) > Mathf.Abs(stick.y)) Move(new Vector3Int(Mathf.RoundToInt(stick.x), 0, 0));
        else Move(new Vector3Int(0, 0, Mathf.RoundToInt(stick.y)));
    }
    
    private void Move(Vector3Int dir) {
        if (dir == Vector3Int.zero) return;
        if (_moving) return;
        if (Physics.OverlapSphereNonAlloc(transform.position + dir, 0.4f, _colliders, obstacle.value) > 0) {
            if (!_colliders[0].TryGetComponent<Sheep>(out var s) || !s.Move(dir)) return;
            StartCoroutine(MoveAnim(dir, pushAnim));
        }
        else StartCoroutine(MoveAnim(dir, walkAnim));
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

            t.position = Vector3.Lerp(sP, eP, anim.Evaluate(time / duration));
            t.rotation = Quaternion.Lerp(sR, eR, animR.Evaluate(time / duration));
            
            yield return null;
        }

        _moving = false;
        yield return null;
    }
}