using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState{
    PATROL,
    CHASE,
    ATTACK
    }
public class EnemyController : MonoBehaviour
{
    private EnemyAnimator enemyAnim;
    private NavMeshAgent navAgent;
    private EnemyState enemyState; //reference to enemy state enum

    public float walk_Speed = 0.5f;
    public float run_Speed = 4f;
    public float chase_Distance = 7f; //determines how far the enemy is from the player before it starts chasing the player

    private float current_Chase_Distance;
    public float attack_Distance = 1.8f;
    public float chase_After_Attack_Distance = 2f; //when the player runs away, we will leave a little space between him and the enemy

    public float patrol_Radius_Min = 20f, patrol_Radius_Max = 60f;
    public float patrol_For_This_Time = 15f; //how many seconds he will patrol 
    private float patrol_Timer;

    public float wait_Before_Attack = 2f; //wait some time before we attack
    private float attack_Timer;

    private Transform target; //target for our player

    public GameObject attack_Point;

    private EnemyAudio enemy_Audio;

    void Awake()
    {
        enemyAnim = GetComponent<EnemyAnimator>();
        navAgent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag(Tags.PLAYER_TAG).transform;
        enemy_Audio = GetComponentInChildren<EnemyAudio>();
    }
    // Start is called before the first frame update
    void Start()

    {
        enemyState = EnemyState.PATROL; //initial state of our enemy is patrol

        patrol_Timer = patrol_For_This_Time; //start off patrol timer at 15
        //when the enemy first gets to the player, attack right away
        attack_Timer = wait_Before_Attack;
        //memorize the value of chase distance so that we could put it back
        current_Chase_Distance = chase_Distance;
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyState == EnemyState.PATROL)
        {
            Patrol();
        }
        if (enemyState == EnemyState.CHASE)
        {
            Chase();
        }
        if (enemyState == EnemyState.ATTACK)
        {
            Attack();
        }
    }
    void Patrol()
    {
        //tell nav agent that he can move (is stopped is false)
        navAgent.isStopped = false;
        navAgent.speed = walk_Speed; //when we patrol, we walk slowly so its walk speed
        patrol_Timer += Time.deltaTime; //add on to the patrol timer

        if (patrol_Timer > patrol_For_This_Time) //patrol for this time = how many seconds he will patrol before we set a new destination    
        {
            SetNewRandomDestination();
            patrol_Timer = 0f; //reset patrol timer
        }

        if (navAgent.velocity.sqrMagnitude > 0)
        {    //if we are moving and have some velocity
            enemyAnim.Walk(true); //make him walk
        }
        else
        {
            enemyAnim.Walk(false);
        }
        //test the distance between the player (target.position) and the enemy (transform.position)
        if(Vector3.Distance(transform.position, target.position) <= chase_Distance) //if distance between enemy and player is less than chase distance
        {
            enemyAnim.Walk(false); //enemy is no longer walking
            enemyState = EnemyState.CHASE; //he is now in the chase state

            //play spotted audio
            enemy_Audio.Play_ScreamSound();
        }
    }
    void Chase()
    {
        
        navAgent.isStopped = false; //enable the agent/enemy to move again
        navAgent.speed = run_Speed; //make speed of nav agent the run speed

        
        navAgent.SetDestination(target.position); //set the player's position as the destination because we are chasing after the player   

        if (navAgent.velocity.sqrMagnitude > 0)
        {    //if we are moving and have some velocity
            enemyAnim.Run(true); //make him walk
        }
        else
        {
            enemyAnim.Run(false);
        }

        if(Vector3.Distance(transform.position,target.position) <= attack_Distance)
        {
            enemyAnim.Run(false);
            enemyAnim.Walk(false);
            enemyState = EnemyState.ATTACK;
            //reset the chase distance to the previous value
            if(chase_Distance != current_Chase_Distance) //if player runs away from enemy
            {
                chase_Distance = current_Chase_Distance; //the current chase distance will now be reset to the original chase distance
            }
        } else if(Vector3.Distance(transform.position,target.position) > chase_Distance) //player runs away from enemy
        {
            enemyAnim.Run(false); //if the distance is too great, the enemy will stop running and wont chase anymore
            enemyState = EnemyState.PATROL; //enemy will now patrol
            patrol_Timer = patrol_For_This_Time;

            //reset the chase distance to previous
            if(chase_Distance != current_Chase_Distance)
            {
                chase_Distance = current_Chase_Distance; //reset the chase distance so we are able to run away from the enemy
            }

        }
    }
    void Attack()
    {
        navAgent.velocity = Vector3.zero; //stop enemy from moving or having any velocity
        navAgent.isStopped = true; //so the nav agent is now stopped since he is attacking

        attack_Timer += Time.deltaTime;

        if(attack_Timer > wait_Before_Attack)
        {
            enemyAnim.Attack();

            attack_Timer = 0f; //reset attack timer

            //play attack sound
            enemy_Audio.Play_AttackSound();
        }
        //if the distance between the enemy and player is greater than the attack distance + the space between the player and enemy
        if(Vector3.Distance(transform.position,target.position) > attack_Distance + chase_After_Attack_Distance)
        {
            enemyState = EnemyState.CHASE; //enemy will now be in the chase state
        }

    }
    void SetNewRandomDestination()
    {
        float rand_Radius = Random.Range(patrol_Radius_Min, patrol_Radius_Max); //gets a value between 20 and 60
        Vector3 randDir = Random.insideUnitSphere * rand_Radius; //insideUnitSphere returns a value between 0 and 1, but we mulitply it by rand radius so itll be a larger number
        randDir += transform.position; //we add that random point to the current position (we add vector to vector pretty much)

        NavMeshHit navHit;

        //sample position gets the random direction and calculates if that is within the navigationable area. If not, it will calculate a new area.
        //navHit is the generated position within the random direction. rand radius will be the maximum distance. The last position checks what layer this should be applied on (i.e: default layer). -1 means include all layers
        NavMesh.SamplePosition(randDir, out navHit, rand_Radius, -1);
        navAgent.SetDestination(navHit.position);
    }

    void Turn_On_AttackPoint()
    {
        attack_Point.SetActive(true);
    }
    void Turn_Off_AttackPoint()
    {
        if (attack_Point.activeInHierarchy)
            attack_Point.SetActive(false);
    }

    public EnemyState Enemy_State
    {
        get; set; //shortcut for return enemyState and set enemyState = value
    }
}
