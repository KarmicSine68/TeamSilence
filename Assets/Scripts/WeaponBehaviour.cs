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

public class WeaponBehaviour : PlayerInputHandler
{
    private void FixedUpdate()
    {
        Debug.Log(aim.ReadValue<Vector2>());
    }
}
