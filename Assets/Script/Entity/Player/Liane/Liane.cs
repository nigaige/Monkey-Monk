using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liane : MonoBehaviour{



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
    [SerializeField] private float lianeSpeed;
    private bool lianeFixed = false;
    private Vector3 lianePos = new Vector3(0,0,0);



    [SerializeField] private LayerMask platformMask;




    [SerializeField] private GameObject monkeyHand;


    private Vector3 monkeyPoint;
    private Vector3 collidePoint;
    private bool isExtending = false;

    public Vector3 getLianeCollidePoint(){
        return collidePoint;
    }


    public void startExtend(int dir){
        isExtending = true;
        lianeDir = direction[dir];
    }

    public void stopExtend(){
        isExtending = false;
    }

    public bool getIsExtending(){
        return isExtending;
    }

    public void fixLianePos(){
        lianeFixed = true;
    }
    public void unfixLianePos(){
        lianeFixed = false;
    }
    public bool isLianeFixed(){
        return lianeFixed;
    }

    public void resetLiane(){
        stopExtend();
        unfixLianePos();
    }

    public Vector3 getLianeDir(){
        return (liane.GetPosition(0) - liane.GetPosition(1)).normalized;
    }

    public bool isLeftOfFixed(){
        return liane.GetPosition(0).x<liane.GetPosition(1).x;
    }



    float CastARay(Vector3 pos,Vector3 dir,float length, LayerMask mask){
        RaycastHit Hit;

        Debug.DrawRay(pos, dir * length);
        bool hitPlateform = Physics.Raycast(pos, transform.TransformDirection( dir), out Hit, length, mask);  
        
        if(hitPlateform) return Hit.distance;
        return -1;
    }

    void extendLiane(){
        
        if (isExtending){
            if (CastARay(liane.GetPosition(1), transform.TransformDirection(lianeDir),lianeSpeed*Time.deltaTime, platformMask) >= 0){

                lianePos = liane.GetPosition(1);

                fixLianePos();
                stopExtend();

            }else if(!lianeFixed){
                liane.SetPosition(1, liane.GetPosition(1) + lianeDir * lianeSpeed * Time.deltaTime);
            }

        }
    }

    void attachToMonkeyHand(){
        liane.SetPosition(0,monkeyHand.transform.position);
        if (!lianeFixed && !isExtending)liane.SetPosition(1,monkeyHand.transform.position);
    }



    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        attachToMonkeyHand();
        extendLiane();
    }
}
