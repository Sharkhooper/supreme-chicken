using System;
using System.Collections;
using System.Collections.Generic;
using Mainmenu;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MainMenu
{
    public class DiffBox : MonoBehaviour
    {
        private Vector3 m_lastMousePos;
        private Vector2 m_mouseDelta;
        public Camera m_camera;
        public DiffWheel m_wheel;

        private void Awake()
        {
            m_camera = Camera.main;
            m_wheel = FindObjectOfType<DiffWheel>();
        }

        public void MousePos(InputAction.CallbackContext context)
        {
            Vector3 pos = context.ReadValue<Vector2>();
            pos.z = 1000;
            m_lastMousePos = m_camera.ScreenToWorldPoint(pos);
        }

        public void MouseClick()
        {
            // Fancier movement
            RaycastHit hit;
             if (Physics.Raycast(m_camera.transform.position, m_lastMousePos - m_camera.transform.position, out hit, int.MaxValue))
             {
                 if(hit.transform == transform)
                 {
                    m_wheel.DifficultyClicked(transform.position);
                 }
             }
        }
    }
}
