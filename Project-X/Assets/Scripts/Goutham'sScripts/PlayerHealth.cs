using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    
    [SerializeField]private float health;


    public void DamagePlayer(float damage)
    {
        health -= damage;
    }

}
