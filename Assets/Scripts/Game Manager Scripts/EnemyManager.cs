using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance; //this instance will make it easy to reference this gameObject/script without using get

    [SerializeField]
    private GameObject boar_Prefab, cannibal_Prefab;

    public Transform[] cannibal_SpawnPoints, boar_SpawnPoints; //array for different spawn points for the cannibal and boar

    [SerializeField]
    private int cannibal_Count, boar_Count;

    private int initial_Cannibal_Count, initial_Boar_Count;

    public float wait_Before_Spawn_Enemies_Time = 10f;
    // Start is called before the first frame update
    void Awake()
    {
        MakeInstance();
    }
    void Start()
    {
        initial_Cannibal_Count = cannibal_Count; //set the initial cannibal count to be the cannibal count (i.e: 6)
        initial_Boar_Count = boar_Count; 

        SpawnEnemies();

        StartCoroutine("CheckToSpawnEnemies");
    }


    void MakeInstance()
    {
        if(instance == null)
        {
            instance = this; //create instance of this gameObject (enemy manager)
        }
    }
    
    void SpawnEnemies()
    {
        SpawnCannibals();
        SpawnBoars();
    }

    void SpawnCannibals()
    {
        int index = 0;

        for (int i = 0; i < cannibal_Count; i++) //as long as index is less than the max amount of cannibals (cannibals count)
        {
            if(index >= cannibal_SpawnPoints.Length) //if cannibal count is 10, and there were 6 cannibal spawn points, the index would eventually be greater than 6, so we reset the spawn point index back to  0
            {
                index = 0; //reset the index to avoid out of bounds exception
            }
            Instantiate(cannibal_Prefab, cannibal_SpawnPoints[index].position,Quaternion.identity); //spawn cannibals at the spawn point's position and give it no rotation (Quaternion.identity)
            index++;
        }
        //once the i >= cannibal count, we have enough cannibals created so lets reset the cannibal count back to 0
        cannibal_Count = 0;
    }
    void SpawnBoars()
    {
        int index = 0;

        for (int i = 0; i < boar_Count; i++) 
        {
            if (index >= boar_SpawnPoints.Length) 
            {
                index = 0; 
            }
            Instantiate(boar_Prefab, boar_SpawnPoints[index].position, Quaternion.identity); 
            index++;
        }
        boar_Count = 0;
    }

    IEnumerator CheckToSpawnEnemies()
    {
        yield return new WaitForSeconds(wait_Before_Spawn_Enemies_Time); //wait 10 seconds before we spawn a new enemy

        SpawnCannibals(); //spawn cannibals (if i < cannibal_Count)
        SpawnBoars();

        StartCoroutine("CheckToSpawnEnemies"); //recursion! Creates infinite coroutine
    }

    public void EnemyDied(bool cannibal)
    {
        if (cannibal) //if the cannibal died is true
        {
            cannibal_Count++; //increase the count of the cannibals so that we are able to spawn 1 more cannibal
            if(cannibal_Count > initial_Cannibal_Count) //initial cannibal count was 6. if initial count was 6 and cannibal count is 7, then cannibal count will be changed to 6. This makes sure we dont a count of more than 6 cannibals
            {
                cannibal_Count = initial_Cannibal_Count; //so we cant spawn more than 6 at a time (in Spawn Cannibals() )
            }

        }
        else //if cannibal is false, it's a boar
        {
            boar_Count++;

            if(boar_Count > initial_Boar_Count)
            {
                boar_Count = initial_Boar_Count;
            }
        }
    }

    public void StopSpawningEnemies()
    {
        StopCoroutine("CheckToSpawnEnemies");
    }
}
