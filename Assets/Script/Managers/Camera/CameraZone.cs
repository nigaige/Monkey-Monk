using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public Vector2 HalfZoneSize { get => halfzoneSize; }
    public bool LockUp => _lockUp;
    public bool LockRight => _lockRight;
    public bool LockDown => _lockDown;
    public bool LockLeft => _lockLeft;

    [SerializeField] private Vector2 halfzoneSize;
    [SerializeField] private bool _lockUp = true;
    [SerializeField] private bool _lockRight = true;
    [SerializeField] private bool _lockDown = true;
    [SerializeField] private bool _lockLeft = true;

    public bool Activated { get; private set; } = true;

    [Min(0)]
    [SerializeField] private float newCameraZ = 15;
    public float NewCameraZ { get => newCameraZ; }

    public bool IsPointInsideZone(Vector2 point)
    {
        return point.x >= transform.position.x - halfzoneSize.x
            && point.x <= transform.position.x + halfzoneSize.x
            && point.y >= transform.position.y - halfzoneSize.y
            && point.y <= transform.position.y + halfzoneSize.y;
    }

    public void UnlockDir(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                _lockUp = false;
                break;
            case Direction.Right:
                _lockRight = false;
                break;
            case Direction.Down:
                _lockDown = false;
                break;
            case Direction.Left:
                _lockLeft = false;
                break;
        }
    }

    public void Deactivate()
    {
        Activated = false;
    }

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 upLeft = transform.position + new Vector3(-halfzoneSize.x, halfzoneSize.y, 0);
        Vector3 upRight = transform.position + new Vector3(halfzoneSize.x, halfzoneSize.y, 0);
        Vector3 downLeft = transform.position + new Vector3(-halfzoneSize.x, -halfzoneSize.y, 0);
        Vector3 downRight = transform.position + new Vector3(halfzoneSize.x, -halfzoneSize.y, 0);

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, halfzoneSize * 2);

        Gizmos.color = Color.magenta;
        if(_lockUp) Gizmos.DrawLine(upLeft, upRight);
        if(_lockRight) Gizmos.DrawLine(downRight, upRight);
        if(_lockDown) Gizmos.DrawLine(downLeft, downRight);
        if(_lockLeft) Gizmos.DrawLine(upLeft, downLeft);
    }
#endif
}
