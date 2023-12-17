using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;





public class PlayerMovement : MonoBehaviour
{   //just to check where our grapple is hitting with object.
    [SerializeField]
    private Transform debugHit;
    [SerializeField]
    private Transform GrappleShotTransForm;

    //Dashing Control Variables
    private bool dashing = true;
    private float dashingPower = 25f;
    private float dashingTime = 0.45f;
    private float dashingCooldown = 1f;
    //  Movement control Variables
    private float SmoothMove = 0.1f; // smoothness of movement 
    private float WalkingSpeed = 3.5f; 
    private float RunningSpeed = 7f; 
    private float GravityPower = 10f; // Strength of gravity affecting the player.
    private float JumpPower = 20f; 
    private float DoubleJumpPower = 25f; 
    private int JumpsRemaining = 1; 

    // Storing Current Velocity Variables.
    private Vector3 CurrentVelocity; // Current velocity of the player.
    private Vector3 SDampVelocity; // Velocity used for smoothing movement transitions.
    private Vector3 CurrentForceVelocity; // Current force-based velocity.
    private Vector3 VelocityMomentum;

   
    CharacterController controller;
    CameraLook cl;

    private Vector3 GrappleShotPosition;
    private State state;
    private float GrappleShotSize;

    private enum State
    {
        Normal,
        GrappleShotShoot,
        GrappleShotInAir,
    }


    private void Awake()
    {
        state=State.Normal;
        GrappleShotTransForm.gameObject.SetActive(false);
    }

    void Start()
    {
        
        controller = GetComponent<CharacterController>();
        cl=GameObject.FindGameObjectWithTag("Player").GetComponent<CameraLook>();
          
    }

   
    void Update()
    { switch (state)
        {
            default:
            case State.Normal:


                Move();
                Dashing();
                GrappleStart();
                cl.Camera();
                break;
                case State.GrappleShotShoot:
                GrappleShotShooted();
                Move();
                cl.Camera();
                break;
                case State.GrappleShotInAir:
                cl.Camera();
                GrappleShotMovement();
             
                break;
        }
    }
   


    private void Move()
    {
        //Player input for movement.
        Vector3 PlayerInput = new Vector3
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = 0f,
            z = Input.GetAxisRaw("Vertical")
        };

        // To stop faster diagonal movement and for to store its existing movements.
        if (PlayerInput.magnitude > 1f)
        {
            PlayerInput.Normalize();
        }

        // Transform input direction into world space.
        Vector3 MoveDir = transform.TransformDirection(PlayerInput);

        // Checks the current movement speed based if the player is holding the "Left Shift" key.
       // ?(operator) its basically like if else 
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? RunningSpeed : WalkingSpeed;

        // For smoothness of players Velocity.
        CurrentVelocity = Vector3.SmoothDamp(CurrentVelocity, MoveDir * currentSpeed, ref SDampVelocity, SmoothMove);
        controller.Move(CurrentVelocity * Time.deltaTime);

        //Raycast to check if its grounded
        Ray GroundCheck = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(GroundCheck, 1.1f))
        {
            // Reset jumps when grounded and apply a downward force.
            JumpsRemaining = 1;
            CurrentForceVelocity.y = -1f;

            //Checking for Jump press or input
            if (Input.GetKey(KeyCode.Space))
            {
                CurrentVelocity.y = JumpPower;
            }
        }
        else
        {
            // Apply gravity when the player is not grounded.
            CurrentForceVelocity.y -= GravityPower * Time.deltaTime;

            // Check for double jump input 
            if (JumpsRemaining > 0 && Input.GetKeyDown(KeyCode.Space))
            {
                CurrentVelocity.y = DoubleJumpPower;
                JumpsRemaining--;
            }
        }

        CurrentVelocity += VelocityMomentum;
        // Moves the player based on the force velocity.
        controller.Move(CurrentForceVelocity * Time.deltaTime);
        if(VelocityMomentum.magnitude>0)
        {
            float MomentumDrag = 3f;
            VelocityMomentum-=VelocityMomentum*MomentumDrag*Time.deltaTime;
        }

       
    }
    private IEnumerator Dash()
    {
        dashing = true;//First The dashing will be true
      
        CurrentVelocity = new Vector3(transform.forward.x * dashingPower, 0f, transform.forward.z * dashingPower);//This is dont to move forward in direction for dash
        yield return new WaitForSeconds(dashingTime);//How much time the dash will be
        CurrentVelocity = Vector3.zero;//after dash our player velocity turn to zero
     
        yield return new WaitForSeconds(dashingCooldown);//After we complete dash it will take some time to dash again
        dashing = true;//setting again to true so we can dash again after one whole cycle

    }
    private void Dashing()
    {
        if (Input.GetKeyDown(KeyCode.Z) && dashing)
        {
            StartCoroutine(Dash());
        }
    }
    
    private void GrappleStart()
    {
        if (TestInputDownGrappleShot())
        {
            if(Physics.Raycast(cl.CameraPlayer.transform.position, cl.CameraPlayer.transform.forward,out RaycastHit raycastHit))
            {
                debugHit.position = raycastHit.point;
                GrappleShotPosition = raycastHit.point;
                GrappleShotSize = 0;
                GrappleShotTransForm.gameObject.SetActive(true);
                GrappleShotTransForm.localScale = Vector3.zero;
                state = State.GrappleShotShoot;
            }
        }
    }
    private void GrappleShotShooted()
    {
        GrappleShotTransForm.LookAt(GrappleShotPosition);
        float GrappleShootSpeed = 250f;
        GrappleShotSize += GrappleShootSpeed * Time.deltaTime;
        GrappleShotTransForm.localScale = new Vector3(1f, 1f, GrappleShotSize);
        if (GrappleShotSize >= Vector3.Distance(transform.position, GrappleShotPosition))
        {
            state = State.GrappleShotInAir;
        }


    }
    private void GrappleShotMovement()
    {
        GrappleShotTransForm.LookAt(GrappleShotPosition);
        Vector3 GrappleShotDir = (GrappleShotPosition - transform.position).normalized;
        float GrappleShotMin = 15f;
        float GrappleShotMax = 40f;
       
        float GrappleShotSpeed =Mathf.Clamp(Vector3.Distance(transform.position,GrappleShotPosition),GrappleShotMin,GrappleShotMax);
        float GrappleShotSpeedMul = 2f;
       
        controller.Move(GrappleShotDir *GrappleShotSpeed* GrappleShotSpeedMul* Time.deltaTime);
        float reachGrappleShotPos = 2f;
        if(Vector3.Distance(transform.position,GrappleShotPosition)<reachGrappleShotPos)
        {
            state = State.Normal;
            GrappleShotTransForm.gameObject.SetActive(false);
        }
        if(TestInputDownGrappleShot())
        {
            state = State.Normal;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            float MomentumSpeedSlowDown = 0.25f;
            VelocityMomentum=GrappleShotDir*GrappleShotSpeed*MomentumSpeedSlowDown;
            float GrappleJumpSpeed = 1.7f;
            VelocityMomentum += Vector3.up * GrappleJumpSpeed;
            state = State.Normal;
        }
    }
    private bool TestInputDownGrappleShot()
    {
        return Input.GetKeyDown(KeyCode.F);//Input Function

    }
   


}

