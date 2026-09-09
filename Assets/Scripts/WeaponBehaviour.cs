/******************************************************************************
 * Author: Brad Dixon
 * File Name: WeaponBehaviour.cs
 * Creation Date: 9/1/2026
 * Last Modified: 9/2/2026
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
    float _camRotation;
    float _camDistance;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;
    [SerializeField] float attackCooldown;
    bool canAttack;
    bool pressingAttack;
    [SerializeField] int baseDamage;
    Vector3 aimTrajectory;
    [Tooltip("The percentage in which the bullets should deviate by in their trajectory")]
    [SerializeField] float bulletDeviation;


    Vector3 lookAtDirection;

    /// <summary>
    /// Gets references to other game objects
    /// </summary>
    override protected void Awake()
    {
        base.Awake();
        playerRef = FindAnyObjectByType<PlayerBehaviour>().gameObject;
        mainCam = Camera.main.GetComponent<FollowingCamera>();
        _camRotation = mainCam.transform.eulerAngles.y;
        _camDistance = Vector3.Distance(Camera.main.transform.position, playerRef.transform.position);
        canAttack = true;
    }

    /// <summary>
    /// Enables the player input functions
    /// </summary>
    protected void OnEnable()
    {
        shoot.started += Shoot_performed;
        shoot.canceled += Shoot_canceled;
    }

    /// <summary>
    /// Disables player input functions to avoid duplicates on reload
    /// </summary>
    protected void OnDisable()
    {
        shoot.started -= Shoot_performed;
        shoot.canceled -= Shoot_canceled;
    }

    /// <summary>
    /// Tells the code that the player isn't pressing the attack button anymore
    /// </summary>
    /// <param name="obj"></param>
    protected void Shoot_canceled(InputAction.CallbackContext obj)
    {
        pressingAttack = false;
    }

    /// <summary>
    /// Tells the code that the player is pressing the attack button
    /// </summary>
    /// <param name="obj"></param>
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
        LookingDirection();
        RotatePlayer();


        //aimTrajectory = Camera.main.ScreenToWorldPoint(worldPos) - new Vector3(playerRef.transform.position.x,
        //    playerRef.transform.position.y);

        //aimTrajectory.z -= playerRef.transform.position.z;
        //aimTrajectory = aimTrajectory.normalized;



        //bulletTrajectory.z -= playerRef.transform.position.z;
        //bulletTrajectory = bulletTrajectory.normalized;
        //Debug.Log(bulletTrajectory);
        //NormalizeDirection();

        if (pressingAttack && canAttack)
        {
            AttackWithWeapon();
        }
    }

    /// <summary>
    /// Calculate direction the player should be facing based on position of the mouse
    /// for rotation and bullet accuracy purposes
    /// </summary>
    void LookingDirection() 
    {
        Vector3 screenPosition = new(aim.ReadValue<Vector2>().x, aim.ReadValue<Vector2>().y, _camDistance);

        lookAtDirection = Vector3.Scale(Camera.main.ScreenToWorldPoint(screenPosition) - playerRef.transform.position, new Vector3(1, 0, 1));
    }

    /// <summary>
    /// Rotate the forwards of the player to face direction of pointer while taking into account the camera rotation
    /// </summary>
    void RotatePlayer() 
    {
        Vector3 lookAtEuler = Quaternion.LookRotation(lookAtDirection).eulerAngles;

        //Ensure no unwanted rotations & account for existing camera rotation
        //lookAtEuler.x = lookAtEuler.z = 0;
        lookAtEuler.y -= _camRotation;

        playerRef.transform.eulerAngles = lookAtEuler;
    }

    /// <summary>
    /// Alter the direction to make the bullet accurately line up with the mouse's positon
    /// </summary>
    void NormalizeDirection()
    {
        aimTrajectory.y = 0;
        if (Mathf.Abs(aimTrajectory.z) <= .8f) //Using .8 because the z tends to be an accurate metric up to this number
        {
            int negativeValue = aimTrajectory.x < 0 ? -1 : 1;
            aimTrajectory.x = (1 - Mathf.Abs(aimTrajectory.z)) * negativeValue;
            //Debug.Log(bulletTrajectory);
        }
        else
        {
            int negativeValue = aimTrajectory.z < 0 ? -1 : 1;
            aimTrajectory.z = (1 - Mathf.Abs(aimTrajectory.x)) * negativeValue;
            //Debug.Log(bulletTrajectory);
        }
    }

    /// <summary>
    /// Adds trajectory deviation to the bullet
    /// </summary>
    void AddDeviation()
    {
        //Convert bullet into percentage if not already
        if(bulletDeviation > 1)
        {
            bulletDeviation /= 100f;
        }

        aimTrajectory.x += Random.Range(-bulletDeviation, bulletDeviation);
        aimTrajectory.z += Random.Range(-bulletDeviation, bulletDeviation);
    }

    /// <summary>
    /// Spawns a bullet and moves it where the player aims
    /// </summary>
    virtual protected void AttackWithWeapon()
    {
        GameObject bulletTemp = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bulletTemp.GetComponent<ProjectileBehaviour>().Init(baseDamage, lookAtDirection.normalized * bulletSpeed);

        AddDeviation();

        canAttack = false;
        StartCoroutine(AttackCooldown());
    }
}
