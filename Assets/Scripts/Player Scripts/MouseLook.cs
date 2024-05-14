using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    private Transform playerRoot,lookRoute;
    [SerializeField]
    private bool invert;
    [SerializeField]
    private bool canUnlock = true;
    [SerializeField]
    private float sensivity = 5f;
    [SerializeField]
    private float smoothWeight = 0.4f;
    [SerializeField]
    private float rollAngle = 10f;
    [SerializeField]
    private float rollSpeed = 3f;
    [SerializeField]
    private Vector2 default_Look_Limits = new Vector2(-70f, 80f);

    private Vector2 look_Angles;
    private Vector2 current_Mouse_Look;
    private Vector2 smooth_Move;

    private float current_Role_Angle;

    private int last_Look_Frame;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //locks cursor (so it cant be seen)
    }

    // Update is called once per frame
    void Update()
    {
        LockAndUnlockCursor();
        if (Cursor.lockState == CursorLockMode.Locked) //If the cursor state is locked
        {
            LookAround();
        }
        void LockAndUnlockCursor()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) //if we press escape key
            {
                if (Cursor.lockState == CursorLockMode.Locked) //if cursor is locked
                {
                    Cursor.lockState = CursorLockMode.None; //none --> unlock cursor and make it visible
                }
                else //if cursor is unlocked
                {
                    Cursor.lockState = CursorLockMode.Locked; //lock the cursor
                    Cursor.visible = false; //make cursor unvisible
                }
            }
        }

        void LookAround()
        {
            //For rotation: + values of x correspond to looking down. - values of x correspond to looking up
            //+ values of y correspond to looking right. - values of y correspond to looking left
            current_Mouse_Look = new Vector2(Input.GetAxis(MouseAxis.MOUSE_Y), Input.GetAxis(MouseAxis.MOUSE_X));
            //thats why we use MOUSE_Y to deal with left and right in the x position and Mouse_X to deal with up and down in the y position


            //x position of lookAngles gets determined by the current mouse x position, sensivity (for smoothness), and invert
            //invert is when you move your mouse up but your player looks down and vice versa. We are testing here to see if invert is true. If it's true, use 1f. If it's not true, use -1f
            look_Angles.x += current_Mouse_Look.x * sensivity * (invert ? 1f : -1f);
            look_Angles.y += current_Mouse_Look.y * sensivity;

            //This line will clamp look_Angles.x between the next 2 values (default look limits of x and y). It won't allow it to go below default x, or above default y
            look_Angles.x = Mathf.Clamp(look_Angles.x, default_Look_Limits.x, default_Look_Limits.y);

            //Set the current role angle to the value we get from our x input (GetAxisRow goes from 0 --> 1 if we move right and if we move left it goes from 0 --> -1. Meanwhile, getAxis increments in slow decimal values (i.e: 0-->0.2...-->1))
            //and finally we have the Time delta time to smooth it out betwe
            //Lerp goes from current role angle to input get axis raw in a time interval linearly (goes from 0 to 3 in a time interval)
            //current_Role_Angle = Mathf.Lerp(current_Role_Angle, Input.GetAxisRaw(MouseAxis.MOUSE_X) *
             //   rollAngle, Time.deltaTime * rollSpeed);
             //But we do not need current rotation on the z axis so role angle is useless to us
             //If you want to create a game where your character gets drunk, you will need rotation in the z axis and so role angle

            lookRoute.localRotation = Quaternion.Euler(look_Angles.x, 0f, 0f); //look route rotates on x axis (up and down)
            playerRoot.localRotation = Quaternion.Euler(0f, look_Angles.y, 0f); //player route rotates on y axis (left and right)

        }
    }
}
