/******************************************************************************
 * Author: Brad Dixon
 * File Name: PlayerBehaviour.cs
 * Creation Date: 8/31/2026
 * Brief: Handles player movement and input actions
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] PlayerInput pActions;
    InputAction move, interact, dash;
    Rigidbody rb;
    [Tooltip("How fast the player moves.")]
    [SerializeField] float moveSpeed;
    [SerializeField] float dashCooldownTime;
    bool canDash;

    #region Input Stuff
    /// <summary>
    /// Sets initial components and input action variables
    /// </summary>
    private void Awake()
    {
        pActions.currentActionMap.Enable();
        move = pActions.currentActionMap.FindAction("Movement");
        interact = pActions.currentActionMap.FindAction("Interact");
        dash = pActions.currentActionMap.FindAction("Dash");

        rb = GetComponent<Rigidbody>();
        Debug.Log("Here");
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
            Debug.Log("Dashing");
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
    /// Sets default variable values
    /// </summary>
    private void Start()
    {
        canDash = true;
    }

    /// <summary>
    /// Moves the player
    /// </summary>
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(move.ReadValue<Vector2>().x, 0, move.ReadValue<Vector2>().y) * moveSpeed;
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldownTime);
        canDash = true;
        Debug.Log("Dash is ready.");
    }
}
