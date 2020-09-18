using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Mainmenu
{
    public class DiffWheel : MonoBehaviour
    {
        public Transform m_head;
        
        public List<Difficulty> m_difficulties;

        private Quaternion m_currentRot;
        private Vector3 m_currentDir;
        private bool m_animating;
        
        private void Awake()
        {
            m_currentRot = transform.rotation;
        }

        // Rotate the slider to the clicked difficulty box
        public void DifficultyClicked(Vector3 pos, int index)
        {
            var currentDir = (m_head.position - transform.position).normalized;
            var dir = (pos - transform.position).normalized;
            var angle = Mathf.Rad2Deg * (Mathf.Atan2(dir.y, dir.z) - Mathf.Atan2(currentDir.y, currentDir.z));
            
            // Make sure it doesn't rotate the wrong way
            angle = Mathf.Abs(angle) < 170 || angle < -170 ? angle * -1 : angle;
            
            if(Mathf.Abs(angle) > 20 && !m_animating)
            {
                Difficulty.current = m_difficulties[index];
                m_currentRot *= Quaternion.Euler(0, angle,0);
                m_animating = true;
                transform.DORotateQuaternion(m_currentRot, 0.3f).onComplete += () => m_animating = false;
            }
        }
       /* public void MouseClick()
        {
            // Fancier movement
            RaycastHit hit;
            if (Physics.Raycast(m_camera.transform.position, m_lastMousePos - m_camera.transform.position, out hit, int.MaxValue))
            {
                if(hit.transform == transform)
                {
                    // Determine which direction to turn by comparing angles
                    var dir = (m_difficultySettings[2].position - transform.position).normalized;
                    var currentDir = (m_head.position - transform.position).normalized;

                    // Turn knob
                    var angle = Mathf.Rad2Deg * (Mathf.Atan2(dir.y, dir.x) - Mathf.Atan2(currentDir.y, currentDir.x));
                    var rot = Quaternion.Euler(0, angle, 0);
                    m_currentRot *= rot;
                    transform.rotation = m_currentRot;
                }
            }
            
            
        }*/
    }
}