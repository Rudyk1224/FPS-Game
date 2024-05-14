using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    private AudioSource footstep_Sound;
    [SerializeField]
    private AudioClip[] footstep_Clip;

    private CharacterController character_Controller;
    [HideInInspector]
    public float volumeMin, volumeMax;

    private float accumulated_Distance; //how far we can go before we play a sound
    [HideInInspector]
    public float step_Distance; //how far we can go when we walk/crouch/sprint before we play a sound
    // Start is called before the first frame update
    void Awake()
    {
        footstep_Sound = GetComponent<AudioSource>();
        character_Controller = GetComponentInParent<CharacterController>(); //the parent (Player) has the CharacterController. Only works for 1 instance of CharacterController (attached to only 1 gameObject)
    }

    // Update is called once per frame
    void Update()
    {
        CheckToPlayFootstepSound();
    }
    void CheckToPlayFootstepSound()
    {
        if (!character_Controller.isGrounded) //if character is NOT grounded (isGrounded == false)
            return; //we will return nothing and EXIT the function (return in a void function exits the function immediately)

        if (character_Controller.velocity.sqrMagnitude > 0) //if the x,y or z components of velocity is > 0
        {
            //recall: accumulate distance is the value for how far we can go (i.e: make a step while sprinting, crouching or walking) until we make a footstep noise
            accumulated_Distance += Time.deltaTime; //count the time for this

                
            if (accumulated_Distance > step_Distance) ////if how far we went is greater than how far we can go
            {
                footstep_Sound.volume = Random.Range(volumeMin, volumeMax); //get random volume. Note: floats are inclusive while ints are exclusive
                footstep_Sound.clip = footstep_Clip[Random.Range(0, footstep_Clip.Length)]; //get random sound
                footstep_Sound.Play(); //play the sound
                accumulated_Distance = 0f; //reset the accumulated distance

            }
        }
        else
        {
            accumulated_Distance = 0f;
        }

    }
}
