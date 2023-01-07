using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LianeAttach : MonoBehaviour
{
    [SerializeField] private Vector2 halfExtends;

    public Vector2[] Points { get => _points; }
    private Vector2[] _points;

    private void Awake()
    {
        _points = new Vector2[4];
        _points[0] = transform.position + transform.rotation * new Vector2(-halfExtends.x * transform.lossyScale.x, halfExtends.y * transform.lossyScale.y);
        _points[1] = transform.position + transform.rotation * new Vector2(halfExtends.x * transform.lossyScale.x, halfExtends.y * transform.lossyScale.y);
        _points[2] = transform.position + transform.rotation * new Vector2(halfExtends.x * transform.lossyScale.x, -halfExtends.y * transform.lossyScale.y);
        _points[3] = transform.position + transform.rotation * new Vector2(-halfExtends.x * transform.lossyScale.x, -halfExtends.y * transform.lossyScale.y);
    }


    private void OnDrawGizmosSelected()
    {
        DebugUtils.DrawBox(transform.position, halfExtends * transform.lossyScale, transform.rotation, Color.green, 0);
    }
}
