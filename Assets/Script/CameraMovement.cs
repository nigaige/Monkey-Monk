using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float smoothTime = 0.2f;

    private Vector2 _velocity;

    private void LateUpdate()
    {
        transform.position = (Vector3)Vector2.SmoothDamp(transform.position, target.transform.position, ref _velocity, smoothTime) + Vector3.forward * transform.position.z;
    }

}
