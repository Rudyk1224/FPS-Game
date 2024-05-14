using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthScript : MonoBehaviour
{
    private EnemyAnimator enemy_Anim;
    private NavMeshAgent navAgent;
    private EnemyController enemy_Controller;

    public float health = 100f;

    public bool is_Player, is_Boar, is_Cannibal;
    private bool is_Dead;
    private EnemyAudio enemyAudio;

    public float xDeadRotation = -80;
    public float zDeadRotation = 85;

    private PlayerStats player_Stats;

    void Awake()
    {
        if (is_Boar || is_Cannibal)
        {
            enemy_Anim = GetComponent<EnemyAnimator>();
            enemy_Controller = GetComponent<EnemyController>();
            navAgent = GetComponent<NavMeshAgent>();
            //get enemy audio
            enemyAudio = GetComponentInChildren<EnemyAudio>();
        }
        if (is_Player)
        {
            //get player's health stats
            player_Stats = GetComponent<PlayerStats>();
        }
    }

    public void ApplyDamage(float damage)
    {
        if (is_Dead) //if we die, don't execute the rest of the code
            return;

        health -= damage; //lose some health

        if (is_Player)
        {
            //show the stats (display the health UI value)
            player_Stats.Display_HealthStats(health);
        }

        if (is_Boar || is_Cannibal) //if the gameObject is a bore or a cannibal
        {
            if (enemy_Controller.Enemy_State == EnemyState.PATROL) //and the enemy's state is in the patrol state
            {
                enemy_Controller.chase_Distance = 50f; //in order for the enemy to know he has been shot, we set his chase distance to way bigger so he will chase you
            }
        }
        if (health <= 0f)
        {
            PlayerDied();
            is_Dead = true;
        }
    }
    void PlayerDied()
    {
        if (is_Cannibal) //since we dont have the dead animation for the cannibal, let's create our own dead animation
        {
            GetComponent<Animator>().enabled = false; //deactivate the animation for the cannibal
            GetComponent<BoxCollider>().isTrigger = false;
            transform.rotation = Quaternion.Euler(new Vector3(-xDeadRotation, 0, zDeadRotation));
            //GetComponent<Rigidbody>().AddTorque(-transform.forward * 5f); //rotate (torque = rotation) the cannibal backward (in - direction) so cannibal falls down

            enemy_Controller.enabled = false;
            navAgent.enabled = false;
            enemy_Anim.enabled = false;

            //Start Coroutine to play the dead sound
            StartCoroutine(DeadSound());
            //Enemy Manager: Spawn more enemies
            EnemyManager.instance.EnemyDied(true);
        }
        if (is_Boar)
        {
            navAgent.velocity = Vector3.zero; //stop the nav mesh agent by making its velocity 0
            navAgent.isStopped = true;
            enemy_Controller.enabled = false;
            enemy_Anim.Dead(); //play the dead animation on the bore's animator

            //Start Coroutine to play the dead sound
            StartCoroutine(DeadSound());
            //Enemy Manager: Spawn more enemies
            EnemyManager.instance.EnemyDied(false);
        }
        if (is_Player)
        {
            //once the player dies, enemies must not do anything anymore (cant attack or patrol or anything)
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(Tags.ENEMY_TAG); //create array for enemies

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].GetComponent<EnemyController>().enabled = false; //turn off the enemy controller for all enemies
            }
            //call enemymanager to stop spawning enemies since we died
            EnemyManager.instance.StopSpawningEnemies();
            
            GetComponent<PlayerMovement>().enabled = false; //player cant move anymore since he died
            GetComponent<PlayerAttack>().enabled = false; //neither can he attack
            GetComponent<WeaponManager>().GetCurrentSelectedWeapon().gameObject.SetActive(false); //neither can he select weapons

        }

        if (tag == Tags.PLAYER_TAG)
        {
            Invoke("RestartGame", 3f); //execute RestartGame in 3 seconds
        }
        else
        {
            Invoke("TurnOffGameObject", 3f); //execute TurnOffGameObject in 3 seconds
        }
    }
    void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay"); //this is how we restart our scene
    }
    void TurnOffGameObject()
    {
        gameObject.SetActive(false);
    }
    IEnumerator DeadSound() //recall: coroutines delay the execution of the function
    {
        yield return new WaitForSeconds(0.3f);
        enemyAudio.Play_DeadSound();
    }
}
