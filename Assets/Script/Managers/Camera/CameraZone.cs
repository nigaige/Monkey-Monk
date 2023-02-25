using UnityEngine;

public class CameraZone : MonoBehaviour
{
    [SerializeField] private Vector2 halfzoneSize;
    public Vector2 HalfZoneSize { get => halfzoneSize; }

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

    public void Deactivate()
    {
        Activated = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, halfzoneSize * 2f);
    }
}
