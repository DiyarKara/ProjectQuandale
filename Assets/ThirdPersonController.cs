using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public CharacterController controller;
    public Transform camera;

    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float turnSpeed = 0.1f;
    float turnVelo;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    private Vector3 velocity;
    [SerializeField] private float gravity;

    [SerializeField] private float jumpHeight = 1.5f;

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        velocity.y += gravity * Time.deltaTime; // calculate gravity
        controller.Move(velocity * Time.deltaTime); // apply gravity
        if (isGrounded && velocity.y < 0) // if on ground dont apply gravity(?)
        {
            velocity.y = -2f;
        }


        float horizontal = Input.GetAxisRaw("Horizontal");  //Horizontal X axis
        float vertical = Input.GetAxisRaw("Vertical"); //Vertical Z axis
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;


        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelo, turnSpeed);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift)) // walking
            {
                print("walk");
                controller.Move(moveDirection.normalized * walkSpeed * Time.deltaTime);
            }
            else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift)) // running
            {
                print("run");
                controller.Move(moveDirection.normalized * runSpeed * Time.deltaTime);
            }
            else if (moveDirection == Vector3.zero) // idle
            {
                print("in idle");
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    print("in jump");
                    Jump();
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                print("in jump");
                if (isGrounded)
                {
                    Jump();
                }
                
            }
        }
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }

    private void OnApplicationFocus(bool focus) // lock the cursor middle of the view
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
