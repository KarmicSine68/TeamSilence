/******************************************************************************
 * Author: Brad Dixon
 * File Name: DummyBehaviour.cs
 * Creation Date: 9/1/2026
 * Last Modified: 9/4/2026
 * Brief: Testing script to test player damage
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;

public class DummyBehaviour : MonoBehaviour
{
    [SerializeField] int maxHealth;
    int currentHealth;
    EnemySpawner spawner;

   /// <summary>
   /// Sets dummy health to full
   /// </summary>
    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Sets a reference to the specific spawner that spawned this dummy
    /// </summary>
    /// <param name="spawnRef"></param>
    public void SetSpawnerReference(EnemySpawner spawnRef)
    {
        spawner = spawnRef;
    }

    /// <summary>
    /// Public call for the dummy to take damage
    /// </summary>
    /// <param name="damage"></param> how much damage the dummy takes
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Deletes the dummy when health reaches 0
    /// </summary>
    void Die()
    {
        spawner.RemoveEnemy();
        Destroy(this.gameObject);
    }
}
