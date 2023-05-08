using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    // The time in seconds before the shell is removed
    public float m_MaxLifeTime = 2f;

    // The amount of damage done is the explosion is centered on a tank
    public float m_MaxDamage = 34f;

    // The maximum distance away from the explosion tanks can be and are affected
    public float m_ExplosionRadius = 5;

    // The amount of force added to a tank at the center of the explosion
    public float m_ExplosionForce = 200f;


    // Reference to the particles that will play on the explosion
    public ParticleSystem m_ExplosionParticles;

    // Use this for initialization
    private void Start()
    {
        // IF it itsn't destroyed then, distroy the shell after it's lifetime
        Destroy(gameObject, m_MaxLifeTime);
    }

    private void OnCollisionEnter(Collision Other)
    {
        // Find the rigidbody of the collision object
        Rigidbody targetRigidbody = Other.gameObject.GetComponent<Rigidbody>();

        // only tanks will have rigidbody scripts 
        if (targetRigidbody != null)
        {
            // Add an explosion force 
            targetRigidbody.AddExplosionForce(m_ExplosionForce,
                            transform.position, m_ExplosionRadius);

            // find the TankHealth script associated with the rigidbody 
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            if (targetHealth != null)
            {
                // Calculate the amount of damage the target should take  
                // based on it's distance from the shell. 
                float damage = CalculateDamage(targetRigidbody.position);

                // Deal this damage to the tank 
                targetHealth.TakeDamage(damage);
            }
        }

        // Unparent the particles from the shell 
        m_ExplosionParticles.transform.parent = null;

        // Play the particle system 
        m_ExplosionParticles.Play();

        // Once the particles have finished, destroy the gameObject they are on 
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);

        // Destroy the shell 
        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        // Create a vector from the shell to the target 
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the shell to the target 
        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius)    
        // the target is away 
        float relativeDistance =
              (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Calculate damage as this proportion of the maximum possible damage 
        float damage = relativeDistance * m_MaxDamage;

        // Make sure that the minimum damage is always 0 
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}

