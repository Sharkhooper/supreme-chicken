using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashAbility : MonoBehaviour
{

    [SerializeField] private Rigidbody _rB;
    [SerializeField] private float _dashCD = 1.0f; //in seconds
    [SerializeField] private float dashDistance = 5.0f;
    private GameObject player;
    private static float currentCD = 0f;


    public void HandleDashInput(InputAction.CallbackContext context)
    {
        if (currentCD <= 0f)
        {
            Vector3 playerPosition = player.transform.position;
            currentCD = _dashCD;
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Debug.Log("MousePosition: " + mousePos);
            Vector3 toMouse = mousePos - new Vector3(playerPosition.x, playerPosition.y, 0);
            // dash towards mouse with normalized Vector * dashDistance
        }
    }

    private void Awake()
    {
        player = gameObject;
    }

    private void Update()
    {
        currentCD -= Time.deltaTime;
    }
}
