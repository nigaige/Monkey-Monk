using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingPlateform : AActivable
{
    Coroutine risingCoroutine;
    public float risingHeight;
    public float risingTime;
    public override void Activate()
    {
        risingCoroutine = StartCoroutine(Rising());
    }

    public IEnumerator Rising()
    {
        Vector3 p_startPosition = this.gameObject.transform.position;
        Vector3 p_newPosition = this.gameObject.transform.position;
        p_newPosition.y += risingHeight;
        float t = 0f;
        float deltaT = 0.01f;
        while (t < risingTime)
        {
            t += deltaT;
            Vector3 newPosition = Vector3.Lerp(p_startPosition, p_newPosition, t/risingTime);
            this.gameObject.transform.SetPositionAndRotation(newPosition,this.gameObject.transform.rotation);
            yield return new WaitForSeconds(deltaT);
        }
        risingCoroutine = null;
    }
}
