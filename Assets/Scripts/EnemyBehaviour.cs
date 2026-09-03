/******************************************************************************
 * Author: Brad Dixon
 * File Name: EnemyBehaviour.cs
 * Creation Date: 9/2/2026
 * Last Modified: 9/2/2026
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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerRef = FindAnyObjectByType<PlayerBehaviour>().gameObject;
        enemyMaterial = GetComponentInChildren<MeshRenderer>().material;
        if(playerInRange)
        {
            AttackPlayer();
        }
        else
        {
            MoveToPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<PlayerBehaviour>())
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerBehaviour>())
        {
            playerInRange = false;
        }
    }

    void MoveToPlayer()
    {
        StartCoroutine(MoveUntilInRange());
    }

    IEnumerator MoveUntilInRange()
    {
        while(!playerInRange)
        {
            Vector3 dir = transform.position - playerRef.transform.position;

            dir *= -1;

            rb.linearVelocity = dir.normalized * moveSpeed;

            yield return new WaitForSeconds(.1f);
        }
        AttackPlayer();
    }

    void AttackPlayer()
    {
        rb.linearVelocity = Vector3.zero;
        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        Color originalColor = enemyMaterial.color;
        enemyMaterial.color = attackColor;
        yield return new WaitForSeconds(delayBeforeAttack);
        enemyMaterial.color = originalColor;

        for (int i = 0; i < burstCount; ++i)
        {
            SpawnCone();
            yield return new WaitForSeconds(burstDelay);
        }
        RandomlyRun();
    }

    void SpawnCone()
    {
        Vector3 originalDir = transform.position - playerRef.transform.position;

        originalDir *= -1;
        originalDir = originalDir.normalized;

        bulletDir = originalDir;

        if(projectileCount % 2 == 1)
        {
            GameObject temp = Instantiate(bulletProjectile, transform.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().linearVelocity = bulletDir * bulletSpeed;
            temp.GetComponent<ProjectileBehaviour>().ProjectileDamage = damage;
        }

        float bulletSpread = 0;

        int j = 0;
        for (int i = projectileCount % 2; i < projectileCount; ++i)
        {
            bulletDir = originalDir;
            if (j % 2 == 0)
            {
                bulletSpread += CalculateSpread();
            }
            GameObject temp = Instantiate(bulletProjectile, transform.position, Quaternion.identity);

            float r = bulletDir.x + bulletSpread;
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

            //NormalizeDirection();

            temp.GetComponent<Rigidbody>().linearVelocity = bulletDir.normalized * bulletSpeed;
            bulletSpread *= -1;
            ++j;
        }
    }

    /// <summary>
    /// Alter the direction to make the bullet accurately line up with the mouse's positon
    /// </summary>
    void NormalizeDirection()
    {
        bulletDir.y = 0;
        if (Mathf.Abs(bulletDir.z) <= .8f) //Using .8 because the z tends to be an accurate metric up to this number
        {
            int negativeValue = bulletDir.x < 0 ? -1 : 1;
            bulletDir.x = (1 - Mathf.Abs(bulletDir.z)) * negativeValue;
            //Debug.Log(bulletTrajectory);
        }
        else
        {
            int negativeValue = bulletDir.z < 0 ? -1 : 1;
            bulletDir.z = (1 - Mathf.Abs(bulletDir.x)) * negativeValue;
            //Debug.Log(bulletTrajectory);
        }
    }

    float CalculateSpread()
    {
        return (coneDegree / 180);
    }

    void RandomlyRun()
    {
        Vector3 randomDirection = Vector3.zero;
        randomDirection.x = Random.Range(-1, 1);
        randomDirection.z = Random.Range(-1, 1);

        rb.linearVelocity = randomDirection.normalized * moveSpeed;

        runTime = Random.Range(minRunTime, maxRunTime);
        StartCoroutine(RunAwayTime());
    }

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
