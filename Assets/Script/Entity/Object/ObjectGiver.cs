using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGiver : MonoBehaviour
{

    [SerializeField] private GameObject objectPrefab;

    void Start()
    {
        Activate();
    }

    private void Activate()
    {
        GameObject handedObject = Instantiate(objectPrefab);
        FindObjectOfType<Player>().GetComponent<PlayerGrab>().GrabObject(handedObject.GetComponent<Grabbable>());
    }
}
