/******************************************************************************
 * Author: Brad Dixon
 * File Name: WeaponBehaviour.cs
 * Creation Date: 9/1/2026
 * Last Modified: 9/1/2026
 * Brief: The basic class the weapons inherit from. Handles aiming 
 *        and attack calling
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WeaponBehaviour : PlayerInputHandler
{
    GameObject playerRef;
    FollowingCamera mainCam;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;
    [SerializeField] float attackCooldown;
    bool canAttack;
    bool pressingAttack;
    [SerializeField] int baseDamage;
    Vector3 bulletTrajectory;

    /// <summary>
    /// Gets references to other game objects
    /// </summary>
    override protected void Awake()
    {
        base.Awake();
        mainCam = Camera.main.GetComponent<FollowingCamera>();
        playerRef = FindAnyObjectByType<PlayerBehaviour>().gameObject;
        canAttack = true;
    }

    protected void OnEnable()
    {
        shoot.started += Shoot_performed;
        shoot.canceled += Shoot_canceled;
    }

    protected void OnDisable()
    {
        shoot.started -= Shoot_performed;
        shoot.canceled -= Shoot_canceled;
    }

    protected void Shoot_canceled(InputAction.CallbackContext obj)
    {
        pressingAttack = false;
    }

    protected void Shoot_performed(InputAction.CallbackContext obj)
    {
        pressingAttack = true;
    }

    /// <summary>
    /// How long a weapon has to wait before attacking again.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    /// <summary>
    /// Gets the mouse's position, relative to the player
    /// </summary>
    protected void FixedUpdate()
    {
        float camDistanceFromPlayer = Vector3.Distance(Camera.main.transform.position, playerRef.transform.position);
        Vector3 worldPos = aim.ReadValue<Vector2>();
        worldPos.z = Mathf.Abs(camDistanceFromPlayer);
        bulletTrajectory = Camera.main.ScreenToWorldPoint(worldPos - new Vector3(playerRef.transform.position.x,
            playerRef.transform.position.y)).normalized;

        Debug.Log(Camera.main.ScreenToWorldPoint(worldPos - new Vector3(playerRef.transform.position.x,
            playerRef.transform.position.y)).normalized);
        NormalizeDirection();

        if(pressingAttack && canAttack)
        {
            AttackWithWeapon();
        }
    }

    /// <summary>
    /// Alter the direction to make the bullet accurately line up with the mouse's positon
    /// </summary>
    void NormalizeDirection()
    {
        bulletTrajectory.y = 0;
        if (Mathf.Abs(bulletTrajectory.z) <= .8f) //Using .8 because the z tends to be an accurate metric up to this number
        {
            int negativeValue = bulletTrajectory.x < 0 ? -1 : 1;
            bulletTrajectory.x = (1 - Mathf.Abs(bulletTrajectory.z)) * negativeValue;
            Debug.Log(bulletTrajectory);
        }
        else
        {
            int negativeValue = bulletTrajectory.z < 0 ? -1 : 1;
            bulletTrajectory.z = (1 - Mathf.Abs(bulletTrajectory.x)) * negativeValue;
            Debug.Log(bulletTrajectory);
        }
    }

    /// <summary>
    /// Spawns a bullet and moves it where the player aims
    /// </summary>
    virtual protected void AttackWithWeapon()
    {
        GameObject bulletTemp = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bulletTemp.GetComponent<ProjectileBehaviour>().ProjectileDamage = baseDamage;

        bulletTemp.GetComponent<Rigidbody>().linearVelocity = bulletTrajectory * bulletSpeed;
        canAttack = false;
        StartCoroutine(AttackCooldown());
    }
}
