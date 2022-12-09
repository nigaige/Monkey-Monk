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
    private bool lianeFixed = false;
    private Vector3 lianePos = new Vector3(0,0,0);

    [SerializeField] private GameObject monkeyHand;
    [SerializeField] private LayerMask platformMask;

    void Update()
    {
        attachToMonkeyHand();
    }

    public void Extend(int dir)
    {
        lianeDir = direction[dir];

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(lianeDir), out hit, float.MaxValue, platformMask))
        {
            liane.SetPosition(1, hit.point);
            lianePos = hit.point;

            lianeFixed = true;
        }
    }

    public void Release()
    {
        lianeFixed = false;
    }

    public bool isLianeFixed(){
        return lianeFixed;
    }

    public Vector3 GetLianeDir(){
        return (liane.GetPosition(1) - liane.GetPosition(0)).normalized;
    }

    public bool isLeftOfFixed(){
        return liane.GetPosition(0).x < liane.GetPosition(1).x;
    }



    float CastARay(Vector3 pos,Vector3 dir,float length, LayerMask mask){
        RaycastHit Hit;

        Debug.DrawRay(pos, dir * length);
        bool hitPlateform = Physics.Raycast(pos, transform.TransformDirection( dir), out Hit, length, mask);  
        
        if(hitPlateform) return Hit.distance;
        return -1;
    }

    void attachToMonkeyHand(){
        liane.SetPosition(0, monkeyHand.transform.position);
        if (!lianeFixed) liane.SetPosition(1, monkeyHand.transform.position);
    }



    
}
