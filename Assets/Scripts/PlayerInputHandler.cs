/******************************************************************************
 * Author: Brad Dixon
 * File Name: PlayerInputHandler.cs
 * Creation Date: 9/1/2026
 * Last Modified: 9/1/2026
 * Brief: Initializes player input. Inherited by scripts that deal with player movement
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    protected PlayerInput pActions;
    protected InputAction move, interact, dash;
    protected InputAction aim;

    /// <summary>
    /// Initializes the player input variables
    /// </summary>
    protected virtual void Awake()
    {
        pActions = GetComponent<PlayerInput>();
        pActions.currentActionMap.Enable();
        move = pActions.currentActionMap.FindAction("Movement");
        interact = pActions.currentActionMap.FindAction("Interact");
        dash = pActions.currentActionMap.FindAction("Dash");
        aim = pActions.currentActionMap.FindAction("Aim");
    }
}
