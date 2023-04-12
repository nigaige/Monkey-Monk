using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableWall : AActivable
{
    public override void Activate()
    {
        this.gameObject.SetActive(false);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Grenade")
        {
            Activate();
        }
    }

    public override void ResetActivalble()
    {
    }
}
