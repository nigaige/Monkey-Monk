using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonkeyMonk.Player;

public class ObjectGiver : InputActivable
{
    [SerializeField] private GameObject objectPrefab;

    public override void Activate()
    {
        GameObject handedObject = Instantiate(objectPrefab);
        FindObjectOfType<Player>().GetComponent<PlayerGrab>().GrabObject(handedObject.GetComponent<Grabbable>());
    }
}
