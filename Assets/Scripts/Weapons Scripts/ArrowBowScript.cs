using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBowScript : MonoBehaviour
{
    private Rigidbody myBody;

    public float speed = 30f; //force at which arrow moves in air
    public float deactivateTimer = 3f; //deactivates the arrow for 3 seconds after shooting
    public float damage = 15f; //damage the arrow does to the enemy

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Invoke calls the function with a certain name in quotes, and calls it in the given time (i.e: 3f)
        //So after 3 seconds have passed, it will call DeactivateGameObject and turn off our arrow
        Invoke("DeactivateGameObject", deactivateTimer);
    }
    public void Launch(Camera mainCamera) //this will launch the arrow/spear
    {
        myBody.velocity = mainCamera.transform.forward * speed; //it will move in the forward direction of the camera (towards the crosshair). Velocity will add force right away to the body, while AddForce will add the force over a period of time
        transform.LookAt(transform.position + myBody.velocity); //this will make the arrow look at the direction away from you (rotates it in the direction of the arrow's velocity)
    }
   void DeactivateGameObject()
    {
        if (gameObject.activeInHierarchy) //if the game object is still on in the hierarchy
        {
        gameObject.SetActive(false); //deactivate it
        }
    }
    private void OnTriggerEnter(Collider target) 
    {
        //after we touch an enemy, deactive the game object
        if(target.tag == Tags.ENEMY_TAG)
        {
            target.GetComponent<HealthScript>().ApplyDamage(damage);
            gameObject.SetActive(false);
        }
    }
}
