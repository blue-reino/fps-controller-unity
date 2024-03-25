using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    public CharacterController controller;
    public AudioSource footStepSound;
    public AudioSource preJump;
    public AudioSource afterJump;
    public float speed = 5f;
    public float sprintSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpForce = 1f;
    public float forwardJumpDuration = 0.5f; // Duration of forward movement after jump
    public float slideSpeed = 15f; // Speed of sliding
    public float slideDeceleration = 30f; // Rate of slide deceleration

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    bool isJumping;
    float forwardJumpTimer;
    Vector3 slideDirection;
    float currentSlideSpeed;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            isJumping = false;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Apply forward motion
        if (!isJumping)
        {
            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
            {
                controller.Move(move * sprintSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Apply forward movement during jump
            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
            {
                controller.Move(move * sprintSpeed * Time.deltaTime);
            }
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            isJumping = true;
            forwardJumpTimer = 0f; // Reset timer

            // Play forward jump sound
            preJump.enabled = true;
            afterJump.enabled = false;
        }

        if (isJumping)
        {
            // Apply forward movement after jump
            forwardJumpTimer += Time.deltaTime;
            if (forwardJumpTimer < forwardJumpDuration)
            {
                float forwardMovement = Mathf.Lerp(0, speed, forwardJumpTimer / forwardJumpDuration);
                controller.Move(transform.forward * forwardMovement * Time.deltaTime);
            }
            else
            {
                isJumping = false;

            }


        }

        // Slide when landing
        if (!isJumping && !isGrounded)
        {
            slideDirection = move.normalized; // Store the direction the player was facing
            currentSlideSpeed = slideSpeed; // Reset slide speed
            // Play after jump sound
            preJump.enabled = false;
            afterJump.enabled = true;
        }
        else if (isGrounded && slideDirection != Vector3.zero)
        {
            // Play after jump sound
            preJump.enabled = false;
            afterJump.enabled = true;
            StartCoroutine(SlideCoroutine());

        }

        // Update footstep sound
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftShift))
        {
            footStepSound.enabled = true;
            footStepSound.pitch = Input.GetKey(KeyCode.LeftShift) ? 1.5f : 1.0f;
        }
        else
        {
            footStepSound.enabled = false;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    IEnumerator SlideCoroutine()
    {
        while (currentSlideSpeed > 0)
        {
            controller.Move(slideDirection * currentSlideSpeed * Time.deltaTime);
            currentSlideSpeed -= slideDeceleration * Time.deltaTime;
            yield return null;
        }

        slideDirection = Vector3.zero;
    }
}
