using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liane : MonoBehaviour
{
    Vector3[] direction = new [] {
        new Vector3(1.0f,0,0),      //right
        new Vector3(0.5f,0.5f,0),   //up right
        new Vector3(0,1.0f,0),      //up
        new Vector3(-0.5f,0.5f,0), //up left
        new Vector3(-1.0f,0,0),     //left
        new Vector3(-0.5f,-0.5f,0), //down left
        new Vector3(0,-1.0f,0),     //down
        new Vector3(0.5f,-0.5f,0),  //down right
    };

    Vector3 lianeDir = new Vector3(0,0,0);  

    //liane
    [SerializeField] private LineRenderer liane;
    [SerializeField] private GameObject monkeyHand;
    [SerializeField] private LayerMask platformMask;

    [SerializeField] private float maxLength = 10;
    public float MaxLength { get => maxLength; }

    private bool lianeFixed = false;
    private Vector3 lianePos = new Vector3(0, 0, 0);

    private float _lianeLength;

    public Vector3 LianePosition { get => lianePos; }

    private Vector3? GetAimAssistPoint(Vector3 aimDir)
    {
        RaycastHit hit;
        if (Physics.Raycast(monkeyHand.transform.position, aimDir, out hit, maxLength, platformMask) && hit.collider.GetComponent<LianeAttach>() != null)
        {
            return hit.point;
        }
        else
        // Test
        {
            float lianeMaxHalfAngle = 20;

            Debug.DrawRay(monkeyHand.transform.position, aimDir * maxLength, Color.red);
            DebugUtils.DrawArc(monkeyHand.transform.position, aimDir * maxLength, lianeMaxHalfAngle, Color.red, 0);

            Vector3 origin = monkeyHand.transform.position + aimDir * maxLength / 2f;

            Collider[] c = Physics.OverlapBox(origin, new Vector3(maxLength / 2f, Mathf.Sin(lianeMaxHalfAngle * Mathf.Deg2Rad) * maxLength, 0.1f), Quaternion.Euler(0, 0, Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg));
            DebugUtils.DrawBox(origin, new Vector3(maxLength / 2f, Mathf.Sin(lianeMaxHalfAngle * Mathf.Deg2Rad) * maxLength, 0.1f), Quaternion.Euler(0, 0, Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg), Color.blue, 0);

            // Find attach
            List<LianeAttach> atts = new();
            foreach (var item in c)
            {
                if (item.TryGetComponent(out LianeAttach attach)) atts.Add(attach);
            }

            // Correct aproximation by checking the angle
            for (int i = atts.Count - 1; i >= 0; i--)
            {
                LianeAttach item = atts[i];
                bool isKeeping = false;

                foreach (var vec in item.Points)
                {
                    Vector2 dir = (vec - (Vector2)monkeyHand.transform.position).normalized;

                    if (Vector2.Angle(aimDir, dir) < lianeMaxHalfAngle) // Keep item
                    {
                        isKeeping = true;
                        break;
                    }
                }

                if (!isKeeping) atts.RemoveAt(i);
            }

            if (atts.Count == 0) return null;

            // Get min angle
            LianeAttach minAttach = null;
            Vector2 minPoint = Vector2.zero;
            float minAngle = float.MaxValue;

            foreach (var item in atts)
            {
                foreach (var vec in item.Points)
                {
                    Vector2 dir = (vec - (Vector2)monkeyHand.transform.position).normalized;

                    if (Vector2.Angle(aimDir, dir) < minAngle) // Keep item
                    {
                        minAngle = Vector2.Angle(aimDir, dir);
                        minAttach = item;
                        minPoint = vec;
                    }
                }


            }

            return minPoint;
        }
    }

    void Update()
    {
        UpdateLianePos();
    }

    public void Extend(int dir)
    {
        lianeDir = direction[dir];

        Vector3? point = GetAimAssistPoint(lianeDir);

        if (point.HasValue) Attach(null, point.Value);
    }

    private void Attach(LianeAttach attach, Vector2 attachPoint)
    {
        liane.SetPosition(1, attachPoint);
        lianePos = attachPoint;

        _lianeLength = Vector3.Distance(monkeyHand.transform.position, attachPoint);

        lianeFixed = true;
    }

    public void Release()
    {
        lianeFixed = false;
    }

    public bool isLianeFixed(){
        return lianeFixed;
    }

    public float GetLianeLength()
    {
        return _lianeLength;
    }

    public void SetLianeLength(float length)
    {
        _lianeLength = length;
    }

    public Vector3 GetLianeDir(){
        return (liane.GetPosition(1) - liane.GetPosition(0)).normalized;
    }

    public bool isLeftOfFixed(){
        return liane.GetPosition(0).x < liane.GetPosition(1).x;
    }

    void UpdateLianePos()
    {
        liane.SetPosition(0, monkeyHand.transform.position);
        if (!lianeFixed) liane.SetPosition(1, monkeyHand.transform.position);
    }
}
