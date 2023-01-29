using JetBrains.Annotations;
using MonkeyMonk.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    [SerializeField] private Transform otherPortal;
    [SerializeField] private LayerMask acceptedObjects;

    private List<GameObject> _bannedObjects = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (!_bannedObjects.Contains(other.gameObject) && (1 << other.gameObject.layer & acceptedObjects.value) > 0)
        {
            other.transform.position = otherPortal.transform.position;
            otherPortal.GetComponent<Teleportation>().Ban(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _bannedObjects.Remove(other.gameObject);
    }
    
    public void Ban(GameObject other)
    {
        _bannedObjects.Add(other.gameObject);
    }

}