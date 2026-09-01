/******************************************************************************
 * Author: Brad Dixon
 * File Name: ProjectileBehaviour.cs
 * Creation Date: 9/1/2026
 * Last Modified: 9/1/2026
 * Brief: Handles damaging enemys when it hits them
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using System.Collections;

public class ProjectileBehaviour : MonoBehaviour
{
    int projectileDamage;
    public int ProjectileDamage
    {
        get => projectileDamage;
        set => projectileDamage = value;
    }
    [SerializeField] float lifeTime;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit something");
    }

    private void Start()
    {
        StartCoroutine(ProjectileLifeTime());
    }

    /// <summary>
    /// The max amount of time a projectile can exist before destroying itself
    /// </summary>
    /// <returns></returns>
    IEnumerator ProjectileLifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.gameObject);
    }
}
