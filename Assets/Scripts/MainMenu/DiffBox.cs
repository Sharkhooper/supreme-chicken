using System;
using System.Collections;
using System.Collections.Generic;
using Mainmenu;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MainMenu
{
    public class DiffBox : MonoBehaviour, IClickable
    {
        public int m_index;
        
        private Vector3 m_lastMousePos;
        private Vector2 m_mouseDelta;
        private Camera m_camera;
        private DiffWheel m_wheel;
        
        private void Start()
        {
            m_camera = Camera.main;
            m_wheel = FindObjectOfType<DiffWheel>();
        }

        public void Click()
        {
            m_wheel.DifficultyClicked(transform.position, m_index);
        }
    }
}
