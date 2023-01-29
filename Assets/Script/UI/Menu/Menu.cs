using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public virtual void OpenMenu()
    {
        gameObject.SetActive(true);
    }

    public virtual void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}
