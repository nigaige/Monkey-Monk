using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPlateformManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Change the end totem visual
        var totemRanderer = gameObject.transform.GetChild(2).gameObject.GetComponent<Renderer>();
        totemRanderer.material.SetColor("_Color", Color.green);

        SceneMaster.Instance.LoadHubWorld();
    }
}
