/******************************************************************************
 * Author: Brad Dixon
 * File Name: ProjectileBehaviour.cs
 * Creation Date: 9/1/2026
 * Last Modified: 9/2/2026
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

    /// <summary>
    /// What happens when a bullet touches something
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<DummyBehaviour>())
        {
            other.GetComponent<DummyBehaviour>().TakeDamage(projectileDamage);
        }

        //Bullet should destroy when hitting anything
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Starts the timer for how long bullets should be alive
    /// </summary>
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
