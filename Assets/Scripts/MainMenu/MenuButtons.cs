﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuButtons : MonoBehaviour
{
    private Vector3 m_lastMousePos;
    private Vector2 m_mouseDelta;
    public Camera m_camera;

    private bool m_animating;

    public Transform m_startButton;
    public Transform m_quitButton;
    
    private void Awake()
    {
        m_camera = Camera.main;
    }

    // Store mouse pos
    public void MousePos(InputAction.CallbackContext context)
    {
        Vector3 pos = context.ReadValue<Vector2>();
        pos.z = 1000;
        m_lastMousePos = m_camera.ScreenToWorldPoint(pos);
    }

    public void MouseClick()
    {
        // Determine which button was pressed if any
        RaycastHit hit;
        if (Physics.Raycast(m_camera.transform.position, m_lastMousePos - m_camera.transform.position, out hit, int.MaxValue) && !m_animating)
        {
            if(hit.transform == m_startButton)
            {
                m_animating = true;
                Animate(m_startButton);
                EventSystems.MainEventSystem.MainEvents.GameStarts();
            }
            
            else if(hit.transform == m_quitButton)
            {
                m_animating = true;
                Animate(m_quitButton);
            }
        }
    }

    private void Animate(Transform toAnimate)
    {
        //Vector3[] points = new Vector3[2];
        //points[0] = new Vector3(toAnimate.position.x, toAnimate.position.y, toAnimate.position.z + 0.06f);
        //points[1] = new Vector3(toAnimate.position.x, toAnimate.position.y, toAnimate.position.z - 0.06f);

        toAnimate.DOMove(new Vector3(toAnimate.position.x, toAnimate.position.y, toAnimate.position.z + 0.06f), 0.6f)
            .onComplete += () =>
            toAnimate.DOMove(new Vector3(toAnimate.position.x, toAnimate.position.y, toAnimate.position.z - 0.06f), 0.6f)
                .onComplete += () => m_animating = false;
    }
}
