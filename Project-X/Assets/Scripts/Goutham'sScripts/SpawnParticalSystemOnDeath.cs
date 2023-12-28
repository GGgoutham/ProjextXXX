using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(IDamageable))]
public class SpawnParticalSystemOnDeath : MonoBehaviour
{
    public IDamageable Damageable;

    [SerializeField] private ParticleSystem DeathParticleSystem;

    private void Awake()
    {
        Damageable = GetComponent<IDamageable>();

    }
    

    private void OnEnable()
    {
        Damageable.OnDeath += Damageable_OnDeath;
    }

    private void Damageable_OnDeath(Vector3 position)
    {
        Instantiate(DeathParticleSystem, position, Quaternion.identity);
        gameObject.SetActive(false);
    }


}
