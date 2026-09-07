/******************************************************************************
 * Author: Brad Dixon
 * File Name: EnemyBehaviour.cs
 * Creation Date: 9/2/2026
 * Last Modified: 9/3/2026
 * Brief: Controls the enemy's actions
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour
{
    bool playerInRange;

    Rigidbody rb;
    GameObject playerRef;

    [SerializeField] int maxHealth;
    int currentHealth;

    [SerializeField] float moveSpeed;
    [SerializeField] float minRunTime;
    [SerializeField] float maxRunTime;
    float runTime;

    [SerializeField] Color attackColor;
    [SerializeField] Material enemyMaterial;
    [SerializeField] float delayBeforeAttack;

    [SerializeField] GameObject bulletProjectile;
    [SerializeField] float bulletSpeed;
    [SerializeField] int damage;

    [SerializeField] float coneDegree;
    [SerializeField] int projectileCount;
    [SerializeField] int burstCount;
    [SerializeField] float burstDelay;

    Vector3 bulletDir;

    EnemySpawner spawner;

    /// <summary>
    /// Sets variables and determines which state the enemy starts in
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerRef = FindAnyObjectByType<PlayerBehaviour>().gameObject;
        enemyMaterial = GetComponentInChildren<MeshRenderer>().material;
        currentHealth = maxHealth;
        if(playerInRange)
        {
            AttackPlayer();
        }
        else
        {
            MoveToPlayer();
        }
    }

    public void SetSpawnerReference(EnemySpawner spawnRef)
    {
        spawner = spawnRef;
    }

    /// <summary>
    /// Tells the enemy player is in range
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<PlayerBehaviour>())
        {
            playerInRange = true;
        }
    }

    /// <summary>
    /// Tells the enemy the player is no longer in range
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerBehaviour>())
        {
            playerInRange = false;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        spawner.RemoveEnemy();
        Destroy(this.gameObject);
    }

    /// <summary>
    /// State function for the enemy's move state
    /// </summary>
    void MoveToPlayer()
    {
        StartCoroutine(MoveUntilInRange());
    }

    /// <summary>
    /// Loops through until the enemy's trigger box detects the player
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveUntilInRange()
    {
        while(!playerInRange)
        {
            //Find direction the player is in
            Vector3 dir = transform.position - playerRef.transform.position;

            dir *= -1;

            //Move to player
            rb.linearVelocity = dir.normalized * moveSpeed;

            yield return new WaitForSeconds(.1f);
        }

        //Change state
        AttackPlayer();
    }

    /// <summary>
    /// Enemy stops moving and enters attack state
    /// </summary>
    void AttackPlayer()
    {
        rb.linearVelocity = Vector3.zero;
        StartCoroutine(AttackDelay());
    }

    /// <summary>
    /// Enemy waits x amount of time before starting their attack
    /// </summary>
    /// <returns></returns>
    IEnumerator AttackDelay()
    {
        //Visual to show that the enemy is going to attack
        Color originalColor = enemyMaterial.color;
        enemyMaterial.color = attackColor;
        yield return new WaitForSeconds(delayBeforeAttack);

        //Enemy starts attack
        enemyMaterial.color = originalColor;

        //Allows enemy to fire in multiple bursts
        for (int i = 0; i < burstCount; ++i)
        {
            SpawnCone();
            yield return new WaitForSeconds(burstDelay);
        }

        //Enter next state
        RandomlyRun();
    }

    /// <summary>
    /// Spawns the projectiles in a cone. How the enemy is able to shoot multiple projectiles at once
    /// </summary>
    void SpawnCone()
    {
        //Bullet's original trajectory. Used to build spread off of
        Vector3 originalDir = transform.position - playerRef.transform.position;

        originalDir *= -1;
        originalDir = originalDir.normalized;

        bulletDir = originalDir;

        //If projectile count is odd, fire the first bullet directly at player
        if(projectileCount % 2 == 1)
        {
            GameObject temp = Instantiate(bulletProjectile, transform.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().linearVelocity = bulletDir * bulletSpeed;
            temp.GetComponent<ProjectileBehaviour>().ProjectileDamage = damage;
        }

        float bulletSpread = 0;

        int j = 0;
        //Spreads the remaining bullets outwards by a set degree
        for (int i = projectileCount % 2; i < projectileCount; ++i)
        {
            bulletDir = originalDir;
            //Every two bullets, increase the spread degree
            if (j % 2 == 0)
            {
                bulletSpread += CalculateSpread();
            }
            GameObject temp = Instantiate(bulletProjectile, transform.position, Quaternion.identity);

            float r = bulletDir.x + bulletSpread;
            //If statements used to wrap the float value if it exceeds 1 or -1
            if(r > 1)
            {
                bulletDir.x = 1 - (r - 1);
            }
            else if(r < -1)
            {
                bulletDir.x = -1 - (r + 1);
            }
            else
            {
                bulletDir.x += bulletSpread;
            }
            r = bulletDir.z + bulletSpread;
            if(r > 1)
            {
                bulletDir.z = 1 - (r - 1);
            }
            else if(r < -1)
            {
                bulletDir.z = -1 - (r + 1);
            }
            else
            {
                bulletDir.z += bulletSpread;
            }

            NormalizeDirection();

            temp.GetComponent<Rigidbody>().linearVelocity = bulletDir.normalized * bulletSpeed;
            //Allows the spread to go in both directions
            bulletSpread *= -1;
            ++j;
        }
    }

    /// <summary>
    /// Alter the direction to make the bullet accurately line up with the player's position
    /// </summary>
    void NormalizeDirection()
    {
        bulletDir.y = 0;
        if (Mathf.Abs(bulletDir.z) <= .8f) //Using .8 because the z tends to be an accurate metric up to this number
        {
            int negativeValue = bulletDir.x < 0 ? -1 : 1;
            bulletDir.x = (1 - Mathf.Abs(bulletDir.z)) * negativeValue;
        }
        else
        {
            int negativeValue = bulletDir.z < 0 ? -1 : 1;
            bulletDir.z = (1 - Mathf.Abs(bulletDir.x)) * negativeValue;
        }
    }

    /// <summary>
    /// Converts the degree into a decimal point
    /// </summary>
    /// <returns></returns>
    float CalculateSpread()
    {
        return (coneDegree / 180);
    }

    /// <summary>
    /// Pick a random direction to run along
    /// </summary>
    void RandomlyRun()
    {
        //Randomize direction
        Vector3 randomDirection = Vector3.zero;
        randomDirection.x = Random.Range(-1, 1);
        randomDirection.z = Random.Range(-1, 1);

        rb.linearVelocity = randomDirection.normalized * moveSpeed;

        //This makes the time the enemy spends in the run state random
        runTime = Random.Range(minRunTime, maxRunTime);
        StartCoroutine(RunAwayTime());
    }

    /// <summary>
    /// Move to next state when done running
    /// </summary>
    /// <returns></returns>
    IEnumerator RunAwayTime()
    {
        yield return new WaitForSeconds(runTime);
        if(playerInRange)
        {
            AttackPlayer();
        }
        else
        {
            MoveToPlayer();
        }
    }
}
