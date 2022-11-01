using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPlateformManager : MonoBehaviour
{
    private string HUB_NAME = "HubScene";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Change the end totem visual
        var totemRanderer = gameObject.transform.GetChild(2).gameObject.GetComponent<Renderer>();
        totemRanderer.material.SetColor("_Color", Color.green);
        // End level
        StartCoroutine(EndOfLevelCoroutine());

    }

    private IEnumerator EndOfLevelCoroutine()
    {
        //Disable player inputs + display end screen

        // Wait 3 seconds
        yield return new WaitForSeconds(3);

        // Return to lobby
        SceneManager.LoadScene(HUB_NAME, LoadSceneMode.Single);
    }
}
