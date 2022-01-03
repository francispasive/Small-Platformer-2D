using UnityEngine;

public class FloorBehaviour : MonoBehaviour
{
    [SerializeField]
    private FloorBehaviour _topFloor = default;
    [SerializeField]
    private FloorBehaviour _bottomFloor = default;

    [Header("Sub-Component")]
    [SerializeField]
    private Collider2D _collider2D;
    public Collider2D Collider2D => _collider2D;

    public FloorBehaviour TopFloor => _topFloor;
    public FloorBehaviour BottomFloor => _bottomFloor;

    private void Reset()
    {
        _collider2D = GetComponent<Collider2D>();
    }
}
