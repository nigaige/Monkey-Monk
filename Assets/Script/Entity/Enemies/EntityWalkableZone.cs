using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWalkableZone : MonoBehaviour
{
    [SerializeField] private Vector3 _startZoneOffset = Vector3.left;
    [SerializeField] private Vector3 _endZoneOffset = Vector3.right;

    public Vector2 GetZones()
    {
        return new Vector2(transform.position.x + _startZoneOffset.x, transform.position.x + _endZoneOffset.x);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position + _startZoneOffset, 0.1f);
        Gizmos.DrawSphere(transform.position + _endZoneOffset, 0.1f);
    }
#endif

}
