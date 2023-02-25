using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Clipper2Lib;

public class Liane : MonoBehaviour
{
    public Vector3 LianePosition { get => _lianePos; }
    public float MaxLength { get => maxLength; }

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

    //liane
    [SerializeField] private LineRenderer liane;
    [SerializeField] private GameObject monkeyHand;
    [SerializeField] private LayerMask platformMask;

    [SerializeField] private float maxLength = 10;

    private bool _lianeFixed = false;

    private Vector3 _lianePos = new Vector3(0, 0, 0);
    private float _lianeLength;

    private LianeAttach _currentAttach;
    private Vector2 _attachOffset;


    private Vector3? GetAimAssistPoint(Vector3 aimDir)
    {
        RaycastHit hit;
        if (Physics.Raycast(monkeyHand.transform.position, aimDir, out hit, maxLength, platformMask) && hit.collider.GetComponent<LianeAttach>() != null)
        {
            return hit.point;
        }
        else
        {
            // === Find attachs with appriximation
            float lianeMaxHalfAngle = 20;

            //Debug.DrawRay(monkeyHand.transform.position, aimDir * maxLength, Color.red, 5);
            //DebugUtils.DrawArc(monkeyHand.transform.position, aimDir * maxLength, lianeMaxHalfAngle, Color.red, 5);

            Vector3 origin = monkeyHand.transform.position + aimDir * maxLength / 2f;

            Collider[] c = Physics.OverlapBox(origin, new Vector3(maxLength / 2f, Mathf.Sin(lianeMaxHalfAngle * Mathf.Deg2Rad) * maxLength, 0.1f), Quaternion.Euler(0, 0, Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg));
            //DebugUtils.DrawBox(origin, new Vector3(maxLength / 2f, Mathf.Sin(lianeMaxHalfAngle * Mathf.Deg2Rad) * maxLength, 0.1f), Quaternion.Euler(0, 0, Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg), Color.blue, 5);

            // Find attachs
            List<LianeAttach> atts = new();
            foreach (var item in c)
            {
                if (item.TryGetComponent(out LianeAttach attach)) atts.Add(attach);
            }

            // === Correct approximation by checking the angle
            
            // Create autolock zone path
            float dirAngleRad = Mathf.Atan2(aimDir.y, aimDir.x);

            PathD path = new PathD();
            const float angleOffsetRad = Mathf.PI / 32f;

            float currAngleRad = dirAngleRad - lianeMaxHalfAngle * Mathf.Deg2Rad;
            float targetAngleRad = dirAngleRad + lianeMaxHalfAngle * Mathf.Deg2Rad;
            float nextAngleRad = currAngleRad + angleOffsetRad;

            while (nextAngleRad <= targetAngleRad)
            {
                Vector3 currPos = monkeyHand.transform.position + new Vector3(Mathf.Cos(currAngleRad), Mathf.Sin(currAngleRad), 0) * maxLength;

                path.Add(new PointD(currPos.x, currPos.y));

                currAngleRad = nextAngleRad;
                nextAngleRad += angleOffsetRad;
            }
            Vector3 lastPos = monkeyHand.transform.position + new Vector3(Mathf.Cos(targetAngleRad), Mathf.Sin(targetAngleRad), 0) * maxLength;
            path.Add(new PointD(lastPos.x, lastPos.y));
            path.Add(new PointD(monkeyHand.transform.position.x, monkeyHand.transform.position.y));

            /*
            // Debug autolock zone
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(new Vector3((float)path[i].x, (float)path[i].y, 0), new Vector3((float)path[i + 1].x, (float)path[i + 1].y, 0), Color.red, 5);
            }
            Debug.DrawLine(new Vector3((float)path[0].x, (float)path[0].y, 0), new Vector3((float)path[path.Count - 1].x, (float)path[path.Count - 1].y, 0), Color.red, 5);
            */

            PathsD subject = new PathsD { path }; // autolock zone path
            PathsD clip = new PathsD(); // attachs path 
            foreach (var item in atts)
            {
                clip.Add(item.ClipperPath);
            }

            // Find intersection
            PathsD ress = Clipper.Intersect(subject, clip, FillRule.NonZero);

            /*
            // Debug intersection
            foreach (var res in ress)
            {
                for (int i = 0; i < res.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3((float)res[i].x, (float)res[i].y, 0), new Vector3((float)res[i + 1].x, (float)res[i + 1].y, 0), Color.blue, 10);
                }
                Debug.DrawLine(new Vector3((float)res[0].x, (float)res[0].y, 0), new Vector3((float)res[^1].x, (float)res[^1].y, 0), Color.blue, 10);
            }
            */

            // Find closest angle point
            float minAngle = float.MaxValue;
            Vector2? minPoint = null;
            foreach (var res in ress)
            {
                for (int i = 0; i < res.Count; i++)
                {
                    Vector2 point = new Vector2((float)res[i].x, (float)res[i].y);
                    Vector2 dir = (point - (Vector2)monkeyHand.transform.position).normalized;

                    if (Vector2.Angle(aimDir, dir) < minAngle) // Keep item
                    {
                        minAngle = Vector2.Angle(aimDir, dir);
                        minPoint = point;
                    }
                }
            }


            if (minPoint.HasValue) 
                return minPoint;
            else
                return null;
        }
    }

    void Update()
    {
        UpdateLianePos();
    }

    public void Extend(int dir)
    {
        Vector3 lianeDir = direction[dir];

        Vector3? point = GetAimAssistPoint(lianeDir);

        if (point.HasValue)
        {
            // Recover liane attachable
            LianeAttach attach = null;

            Collider[] colls = Physics.OverlapSphere(point.Value, 0.1f);
            foreach (var coll in colls)
            {
                if (coll.TryGetComponent(out LianeAttach lianeAttach))
                {
                    attach = lianeAttach;
                    break;
                }
            }

            Attach(attach, point.Value);
        }
    }

    private void Attach(LianeAttach attach, Vector2 attachPoint)
    {
        if (!attach) return;
        liane.SetPosition(1, attachPoint);
        _lianePos = attachPoint;

        _lianeLength = Vector3.Distance(monkeyHand.transform.position, attachPoint);

        _lianeFixed = true;

        _currentAttach = attach;
        _attachOffset = (Vector2)attach.transform.position - attachPoint;
        _currentAttach.OnAttach();
    }

    public void Release()
    {
        _lianeFixed = false;
        _currentAttach.OnDetach();
        _currentAttach = null;
    }

    public bool isLianeFixed(){
        return _lianeFixed;
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
        if (!_lianeFixed) liane.SetPosition(1, monkeyHand.transform.position);
        else
        {
            _lianePos = _currentAttach.transform.position + (Vector3)_attachOffset;
            liane.SetPosition(1, _currentAttach.transform.position + (Vector3)_attachOffset);
        }
    }
}
