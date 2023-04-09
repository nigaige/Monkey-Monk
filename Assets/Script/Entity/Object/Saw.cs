using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RotateSaw());
    }


    private IEnumerator RotateSaw()
    {
        while (true)
        {
            transform.Rotate(new Vector3(0, 5f, 0));
            yield return new WaitForSeconds(0.01f);

        }
    }
}
