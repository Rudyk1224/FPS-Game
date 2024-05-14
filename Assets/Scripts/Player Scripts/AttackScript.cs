using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public float damage = 2f;
    public float radius = 1f;
    public LayerMask layerMask;


    // Update is called once per frame
    void Update()
    {
        //this is a collider array that creates a sphere at the attack point's position with a certain radius and a certain layer (i.e: enemy layer). So it only detects collisions with gameObjects that are on the "Enemy" layer
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, layerMask);

        if (hits.Length > 0)
        { 
            //if we touched a game object
            hits[0].gameObject.GetComponent<HealthScript>().ApplyDamage(damage);
            gameObject.SetActive(false);
        }
    }
}
