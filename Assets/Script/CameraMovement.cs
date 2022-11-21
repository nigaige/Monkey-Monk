using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private Vector2 offset;

    private Vector2 _velocity;

    private void LateUpdate()
    {
        transform.position = (Vector3)Vector2.SmoothDamp(transform.position, (Vector2)target.transform.position + offset, ref _velocity, smoothTime) + Vector3.forward * transform.position.z;
    }

}
