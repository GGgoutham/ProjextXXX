using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]

public class EnemyRefrences : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    //public Animator animator;//in future when we add animation
    public PlayerHealth playerHealth;

    [Header("Stats")]
    public float pathUpdateDelay = 0.2f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
       // animator = GetComponent<Animator>();

        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

}
