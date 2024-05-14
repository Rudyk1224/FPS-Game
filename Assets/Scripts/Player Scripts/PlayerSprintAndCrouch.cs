using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprintAndCrouch : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public float sprint_Speed = 10f;
    public float move_Speed = 5f;
    public float crouch_Speed = 2f;

    private Transform look_Root;
    private float stand_Height = 1.6f;
    private float crouch_Height = 1f;

    private bool is_Crouching;

    private PlayerFootsteps player_Footsteps;

    private float sprint_Volume = 1f; //volume at 100%
    private float crouch_Volume = 0.1f; //volume at 10%
    private float walk_Volume_Min = 0.2f, walk_Volume_Max = 0.6f;

    private float walk_Step_Distance = 0.4f; //When we walk, we go medium, so we will hear the sounds sometimes (every 0.4 of a second)
    private float sprint_Step_Distance = 0.25f; //When we sprint, we go fast, so we will hear the sounds frequently after a low amount of steps (every 0.25 of a second)
    private float crouch_Step_Distance = 0.5f; //When we crouch, we go slow, so we will hear the sounds infrequently after a large amount of steps (every 0.5 of a second)

    private PlayerStats player_Stats;
    private float sprint_Value = 100f;
    public float sprint_Threshold = 10f;
    void Awake()
    {
        //get references to components
        playerMovement = GetComponent<PlayerMovement>();
        look_Root = transform.GetChild(0); //this will get the child of the Player gameObject which is Look Root and get access to its transform
        player_Footsteps = GetComponentInChildren<PlayerFootsteps>(); //will look for children who have PlayerFootSteps component
        player_Stats = GetComponent<PlayerStats>();
        //Note: if we have more than 1 component (more than 1 gameObject has it), it's better to make it a private variable with SerializeField or public variable, and drag and drop it
    }
    void Start()
    {
        //Player starts off walking
        player_Footsteps.volumeMin = walk_Volume_Min;
        player_Footsteps.volumeMax = walk_Volume_Max;
        player_Footsteps.step_Distance = walk_Step_Distance;
    }

    // Update is called once per frame
    void Update()
    {
        Sprint();
        Crouch();
    }

    void Sprint()
    {

       if(sprint_Value > 0f) //if we have stamina, we can sprint
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !is_Crouching) //when you're not crouching and holding DOWN the shift key
            {
                playerMovement.speed = sprint_Speed; //change playerMovement's speed to something bigger
                player_Footsteps.step_Distance = sprint_Step_Distance;
                player_Footsteps.volumeMin = sprint_Volume; //we want to simulate that we touch the grass hard so loud sound
                player_Footsteps.volumeMax = sprint_Volume;
            }
        }
        if(Input.GetKeyUp(KeyCode.LeftShift) && !is_Crouching) //when you're not crouching and LET GO of the shift key
        {
            //Go back to normal walking speed, with normal step distance and normal volume
            playerMovement.speed = move_Speed;

            player_Footsteps.step_Distance = walk_Step_Distance;
            player_Footsteps.volumeMin = walk_Volume_Min;
            player_Footsteps.volumeMax = walk_Volume_Max;
           
        }

        if(Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))) //if we press and HOLD the left shift and we are not crouching and we press the correspond left/right/up/down key. If we used GetKeyDown, it will only execute the block of code once and the stamina won't decrease at all
        {
            sprint_Value -= sprint_Threshold * Time.deltaTime;
            if (sprint_Value <= 0f)
            {
                sprint_Value = 0f;
                //Now reset the speed and sound to normal walking speed and sound:
                playerMovement.speed = move_Speed;
                player_Footsteps.step_Distance = walk_Step_Distance;
                player_Footsteps.volumeMin = walk_Volume_Min;
                player_Footsteps.volumeMax = walk_Volume_Max;
            }
            player_Stats.Display_StaminaStats(sprint_Value); //display stamina 
        }
        else
        {
            if(sprint_Value != 100f ) //if we are walking or crouching and our stamina is not 100, regenerate stamina
            {
                sprint_Value += (sprint_Threshold / 2f) * Time.deltaTime; //we will spend the stamina value 2 times faster than if we were walking and regenerating stamina
                player_Stats.Display_StaminaStats(sprint_Value); //display stamina
                if(sprint_Value > 100f)
                {
                    sprint_Value = 100f;
                }
            }
        }
    }
    void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (is_Crouching) //if we are crouching (is_Crouching is true), we must stand up
            {
                //localPosition is the position with respect to the parent (Player). Position refers to the position with respect to the world which is not what we want (it will crouch us out of the world)
                look_Root.localPosition = new Vector3(0f, stand_Height, 0f);
                playerMovement.speed = crouch_Speed;

                player_Footsteps.step_Distance = walk_Step_Distance;
                player_Footsteps.volumeMin = walk_Volume_Min;
                player_Footsteps.volumeMax = walk_Volume_Max;

                is_Crouching = false; //we are standing up now
            }
            else //if we are not crouching (is_Crouching is false), we must crouch down
            {
                look_Root.localPosition = new Vector3(0f, crouch_Height, 0f);
                playerMovement.speed = crouch_Speed;

                player_Footsteps.step_Distance = crouch_Step_Distance;
                player_Footsteps.volumeMin = crouch_Volume;
                player_Footsteps.volumeMax = crouch_Volume;
                is_Crouching = true;

            }
        }
    }
}
