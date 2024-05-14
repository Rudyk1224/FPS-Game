using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController character_Controller;
    private Vector3 moveDirection;
    public float speed = 5f;
    private float gravity = 20f;
    public float jump_Force = 10f;
    private float vertical_Velocity;

   
    private void Awake()
    {
        character_Controller = GetComponent<CharacterController>(); //gets access to the properties of the character controller of the player
    }
 

    // Update is called once per frame
    void Update()
    {
        MoveThePlayer();
        
    }

    void MoveThePlayer()
    {
        //x moves player left or right. z moves player forward or back
        moveDirection = new Vector3(Input.GetAxis(Axis.HORIZONTAL), 0f, Input.GetAxis(Axis.VERTICAL));

        moveDirection = transform.TransformDirection(moveDirection);
        //local space is local to the player himself (what the player can see at that moment). Its relative to its current position
        //world space is relative to the entire scene (entire world)
        //so we transform the transform of this game object from local to world space
        moveDirection *= speed * Time.deltaTime;
        ApplyGravity();
        character_Controller.Move(moveDirection); //move character controller by passing in an amount/speed (moveDirection)
    }

    void ApplyGravity()
    {
       
         vertical_Velocity -= gravity * Time.deltaTime; //apply gravity

         //jump if the conditions are true
         PlayerJump(); 
        
        moveDirection.y = vertical_Velocity * Time.deltaTime; //this will affect the actual moveDirection y position of vector
    }
    void PlayerJump()
    {
        //if we are on the ground and we press the space bar
        if(character_Controller.isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            vertical_Velocity = jump_Force; //make vertical velocity equal to the jump force
        }
    }

  

    

}
