using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public VariableJoystick variableJoystick;
    public Rigidbody rb;
    [SerializeField] private Transform stickman;
    [SerializeField] private Animator animator;

    private void Start()
    {
        animator = stickman.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Vector3 joystickDirection = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;

        Vector3 movement = joystickDirection * speed * Time.fixedDeltaTime;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z); // Apply movement without affecting the y-velocity

        bool isMoving = joystickDirection.magnitude > 0;

        animator.SetBool("isMoving", isMoving);

        if (joystickDirection != Vector3.zero)
        {
            Vector3 direction = joystickDirection.normalized;
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, toRotation, 360 * Time.fixedDeltaTime));
        }
    }
}
