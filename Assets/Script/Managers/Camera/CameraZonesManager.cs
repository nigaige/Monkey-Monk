using Clipper2Lib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZonesManager : MonoBehaviour
{
    [SerializeField] private CameraZone[] zones;

    private void Awake()
    {
        
    }

    public CameraZone FindCurrentZone(Transform target)
    {
        foreach (var zone in zones)
        {
            if (zone.IsPointInsideZone(target.transform.position)) return zone;
        }

        return null;
    }

    public List<CameraZone> FindCurrentZones(Transform target)
    {
        List<CameraZone> insideZones = new();
        foreach (var zone in zones)
        {
            if (!zone.Activated) continue;
            if (zone.IsPointInsideZone(target.transform.position)) insideZones.Add(zone);
        }

        return insideZones;
    }

}
