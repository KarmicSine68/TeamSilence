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

            Debug.Log(dir.normalized);
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
        RandomlyRun();
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
