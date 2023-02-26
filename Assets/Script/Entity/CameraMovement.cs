using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private CameraZonesManager zonesManager;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 maxOffset;
    [SerializeField] private float camSmooth;

    private List<CameraZone> _insideZones = new();
    private Vector3 _camVel;

    private void Awake()
    {
        UpdateCamZone();
    }

    void LateUpdate()
    {
        if (target == null) return;

        UpdateCamZone();

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

        foreach (var currentZone in _insideZones)
        {
            // Clamp target by zone limit
            Vector2 camB = GetCameraViewBounds(Mathf.Abs(target.transform.position.z - targetPosition.z)) / 2.0f;

            if (currentZone.HalfZoneSize.x <= camB.x) targetPosition.x = currentZone.transform.position.x;
            else
            {
                if (currentZone.LockRight) targetPosition.x = Mathf.Min(targetPosition.x, currentZone.transform.position.x + (currentZone.HalfZoneSize.x - camB.x));
                if (currentZone.LockLeft) targetPosition.x = Mathf.Max(targetPosition.x, currentZone.transform.position.x - (currentZone.HalfZoneSize.x - camB.x));
            }

            if (currentZone.HalfZoneSize.y <= camB.y) targetPosition.y = currentZone.transform.position.y;
            else
            {
                if (currentZone.LockUp) targetPosition.y = Mathf.Min(targetPosition.y, currentZone.transform.position.y + (currentZone.HalfZoneSize.y - camB.y));
                if (currentZone.LockDown) targetPosition.y = Mathf.Max(targetPosition.y, currentZone.transform.position.y - (currentZone.HalfZoneSize.y - camB.y));
            }

            targetPosition.z = -currentZone.NewCameraZ;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _camVel, camSmooth);
    }

    private void UpdateCamZone()
    {
        if (zonesManager == null) return;

        List<CameraZone> newZones = zonesManager.FindCurrentZones(target);

        foreach (CameraZone zone in newZones)
        {
            if (!_insideZones.Contains(zone))
            {
                _insideZones.Add(zone);
            }
        }

        for (int i = _insideZones.Count - 1; i >= 0; i--)
        {
            if (!newZones.Contains(_insideZones[i]))
            {
                _insideZones.RemoveAt(i);
            }
        }
    }

    private Vector2 GetCameraViewBounds(float z)
    {
        Vector2 p1 = GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0, 0, z));
        Vector2 p2 = GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1, 1, z));
        return p2 - p1;
    }
}
