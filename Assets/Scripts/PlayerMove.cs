using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Vector2 inputDir;

    private Rigidbody rb;

    [SerializeField]
    LayerMask groundLayerMask;
    [SerializeField]
    private float horizontalMoveSpeed = 1f;

    [SerializeField]
    [Range(1.0f, 5.0f)]
    private float fallMultiplier;

    [SerializeField]
    private float lowJumpMultiplier;

    [SerializeField]
    private bool grounded;

    [SerializeField]
    private bool onWall;
    [SerializeField]
    private bool PrevOnWall;
    private bool onWallL;
    private bool onWallR;

    [SerializeField]
    private float jumpVelocity;

    [SerializeField]
    private float slideVelocity;

    [SerializeField]
    private int maxWallUpdateDelay = 3;
    private int wallUpdateDelay;
    private bool jumpPressed;
    private bool jumpHeld;


    [SerializeField]
    private float groundSkin = 0.05f;

    private Collider collider;

    private bool objectCreated;

    public Vector2 maxWallVelocity = new Vector2(10.0f,10.0f);

    private float maxFallVelocity = 0f;

    [SerializeField]
    CubeAnimate cubeAnim;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody> ();
        collider = GetComponent<BoxCollider> ();
        objectCreated = true;
        wallUpdateDelay = maxWallUpdateDelay;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    void FixedUpdate()
    {
        MoveOnInput();
        SmoothMovement();
    }
    void GetInput()
    {
        inputDir.x = Input.GetAxis("Horizontal");
        inputDir.y = Input.GetAxis("Vertical");

        if(onWall)
            PrevOnWall = onWall;

        if(Input.GetKeyDown(KeyCode.Space) && (grounded || PrevOnWall))
        {
            jumpPressed = true;
        } 
        else 
        {
            jumpPressed = false;   
        }

        if(Input.GetKey(KeyCode.Space))
        {
            jumpHeld = true;
        }
        else
        {
            jumpHeld = false;
        }

        if(!onWall)
        {
            if(wallUpdateDelay == 1)
            {
                PrevOnWall = onWall;
                wallUpdateDelay = maxWallUpdateDelay;
            }
            else 
            {
                wallUpdateDelay -= 1;
            }
        }
        
    }


    void SmoothMovement()
    {
        float verticalVelocity = rb.velocity.y;
        float horizontalVelocity = rb.velocity.x;

        if(verticalVelocity < 0f)
        {
            verticalVelocity += Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            if(verticalVelocity < maxFallVelocity)
                maxFallVelocity = verticalVelocity;
        } 
        else if(verticalVelocity > 0f && !jumpHeld) {
            verticalVelocity += Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        if(onWall && !grounded)
        {
            //Allow slide and Prevent Excess walljumpspeed
            if(verticalVelocity < 0f)
                verticalVelocity = -slideVelocity;
            else
                verticalVelocity = Mathf.Min(verticalVelocity,maxWallVelocity.y);

            //Prevent Wall Grab
            if(onWallL && horizontalVelocity < 0f)
                horizontalVelocity = 0f;
            if(onWallR && horizontalVelocity < 0f)
                horizontalVelocity = 0f;
        }


        rb.velocity = new Vector3(horizontalVelocity, verticalVelocity,0f);
    }
    void MoveOnInput()
    {
        if(jumpPressed ) 
        {
            if(rb.velocity.y > maxWallVelocity.y && onWall)
            {
                rb.velocity = new Vector3(rb.velocity.x,maxWallVelocity.y,0f);

            } else {
                rb.AddForce(Vector3.up * jumpVelocity,ForceMode.Impulse);
            }
            
            grounded = false;
        } 
        else {
            Vector3 boxCenterD = transform.position + Vector3.down * collider.bounds.size.y * 0.5f;
            Vector3 boxHalfScale = new Vector3(transform.localScale.x / 2.0f - 0.05f, groundSkin, transform.localScale.z/2);
            bool airborne = !grounded;
            grounded = (Physics.OverlapBox(boxCenterD,boxHalfScale,Quaternion.identity,groundLayerMask).Length != 0);
            if(airborne && grounded)
            {
                cubeAnim.CubeSquish(maxFallVelocity);
                Debug.Log(maxFallVelocity);
                maxFallVelocity = 0f;
            }
            Vector3 boxCenterL = transform.position + Vector3.left * collider.bounds.size.x * 0.5f;
            Vector3 boxCenterR = transform.position + Vector3.right * collider.bounds.size.x * 0.5f;
            boxHalfScale = new Vector3(groundSkin,transform.localScale.y / 2.0f - 0.05f, transform.localScale.z/2);
            onWallL = (Physics.OverlapBox(boxCenterL,boxHalfScale,transform.rotation,groundLayerMask).Length != 0); 
            onWallR = (Physics.OverlapBox(boxCenterR,boxHalfScale,transform.rotation,groundLayerMask).Length != 0);

            onWall = onWallL || onWallR;
        }

        rb.velocity = new Vector3(inputDir.x * horizontalMoveSpeed, rb.velocity.y,0f);
    }
    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        if(!objectCreated) 
            return;
        Vector3 boxHalfScale = new Vector3(transform.localScale.x - 0.1f, 2.0f * groundSkin, transform.localScale.z);
        Gizmos.DrawWireCube(transform.position + Vector3.down * collider.bounds.size.y * 0.5f, boxHalfScale);

        boxHalfScale = new Vector3(2.0f * groundSkin, transform.localScale.y - 0.1f, transform.localScale.z);
        Gizmos.DrawWireCube(transform.position + Vector3.left * collider.bounds.size.x * 0.5f, boxHalfScale);

        boxHalfScale = new Vector3(2.0f * groundSkin, transform.localScale.y - 0.1f, transform.localScale.z);
        Gizmos.DrawWireCube(transform.position + Vector3.right * collider.bounds.size.x * 0.5f, boxHalfScale);
    }
}


/*
//Old MoveInput
void MoveOnInput()
    {
        if(jumpPressed ) 
        {
            rb.AddForce(Vector3.up * jumpVelocity,ForceMode.Impulse);
            grounded = false;
        } 
        else {
            // for(float i = -1 ; i <= 1; i += 1) 
            // {
            //     Vector3 rayStart = transform.position ;
            //     rayStart += Vector3.right * i * collider.bounds.size.x * 0.5f;
            //     Debug.DrawLine(rayStart,rayStart + Vector3.down * (groundSkin + collider.bounds.size.y) * 0.5f , Color.red,0.5f);
            //     if(Physics.Raycast(rayStart,Vector3.down,groundSkin + collider.bounds.size.y * 0.5f, groundLayerMask))
            //     {
            //         grounded = true;
            //         break;
            //     }

            // }
            Vector3 boxCenter = transform.position + Vector3.down * collider.bounds.size.y * 0.5f;
            Vector3 boxHalfScale = new Vector3(transform.localScale.x / 2, groundSkin, transform.localScale.z/2);
            
            grounded = (Physics.OverlapBox(boxCenter,boxHalfScale,transform.rotation,groundLayerMask).Length != 0);
        }

        rb.velocity = new Vector3(inputDir.x * horizontalMoveSpeed, rb.velocity.y,0f);
    }
*/