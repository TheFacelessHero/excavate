using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator an;
    private Rigidbody2D rb;
    Vector2 velocity;
    [Header("Basic Stats")]
    public bool canFly = false;
    public bool isFlying = false;
    public float verticalFlySpeed;
    public float speed;
    private bool crouching;
    public float walkingSpeed;
    public float crouchSpeed;
    public KeyCode crouchKey;
    public Collider2D headCollider;
    public Vector2 standingHeadPos;
    public Vector2 crouchingHeadPos;
    public float sprintSpeed;
    private bool sprinting;
    public KeyCode sprintKey;
    public float externalSpeedMultiplier;
    public float jumpForce;
    public KeyCode jumpKey;
    private float moveInput;

    public float jumpTime;
    private float jumpTimeCounter;
    private bool isJumping;

    [Header("Grounded")]
    private bool isGrounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;
    public float friction = 1;
    public float airFriction = 0.1f;

    private float origGravScale;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        origGravScale = rb.gravityScale;
        an = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        velocity = new Vector2(Mathf.Lerp(rb.velocity.x,(moveInput * speed),friction), velocity.y);
        rb.velocity = velocity;
        UpdateAnimator();
    }

    private void Update()
    {
        velocity = rb.velocity;
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        if (isGrounded) {

            WorldManager wm = FindObjectOfType<WorldManager>();
            int tile = wm.GetTile(feetPos.position + (Vector3.down * 0.2f));
            if(tile != 0) friction = wm.blockIDs.Blocks[tile].friction;
        }
        else
        {
            friction = airFriction;
        }

        if (moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }


        if (Input.GetKey(crouchKey))
        {
            crouching = true;
            sprinting = false;
            speed = crouchSpeed;
            headCollider.offset = crouchingHeadPos;
        }
        else if (Input.GetKey(sprintKey))
        {
            crouching = false;
            sprinting = true;
            speed = sprintSpeed;
            headCollider.offset = standingHeadPos;
        }
        else
        {
            crouching = false;
            sprinting = false;
            speed = walkingSpeed;
            headCollider.offset = standingHeadPos;
        }
        speed *= externalSpeedMultiplier;

        if (isFlying == false)
        {
            rb.gravityScale = origGravScale;
            if (isGrounded == true && Input.GetKeyDown(jumpKey))
            {
                isJumping = true;
                jumpTimeCounter = jumpTime;
                velocity = Vector2.up * jumpForce;
            }

            if (Input.GetKey(jumpKey) && isJumping == true)
            {
                if (jumpTimeCounter > 0)
                {
                    velocity = Vector2.up * jumpForce;
                    jumpTimeCounter -= Time.deltaTime;
                }
                else
                {
                    isJumping = false;
                }
            }

            if (Input.GetKeyUp(jumpKey))
            {
                isJumping = false;
            }

        }
        else
        {
            rb.gravityScale = 0;
            if (Input.GetKey(jumpKey))
            {
                velocity = Vector2.up * verticalFlySpeed;
            }
            else if (Input.GetKey(crouchKey))
            {
                velocity = Vector2.up * -verticalFlySpeed;
            }
            else
            {
                velocity = Vector2.up * 0;
            }
        }

        if (canFly)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isFlying = !isFlying;
            }
        }else
        {
            isFlying = false;
        }
        


        //UpdateAnimator();
    }

    public void UpdateAnimator()
    {
        an.SetBool("Flying", isFlying);
        an.SetBool("Grounded", isGrounded);
        an.SetBool("Crouching", crouching);
        an.SetBool("Sprinting", sprinting);
        an.SetBool("Jumping", isJumping);
        an.SetFloat("YVel", rb.velocity.y);
        an.SetFloat("XVel", Mathf.Abs(rb.velocity.x));
    }

}
