using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;


    /*Variables for moving*/
    [SerializeField]
    private float walkingSpeed = 5;
    [SerializeField]
    private float rotationSpeed =10;
    private float movementX;
    private float movementZ;
    private float YRotation = 0;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        if(YRotation > 0)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        else if (YRotation < 0)
        {
            transform.Rotate(Vector3.down, rotationSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = transform.forward * movementZ + transform.right * movementX;
        rb.MovePosition(transform.position + (moveDirection.normalized*walkingSpeed*Time.deltaTime));
        rb.angularVelocity = Vector3.zero;
        //rb.AddForce(moveDirection.normalized*walkingSpeed,ForceMode.Acceleration);
    }


    void OnMove(InputValue movementValue) 
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementZ = movementVector.y;
    }
    void OnRotate(InputValue movementValue) 
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        YRotation = movementVector.x;

    }

}

