using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    private Transform target;

    private EnemyRefrences enemyRefrences;

    private float pathUpdateDeadLine;//to check when we can update the path

    private float attackDistance;
    private bool inRange;



    [SerializeField]private float attackDamage;

    private void Awake()
    {
        enemyRefrences = GetComponent<EnemyRefrences>();
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        attackDistance = enemyRefrences.navMeshAgent.stoppingDistance;
    }

    private void Update()
    {
         inRange = Vector3.Distance(transform.position,target.transform.position)<=attackDistance;

        if (inRange) 
        {
            LookAtTarget();
        } 
        else
        {
            UpdatePath();
        }
    }

    public void LookAtTarget()
    {
        Vector3 lookPos = target.position-transform.position; //looking for direction of player
        lookPos.y = 0f;
        quaternion rotation = Quaternion.LookRotation(lookPos);//cal rotation needed
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);//soomth rotaion of enemy
    }
    public void UpdatePath()
    {
        if(Time.time>=pathUpdateDeadLine)//checking if we can update the path
        {
            
            pathUpdateDeadLine = Time.time+enemyRefrences.pathUpdateDelay;//setting next deadline to update the path
            enemyRefrences.navMeshAgent.SetDestination(target.position);//setting new path

        }

    }

    public void AttackPlayer()
    {
        if(inRange)
        {
            enemyRefrences.playerHealth.DamagePlayer(attackDamage);
        }
    }


}
