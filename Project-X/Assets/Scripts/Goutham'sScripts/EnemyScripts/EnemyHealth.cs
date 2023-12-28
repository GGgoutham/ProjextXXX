using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{

    [SerializeField] private int max_Health = 100;
    [SerializeField] private int health;


   public int currentHealth { get => health; private set => health = value; }   

    public int maxHealth { get => max_Health; private set => max_Health = value; }

    public event IDamageable.TakeDamageEvent OntakeDamage;

    public  event IDamageable.DeathEvent OnDeath;

    // Start is called before the first frame update
    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    void IDamageable.TakeDamage(int damage)
    {
        int damageTaken = Mathf.Clamp(damage, 0, currentHealth);
        currentHealth-=damageTaken;
        if(damageTaken!= 0)
        {
            OntakeDamage?.Invoke(damageTaken);
        }
        if(currentHealth==0 && damageTaken!=0)
        {
            OnDeath?.Invoke(transform.position);
        }
    }


}
