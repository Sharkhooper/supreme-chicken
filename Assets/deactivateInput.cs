using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class deactivateInput : MonoBehaviour
{
    private float xPos;
    private bool inputActivated = true;
    GameObject gO;
    private void Awake()
    {
        gO = gameObject;
        gO.GetComponent<PlayerInput>().DeactivateInput();
        inputActivated = false;
        xPos = gO.transform.position.x;
    }

    private void Update()
    {
        if (!inputActivated)
        {
            if (gO.transform.position.x > xPos + 3.0f)
            {
                gO.GetComponent<PlayerInput>().ActivateInput();
            }
        }
    }
}
