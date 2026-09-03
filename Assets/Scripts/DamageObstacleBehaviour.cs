/******************************************************************************
 * Author: Brad Dixon
 * File Name: DamageObstacleBehaviour.cs
 * Creation Date: 9/1/2026
 * Last Modified: 9/1/2026
 * Brief: Testing script to test the player's ability to dash through damage.
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;

public class DamageObstacleBehaviour : MonoBehaviour
{
    [SerializeField] int damage;

    /// <summary>
    /// Player takes damage on contact
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<PlayerBehaviour>())
        {
            other.GetComponentInParent<PlayerBehaviour>().TakeDamage(damage);
        }
    }
}
