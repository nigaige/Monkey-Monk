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
    KeyCode vkLiane = KeyCode.A; //touche Q TODO gestion des clavier
    KeyCode vkcheat2 = KeyCode.Z;
    KeyCode vkcheat3 = KeyCode.E;



    Collider m_Collider;


    Rigidbody rb;
    private int dir;
    private int lastDir = 1;

    [SerializeField] int MaxJump = 2;
    public int nbJump = 1;

    [SerializeField] public bool onGround = false ;
    public Vector3 RayCast_Dir;
    
    [SerializeField] private LayerMask platformMask;

    //liane
    [SerializeField] private Liane liane;
    [SerializeField] private float lianeSpeed;
    private float lianeAcceleration = 100000;




    

    void vMovment() {
        //Debug.Log(nbJump);
        if (Input.GetKeyDown(vkJump) && nbJump > 0) {
            Vector3 direction = Vector3.up * jumpSpeed; 
            Vector3 velo = rb.velocity;
            velo.y = 0;
            rb.velocity = velo;
            rb.AddForceAtPosition(direction, transform.position);
            nbJump --;
            GetComponent<AudioSource>().Play();
        }

        //clamp la velocité de chute
        if(rb.velocity.y <= maxFallSpeed){
            Vector3 velo = rb.velocity;
            velo.y = maxFallSpeed;
            rb.velocity = velo;
        }


    }

    void hMovment(bool left, bool right) {
        dir = (left ? -1 : 0) + (right ? 1 : 0);
        if (dir !=0 && dir != lastDir){
            lastDir = dir;
        }

        if (onGround){
            velocity = hspeed * dir;
            acceleration += velocity * Time.deltaTime;

            if (acceleration > hmaxSpeed)  {acceleration = hmaxSpeed; }
            if (acceleration < -hmaxSpeed) {acceleration = -hmaxSpeed; }
            if (dir == 0)               {acceleration = 0;      }
        }

        //déplacement selon la velocité
        rb.velocity = new Vector3(acceleration,rb.velocity.y,0);
        
        transform.position += new Vector3(acceleration,0,0);

    }


    //orthogonal vector
    public Vector3 PerpendicularClockwise(Vector3 vect){
        return new Vector3(vect.y, -vect.x,0);
    }
    public Vector3 PerpendicularCounterClockwise(Vector3 vect){
        return new Vector3(-vect.y, vect.x,0);
    }


    void CatchAcceleration(){

    }

    private void setLianeAcceleration(Vector3 dir, Vector3 lianeDir){
        lianeAcceleration = Vector3.Dot(dir, lianeDir) ;
        if (!liane.isLeftOfFixed()){
            lianeAcceleration *= -1;
        }
    }

    private void lianeMovment(){
        Vector3 lianeDir = liane.getLianeDir();

        if (lianeAcceleration == 100000) {
            setLianeAcceleration(rb.velocity, lianeDir);
        }

        

        
        rb.velocity = PerpendicularCounterClockwise(lianeDir)*lianeAcceleration * lianeSpeed;

    }


    float CastARay(Vector3 pos,Vector3 dir,float length, LayerMask mask){
        RaycastHit Hit;

        Debug.DrawRay(pos, dir * length);
        bool hitPlateform = Physics.Raycast(pos, transform.TransformDirection( dir), out Hit, length, mask);  
        
        if(hitPlateform) return Hit.distance;
        return -1;
    }




    void Checkground(){
        m_Collider = GetComponent<Collider>();


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
        if(rayDir >0){
            RayCast_Dir=Vector3.up;
            raySize = (0.2f);
        }

        float dist1 = CastARay(RayStart1, transform.TransformDirection( RayCast_Dir),raySize, platformMask);
        float dist2 = CastARay(RayStart2, transform.TransformDirection( RayCast_Dir),raySize, platformMask);

        if (dist1 > 0 || dist2 > 0) {
            float dist = (dist1 + dist2) / 2;

            onGround = true;
            nbJump = MaxJump;
        }else {
            onGround = false;
        }

    }


    void startLiane(){
        if(Input.GetKeyDown(vkLiane)){
            if (liane.isLianeFixed() || liane.getIsExtending()){
                liane.resetLiane();
                lianeAcceleration = 100000;
                acceleration = rb.velocity.x/1000;
                
            }else {
                //TODO 8 DIRECTION
                if (lastDir == 1){//right
                    liane.startExtend(1);
                }else {
                    liane.startExtend(3);
                }
                
            }
        }
    }

    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update(){
        if (liane.isLianeFixed()){
            Debug.Log("liane movment");
            lianeMovment();    
        }else {
            hMovment(Input.GetKey(vkLeft), Input.GetKey(vkRight));
            vMovment();
        }
        



        
        //extendLiane(new Vector3(0.5f,0.5f,0));

        Checkground ();
        startLiane();

    }
}
