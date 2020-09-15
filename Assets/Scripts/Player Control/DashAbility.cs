using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashAbility : MonoBehaviour
{

    [SerializeField] private Rigidbody _rB;
    [SerializeField] private float _dashCD = 0.5f; //in seconds
    private static float currentCD = 0f;
    

    public void HandleDashInput(InputAction.CallbackContext context)
    {
        Debug.Log("Dashing!");
        if (currentCD.Equals(0f))
        {
            currentCD += _dashCD;
            // dash
        }
    }
}
