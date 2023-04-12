using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : AActivable
{
    Coroutine fallingCoroutine;
    public override void Activate()
    {
        fallingCoroutine = StartCoroutine(Falling());
    }

    public IEnumerator Falling()
    {
        Transform wallTransform = this.gameObject.transform;
        while (360f - wallTransform.localEulerAngles.z < 90f || wallTransform.localEulerAngles.z == 0)
        {
            Debug.Log(wallTransform.localEulerAngles.z);
            wallTransform.Rotate(new Vector3(0,0,-1f));
            yield return new WaitForSeconds(0.01f);
        }
        fallingCoroutine = null;
    }

    public override void ResetActivalble()
    {
    }
}
