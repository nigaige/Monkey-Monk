using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{

    [SerializeField] float hspeed = 1f;
    [SerializeField] float hmaxSpeed =1f;
    
    float velocity = 0;
    float acceleration = 0;


    [SerializeField] float gravity = 0.1f;
    [SerializeField] float jumpSpeed = 0.4f;

    [SerializeField] float vSpeed = 0f;
    [SerializeField] float maxFallSpeed = -1;
    private Animator anim;
    private SpriteRenderer sp;
 


    KeyCode vkLeft = KeyCode.LeftArrow;
    KeyCode vkRight = KeyCode.RightArrow;
    KeyCode vkUp = KeyCode.UpArrow;
    KeyCode vkDown = KeyCode.DownArrow;
    KeyCode vkJump = KeyCode.Space;
    KeyCode vkcheat1 = KeyCode.A;
    KeyCode vkcheat2 = KeyCode.Z;
    KeyCode vkcheat3 = KeyCode.E;



    Collider m_Collider;


    Rigidbody rb;
    int dir;
    [SerializeField] int MaxJump = 2;
    public int nbJump = 1;

    [SerializeField] public bool onGround = false ;
    public Vector3 RayCast_Dir;
    
    [SerializeField] private LayerMask platformMask;
    [SerializeField] private LayerMask platformfall;
    [SerializeField] private LayerMask platformV;



    

    void vMovment() {
        Debug.Log(nbJump);
        if (Input.GetKeyDown(vkJump) && nbJump > 0) {
            Vector3 direction = Vector3.up * jumpSpeed; 
            Vector3 velo = rb.velocity;
            velo.y = 0;
            rb.velocity = velo;
            rb.AddForceAtPosition(direction, transform.position);
            nbJump --;
        }

        //clamp la velocité de chute
        if(rb.velocity.y <= maxFallSpeed){
            Vector3 velo = rb.velocity;
            velo.y = maxFallSpeed;
            rb.velocity = velo;
        }


    }

    void hMovment(bool left, bool right) {
/*
        transform.position += new Vector3(hspeed * Time.deltaTime * dir, 0, 0);
        float hMovment = hspeed * Time.deltaTime * dir;



        if(!onGround){
            if (right == true)
        }
*/



        dir = (left ? -1 : 0) + (right ? 1 : 0);
  
        if (onGround){
            velocity = hspeed * dir;
            acceleration += velocity * Time.deltaTime;

            if (acceleration > hmaxSpeed)  {acceleration = hmaxSpeed; }
            if (acceleration < -hmaxSpeed) {acceleration = -hmaxSpeed; }
            if (dir == 0)               {acceleration = 0;      }
        }



        //déplacement selon la velocité
        transform.position += new Vector3(acceleration,0,0);

    }


    void Checkground(){
        m_Collider = GetComponent<Collider>();
        RaycastHit Hit1;
        RaycastHit Hit2;

        Vector3 RayStart1;
        Vector3 RayStart2;

        float rayDist = 0.05f;


        float rayDir = (vSpeed > 0) ? 1 : -1;


        RayStart1 = new Vector3(
            transform.position.x - m_Collider.bounds.extents.x,
            transform.position.y + (m_Collider.bounds.extents.y - rayDist) * rayDir,
            transform.position.z
            );
        RayStart2 = new Vector3(
            transform.position.x + m_Collider.bounds.extents.x,
            transform.position.y + (m_Collider.bounds.extents.y - rayDist) * rayDir,
            transform.position.z
            );

        
        float raySize = (0.1f);

        RayCast_Dir = Vector3.down;
        if(rayDir >0)
            {
                RayCast_Dir=Vector3.up;
                raySize = (0.2f);
            }
        Debug.DrawRay(RayStart1, RayCast_Dir * raySize);
        Debug.DrawRay(RayStart2, RayCast_Dir * raySize);
        bool hitPlatformMask1 = Physics.Raycast(RayStart1, transform.TransformDirection( RayCast_Dir), out Hit1, raySize, platformMask);  
        bool hitPlatformMask2 = Physics.Raycast(RayStart2, transform.TransformDirection( RayCast_Dir), out Hit2, raySize, platformMask);
        float dist = Hit1.distance + Hit2.distance / 2;

       // Debug.Log(dist);

        if (hitPlatformMask1 || hitPlatformMask2) {
            onGround = true;
            nbJump = MaxJump;
        }else {
            onGround = false;
        }
    }

    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update(){
        hMovment(Input.GetKey(vkLeft), Input.GetKey(vkRight));
        Checkground ();
        vMovment();
    }
}
