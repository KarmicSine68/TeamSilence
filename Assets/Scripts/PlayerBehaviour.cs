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

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] PlayerInput pActions;
    InputAction move, interact, dash;
    Rigidbody rb;
    [Tooltip("How fast the player moves.")]
    [SerializeField] float moveSpeed;

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
    }

    private void Interact_started(InputAction.CallbackContext obj)
    {

    }

    /// <summary>
    /// Moves the player
    /// </summary>
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(move.ReadValue<Vector2>().x, 0, move.ReadValue<Vector2>().y) * moveSpeed;
    }
}
