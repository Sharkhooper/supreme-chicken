using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using MainMenu;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuButtons : MonoBehaviour
{
    private Vector3 m_lastMousePos;
    private Vector2 m_mouseDelta;
    public Camera m_camera;

    private bool m_animating;

    public List<MenuItem> m_boxes;
    

    private MenuItem m_currentSelection;
    private Color m_selectionStartColor;
    
    private void Awake()
    {
        m_camera = Camera.main;
    }

    private void Start()
    {
        m_currentSelection = m_boxes[0];
        m_selectionStartColor = m_currentSelection.m_transformProperty.GetComponent<Renderer>().material.color;
        
        m_currentSelection.m_transformProperty.GetComponent<Renderer>().material.color = Color.red;
    }

    public void MouseClick()
    {
        Vector3 pos = Mouse.current.position.ReadValue();
        pos.z = 1000;
        m_lastMousePos = m_camera.ScreenToWorldPoint(pos);

        // Determine which button was pressed if any
        RaycastHit hit;
        if (Physics.Raycast(m_camera.transform.position, m_lastMousePos - m_camera.transform.position, out hit,
            int.MaxValue) && !m_animating)
        {
            for (int i = 0; i < m_boxes.Count; ++i)
            {
                var box = m_boxes[i];
                if (box.m_transformProperty == hit.transform)
                {
                    box.Click();
                    m_currentSelection.m_transformProperty.GetComponent<Renderer>().material.color = m_selectionStartColor;
                    m_currentSelection = m_boxes[i];
                    m_selectionStartColor = m_currentSelection.m_transformProperty.GetComponent<Renderer>().material.color;
                    m_currentSelection.m_transformProperty.GetComponent<Renderer>().material.color = Color.red;
                }
            }

        }
    }

    public void ControllerClick()
    {
        m_currentSelection.Click();
    }

    public void Move(InputAction.CallbackContext context)
    {
       var value = context.ReadValue<float>();
       if(value == 0)
           return;
       for (int i = 0; i < m_boxes.Count; ++i)
       {
           Debug.Log(i);
           Debug.Log("Val  " + value);
           var box = m_boxes[i];
           if (m_currentSelection.m_transformProperty == box.m_transformProperty)
           {
               if (i == m_boxes.Count - 1 && value > 0 ||
                   i == 0 && value < 0)
               {
                   Debug.Log("Abort");
                   return;
               }

               m_currentSelection.m_transformProperty.GetComponent<Renderer>().material.color = m_selectionStartColor;
               int index = value > 0? i + 1 : i - 1;
               m_currentSelection = m_boxes[index];
               m_selectionStartColor = m_currentSelection.m_transformProperty.GetComponent<Renderer>().material.color;
               m_currentSelection.m_transformProperty.GetComponent<Renderer>().material.color = Color.red;

               return;
           }
       }
    }
   
}
