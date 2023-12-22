using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    // Variables for debugging and visualizing certain points in the scene.
    [SerializeField] private Transform debugHit;
    [SerializeField] private Transform GrappleShotTransForm;
    [SerializeField] private Transform Enemy;

    // Dashing Control Variables
    private bool dashing = true;
    private float dashingPower = 25f;
    private float dashingTime = 0.45f;
    private float dashingCooldown = 2f;

    // Movement control Variables
    private float SmoothMove = 0.1f; // Smoothness of movement 
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

    // Enemy pull stuff
    private float EnemyPullSpeed = 25f;
    private float PlayerAndEnemyDistanceCheck = 2f;

    // References to other components.
    CharacterController controller;
    CameraLook cl;

    // Grapple variables
    private Vector3 GrappleShotPosition;
    private State state;
    private float GrappleShotSize;

    RaycastHit hit;

   
    // Enum to represent the different states of the player.
    private enum State
    {
        Normal,
        GrappleShotShoot,
        GrappleShotInAir,
        EnemyPull,
    }

   
    private void Awake()
    {
        state = State.Normal;
        GrappleShotTransForm.gameObject.SetActive(false);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cl = GameObject.FindGameObjectWithTag("Player").GetComponent<CameraLook>();
    }

    void Update()
    {
        // State machine for player behavior.
        switch (state)
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
            case State.EnemyPull:
                EnemyGrappling();
                cl.Camera();
                Move();
                break;
        }
    }

    private void Move()
    {
        // Player input for movement.
        Vector3 PlayerInput = new Vector3
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = 0f,
            z = Input.GetAxisRaw("Vertical")
        };

        // To stop faster diagonal movement and for storing its existing movements.
        if (PlayerInput.magnitude > 1f)
        {
            PlayerInput.Normalize();
        }

        // Transform input direction into world space.
        Vector3 MoveDir = transform.TransformDirection(PlayerInput);

        // Checks the current movement speed based on whether the player is holding the "Left Shift" key.
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? RunningSpeed : WalkingSpeed;

        // For smoothness of player's Velocity.
        CurrentVelocity = Vector3.SmoothDamp(CurrentVelocity, MoveDir * currentSpeed, ref SDampVelocity, SmoothMove);
        controller.Move(CurrentVelocity * Time.deltaTime);

        // Raycast to check if it's grounded
        Ray GroundCheck = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(GroundCheck, 1.1f))
        {
            // Reset jumps when grounded and apply a downward force.
            JumpsRemaining = 1;
            CurrentForceVelocity.y = -1f;

            // Checking for Jump press or input
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

        // Moves the player based on the force velocity.
        controller.Move(CurrentForceVelocity * Time.deltaTime);
        if (VelocityMomentum.magnitude > 0)
        {
            //basically slowing down the drag(the slide)
            float MomentumDrag = 3f;
            VelocityMomentum -= VelocityMomentum * MomentumDrag * Time.deltaTime;
        }
    }

    private IEnumerator Dash()
    {


        dashing = false; // First, dashing will be true

        // Move forward in the direction for dash
        CurrentVelocity = new Vector3(transform.forward.x * dashingPower, 0f, transform.forward.z * dashingPower);
        yield return new WaitForSeconds(dashingTime); // How much time the dash will be
        CurrentVelocity = Vector3.zero; // After dash, player velocity turns to zero

        yield return new WaitForSeconds(dashingCooldown); // After completing dash, it takes some time to dash again
        dashing = true; // Setting again to true so we can dash again after one whole cycle
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
        if (InputDownGrappleShot())
        {
            if (Physics.Raycast(cl.CameraPlayer.transform.position, cl.CameraPlayer.transform.forward, out RaycastHit raycastHit))
            {
                debugHit.position = raycastHit.point; // Where the mouse is pointed and clicked  a new cube object will be created for ref
                GrappleShotPosition = raycastHit.point; // Where we shoot an object will be created
                GrappleShotSize = 0;
                GrappleShotTransForm.gameObject.SetActive(true); // Grapple will be set to true if we shoot
                GrappleShotTransForm.localScale = Vector3.zero; // Our  position will be set to zero
                state = State.GrappleShotShoot;
                hit = raycastHit;
                state = State.GrappleShotShoot;
            }
        }
    }

    public void GrappleShotShooted()
    {
        GrappleShotTransForm.LookAt(GrappleShotPosition);
        float GrappleShootSpeed = 250f;
        GrappleShotSize += GrappleShootSpeed * Time.deltaTime;
        GrappleShotTransForm.localScale = new Vector3(1f, 1f, GrappleShotSize);

        // In air state
        if (GrappleShotSize >= Vector3.Distance(transform.position, GrappleShotPosition))
        {
            state = State.GrappleShotInAir;
            GrappleShotTransForm.gameObject.SetActive(false); // Disables the grappleHook in air while grappling
        }

        // Checking if the grapple hits an enemy
        if (GrappleShotSize >= Vector3.Distance(transform.position, GrappleShotPosition))
        {
            state = hit.collider.CompareTag("Enemy") ? State.EnemyPull : State.GrappleShotInAir;//?its basically like if else

            if (state == State.EnemyPull)
            {
                Enemy = hit.collider.gameObject.transform;
            }
        }
    }

    private void GrappleShotMovement()
    {
        GrappleShotTransForm.LookAt(GrappleShotPosition);
        Vector3 GrappleShotDir = (GrappleShotPosition - transform.position).normalized; // Normalizing our new Grapple Shot position
        float GrappleShotMin = 15f;
        float GrappleShotMax = 40f;

        // Clamping our min and max value
        float GrappleShotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, GrappleShotPosition), GrappleShotMin, GrappleShotMax);
        float GrappleShotSpeedMul = 2f;

        // Moving the player using character controller
        controller.Move(GrappleShotDir * GrappleShotSpeed * GrappleShotSpeedMul * Time.deltaTime);

        float reachGrappleShotPos = 2f;

        // Set the grapple wire to false when we stop
        if (Vector3.Distance(transform.position, GrappleShotPosition) < reachGrappleShotPos)
        {
            state = State.Normal;
            GrappleShotTransForm.gameObject.SetActive(false);
        }

        // Cancel grapple if the player presses the grapple key again
        if (InputDownGrappleShot())
        {
            state = State.Normal;
        }

        // Jump in between grappling and other movements
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float MomentumSpeedSlowDown = 0.25f;
            VelocityMomentum = GrappleShotDir * GrappleShotSpeed * MomentumSpeedSlowDown;
            float GrappleJumpSpeed = 1.7f;
            VelocityMomentum += Vector3.up * GrappleJumpSpeed;
            state = State.Normal;
        }
    }

    // Not hardcoding for Jumping
    private bool InputDownGrappleShot()
    {
        return Input.GetKeyDown(KeyCode.F); // Input Function
    }

    private void EnemyGrappling()
    {
        //checks for proper reference
        if (Enemy != null)
        {
            if (Enemy.GetComponent<CharacterController>() != null)
            {
               //checking dir of the player and normalizing it 
                Vector3 EnemyDir = (transform.position - Enemy.position).normalized;
                //Moves the enemy to player at the specific speed
                Enemy.GetComponent<CharacterController>().Move(EnemyDir * EnemyPullSpeed * Time.deltaTime);

               //distance checking between enemy and the player so its stop 
                if (Vector3.Distance(transform.position, Enemy.position) < PlayerAndEnemyDistanceCheck)
                {
                    state = State.Normal;
                    Enemy = null;//clearing enemy ref if its not grappled
                }
            }
            else
            {
                state = State.Normal;
                Enemy = null;
            }
        }
        else
        {
            state = State.Normal;
        }
    }
}
