using MonkeyMonk.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class MovingPlateform : AActivable
{
    Coroutine MovingCoroutine;
    [SerializeField] private SplineContainer splineContainer;
    public float MovingSpeed;


    public void Start()
    {
       // MovingCoroutine = MoveToNextNode();
    }

    public override void Activate()
    {
       if (MovingCoroutine == null) MovingCoroutine= StartCoroutine(MoveToNextNode());
    }


    private IEnumerator MoveToNextNode(bool splineDir = false)
    {
        float lerpVal = 0.0f;
        float lerpSpeed = MovingSpeed / splineContainer.Spline.GetLength();
        Vector3 reajustToPlateform = Vector3.left * transform.lossyScale.x / 2f + Vector3.up * transform.lossyScale.y / 2f;
        while (lerpVal < 1.0f)
        {
            transform.position = splineContainer.transform.position + (Vector3)splineContainer.Spline.EvaluatePosition((splineDir) ? 1 - lerpVal : lerpVal);
            lerpVal += lerpSpeed * Time.deltaTime;
            yield return null;
        }

        transform.position = splineContainer.transform.position + (Vector3)splineContainer.Spline.EvaluatePosition((splineDir) ? 0f : 1f);
        MovingCoroutine = null;
        if (splineContainer.Spline.Closed) MovingCoroutine = StartCoroutine(MoveToNextNode());
    }

    public override void ResetActivalble()
    {
        if (MovingCoroutine != null) StopCoroutine(MovingCoroutine);
        MovingCoroutine = null;
        transform.position = splineContainer.transform.position+ (Vector3)splineContainer.Spline.EvaluatePosition(0);
    }
}
