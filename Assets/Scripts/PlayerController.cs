using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlatformerCharacterBehaviour), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Sub-Behaviours")]
    [SerializeField]
    private PlatformerCharacterBehaviour _platformerCharacterBehaviour;
    [SerializeField]
    private PlayerInput _playerInput;

    private bool _dropRequest = false;
    private Coroutine _dropRoutine = default;
    private Coroutine _dropRequestRoutine = default;

    private void OnEnable()
    {
        if (_playerInput != null)
        {
            _playerInput.actions["MovementDirection"].performed += InputAction_MovementDirection;
            _playerInput.actions["Jump"].performed += InputAction_Jump;
            _playerInput.actions["Drop"].performed += InputAction_Drop;
        }
    }

    private void OnDisable()
    {
        if (_playerInput != null)
        {
            _playerInput.actions["MovementDirection"].performed -= InputAction_MovementDirection;
            _playerInput.actions["Jump"].performed -= InputAction_Jump;
            _playerInput.actions["Drop"].performed -= InputAction_Drop;
        }
    }

    private void OnDestroy()
    {
        if (_dropRoutine != null)
        {
            StopCoroutine(_dropRoutine);
        }
    }

    public void InputAction_MovementDirection(InputAction.CallbackContext context)
    {
        var direction = context.ReadValue<Vector2>();
        _platformerCharacterBehaviour.UpdateMovement(direction);
    }

    public void InputAction_Jump(InputAction.CallbackContext context)
    {
        if (_dropRequest)
        {
            return;
        }

        var collider = _platformerCharacterBehaviour.Collider2D;
        var results = new RaycastHit2D[10];
        var hitCount = collider.Cast(Vector2.down, _platformerCharacterBehaviour.GroundFilter, results, .05f, true);
        if (hitCount > 0)
        {
            foreach (var hit in results)
            {
                if (!hit) continue;

                var floorBehaviour = hit.collider.GetComponent<FloorBehaviour>();
                if (floorBehaviour == null) continue;

                if (floorBehaviour.TopFloor != null)
                {
                    _platformerCharacterBehaviour.Jump();
                    break;
                }

            }
        }
    }

    public void InputAction_Drop(InputAction.CallbackContext context)
    {
        if (_dropRequestRoutine != null)
        {
            StopCoroutine(_dropRequestRoutine);
        }
        _dropRequestRoutine = StartCoroutine(DropRequestHandler(true, .2f));

        var collider = _platformerCharacterBehaviour.Collider2D;
        var results = new RaycastHit2D[10];
        var hitCount = collider.Cast(Vector2.down, _platformerCharacterBehaviour.GroundFilter, results, .05f, true);
        if (hitCount > 0)
        {
            foreach (var hit in results)
            {
                if (!hit) continue;

                var floorBehaviour = hit.collider.GetComponent<FloorBehaviour>();
                if (floorBehaviour == null) continue;

                if (floorBehaviour.BottomFloor != null)
                {
                    Physics2D.IgnoreCollision(collider, hit.collider);
                    if (_dropRoutine != null)
                    {
                        StopCoroutine(_dropRoutine);
                    }
                    _dropRoutine = StartCoroutine(EnableCollision(hit.collider, .5f));
                    break;
                }

            }
        }

        IEnumerator EnableCollision(Collider2D otherCollider, float delay)
        {
            yield return new WaitForSeconds(delay);
            Physics2D.IgnoreCollision(collider, otherCollider, false);
        }

        IEnumerator DropRequestHandler(bool dropRequestValue, float duration)
        {
            _dropRequest = dropRequestValue;
            yield return new WaitForSeconds(duration);
            _dropRequest = !_dropRequest;
        }
    }

    private void Reset()
    {
        _platformerCharacterBehaviour = GetComponent<PlatformerCharacterBehaviour>();
        _playerInput = GetComponent<PlayerInput>();
    }

}
