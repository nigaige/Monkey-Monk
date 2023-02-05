using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private CameraZonesManager zonesManager;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 maxOffset;
    [SerializeField] private float camSmooth;

    private CameraZone _currentZone;
    private Vector3 _camVel;

    private void Awake()
    {
        SearchNewCamZone();
    }

    void LateUpdate()
    {
        if (target == null) return;

        if(_currentZone == null || !_currentZone.IsPointInsideZone(target.transform.position))
        {
            SearchNewCamZone();
        }

        Vector3 targetPosition = transform.position;
        float x_offset = target.position.x - transform.position.x;

        // Déplacement à droite
        if (x_offset > 0)
        {
            targetPosition.x += x_offset;
        }

        // Déplacement à gauche
        else if (x_offset < -maxOffset.x)
        {
            targetPosition.x += x_offset + maxOffset.x;
        }

        float y_offset = target.position.y - transform.position.y;

        // Déplacement en haut
        if (y_offset > maxOffset.y)
        {
            targetPosition.y += y_offset - maxOffset.y;
        }
        // Déplacement en bas
        else if (y_offset < -maxOffset.y)
        {
            targetPosition.y += (y_offset + maxOffset.y);
        }

        if(_currentZone != null)
        {
            // Clamp target by zone limit
            Vector2 camB = GetCameraViewBounds(Mathf.Abs(target.transform.position.z - targetPosition.z)) / 2.0f;

            targetPosition.x = Mathf.Clamp(targetPosition.x, _currentZone.transform.position.x - _currentZone.HalfZoneSize.x + camB.x, _currentZone.transform.position.x + _currentZone.HalfZoneSize.x - camB.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, _currentZone.transform.position.y - _currentZone.HalfZoneSize.y + camB.y, _currentZone.transform.position.y + _currentZone.HalfZoneSize.y - camB.y);

            targetPosition.z = -_currentZone.NewCameraZ;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _camVel, camSmooth);
    }

    private void SearchNewCamZone()
    {
        if (zonesManager == null) return;

        CameraZone newCam = zonesManager.FindCurrentZone(target);
        if (newCam != null) _currentZone = newCam;
    }

    private Vector2 GetCameraViewBounds(float z)
    {
        Vector2 p1 = GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0, 0, z));
        Vector2 p2 = GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1, 1, z));
        return p2 - p1;
    }
}
