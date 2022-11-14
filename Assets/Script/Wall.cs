using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IActivable
{
    public void Activate()
    {
        Destroy(gameObject);
    }
}
