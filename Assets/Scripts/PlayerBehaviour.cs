/******************************************************************************
 * Author: Brad Dixon
 * File Name: PlayerBehaviour.cs
 * Creation Date: 8/31/2026
 * Last Modified: 9/1/2026
 * Brief: Handles player movement and input actions
 * External Resources: https://discussions.unity.com/t/camera-relative-movement/763440/4
 * ***************************************************************************/
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviour : PlayerInputHandler
{
    //PlayerInput pActions;
    //InputAction move, interact, dash;
    Rigidbody rb;
    [Tooltip("How fast the player moves.")]
    [SerializeField] float moveSpeed;
    [Tooltip("How long the player must wait between dashes.")]
    [SerializeField] float dashCooldownTime;
    bool canDash;
    [Tooltip("How far the player dashes.")]
    [SerializeField] float dashForce;

    [SerializeField] int maxHealth;
    int currentHealth;

    [SerializeField] float knockbackForce;

    [SerializeField] GameObject playerModel;

    bool invincible, alive;

    [Tooltip("How long a player is invinvible for after taking damage.")]
    [SerializeField] float invinvibilityTimeWhenHit;

    [Tooltip("How long a player is invincibile for when dashing.")]
    [SerializeField] float invincibilityTimeWhenDashing;

    #region Input Stuff
    /// <summary>
    /// Sets initial components and input action variables
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        canDash = true;
        alive = true;
        invincible = false;
    }

    /// <summary>
    /// Enables Input Action events
    /// </summary>
    private void OnEnable()
    {
        interact.started += Interact_started;
        dash.started += Dash_started;
    }

    /// <summary>
    /// Player dashes if able to
    /// </summary>
    /// <param name="obj"></param>
    private void Dash_started(InputAction.CallbackContext obj)
    {
        if(canDash)
        {
            PlayerDash();
            canDash = false;
            StartCoroutine(DashCooldown());
        }
        else
        {
            Debug.Log("Dash not ready yet.");
        }
    }

    /// <summary>
    /// Code for when player presses the interact button
    /// </summary>
    /// <param name="obj"></param>
    private void Interact_started(InputAction.CallbackContext obj)
    {
        Debug.Log("Interacting");
    }

    /// <summary>
    /// Disables the input system events so there aren't duplicates
    /// </summary>
    private void OnDisable()
    {
        pActions.currentActionMap.Disable();
        interact.started -= Interact_started;
        dash.started -= Dash_started;
    }
    #endregion

    /// <summary>
    /// Moves the player
    /// </summary>
    private void FixedUpdate()
    {
        if (alive)
        {
            Vector3 xMove = move.ReadValue<Vector2>().x * Camera.main.transform.right;
            Vector3 zMove = move.ReadValue<Vector2>().y * Camera.main.transform.forward;
            rb.linearVelocity = (xMove + zMove) * moveSpeed;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            rb.linearVelocity = Quaternion.Euler(0, Camera.main.transform.rotation.y, 0) * rb.linearVelocity;
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Once the player dashes, x amount of time must elapse before they can dash again
    /// </summary>
    /// <returns></returns>
    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldownTime);
        canDash = true;
        Debug.Log("Dash is ready.");
    }

    /// <summary>
    /// Dashes the player in the direction they are moving
    /// </summary>
    private void PlayerDash()
    {
        invincible = true;
        Debug.Log("I'M INVINCIBLE");
        //If player is stationary, dash to the right
        if (rb.linearVelocity == Vector3.zero)
        {
            rb.AddForce(Vector3.right * dashForce * moveSpeed, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(rb.linearVelocity * dashForce, ForceMode.Impulse);
        }
        StartCoroutine(ITime(invincibilityTimeWhenDashing));
    }

    /// <summary>
    /// Public call for when the player takes damage
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        if (!invincible)
        {
            invincible = true;
            StartCoroutine(ITime(invinvibilityTimeWhenHit));
            currentHealth -= damage;
            TakeKnockback();

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    /// <summary>
    /// Direction knockback
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="attackDir"></param>
    public void TakeDamage(int damage, Vector3 attackDir)
    {
        if (!invincible)
        {
            invincible = true;
            StartCoroutine(ITime(invinvibilityTimeWhenHit));
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        rb.AddForce(attackDir, ForceMode.Impulse);
    }

    /// <summary>
    /// When the player's health reaches 0
    /// </summary>
    void Die()
    {
        alive = false;
        playerModel.SetActive(false);
    }

    /// <summary>
    /// Player gets knocked back when taking damage
    /// </summary>
    void TakeKnockback()
    {
        Vector3 knockbackDirection;
        if (rb.linearVelocity == Vector3.zero)
        {
            knockbackDirection = Vector3.left * moveSpeed * knockbackForce;
        }
        else
        {
            knockbackDirection = rb.linearVelocity * -1 * knockbackForce;
        }
        rb.AddForce(knockbackDirection, ForceMode.Impulse);
    }

    /// <summary>
    /// How long the player has before they are no longer invinvible
    /// </summary>
    /// <returns></returns>
    IEnumerator ITime(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("I'M MORTAL!!!");
        invincible = false;
    }
}
