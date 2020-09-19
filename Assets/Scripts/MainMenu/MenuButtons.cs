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

    public List<DiffBox> m_boxes;
    public Transform m_startButton;
    public Transform m_quitButton;
    public Transform m_door;
    public Transform m_pivot;
    public GameObject finishLine;

    private void Awake()
    {
        m_camera = Camera.main;
    }

    public void MouseClick()
    {
        Vector3 pos = Mouse.current.position.ReadValue();
        pos.z = 1000;
        m_lastMousePos = m_camera.ScreenToWorldPoint(pos);
        
        // Determine which button was pressed if any
        RaycastHit hit;
        if (Physics.Raycast(m_camera.transform.position, m_lastMousePos - m_camera.transform.position, out hit, int.MaxValue) && !m_animating)
        {
            if(hit.transform == m_startButton)
            {
                SlowMotionOverTime slowMo = GetComponent<SlowMotionOverTime>();
                if(slowMo != null)
                {
                    slowMo.StartScaling();
                }
                if(finishLine != null)
                {
                    finishLine.GetComponent<finishLine>().ResetTime();
                }

                m_animating = true;
                Animate(m_startButton);
                EventSystems.MainEventSystem.MainEvents.GameStarts();
                
                m_door.RotateAround(m_pivot.position, new Vector3(1, 0,0), -90);
                m_door.position = m_door.transform.forward * -1;
            }
            
            else if(hit.transform == m_quitButton)
            {
                m_animating = true;
                Animate(m_quitButton);
                EventSystems.MainEventSystem.MainEvents.GameQuit();
                Application.Quit();
            }
            else
            {
                for (int i = 0; i < m_boxes.Count; ++i)
                {
                    var box = m_boxes[i];
                    if (box.transform == hit.transform)
                    {
                        box.Click();
                    }
                }
            }
        }
    }

    private void Animate(Transform toAnimate)
    {
        //Vector3[] points = new Vector3[2];
        //points[0] = new Vector3(toAnimate.position.x, toAnimate.position.y, toAnimate.position.z + 0.06f);
        //points[1] = new Vector3(toAnimate.position.x, toAnimate.position.y, toAnimate.position.z - 0.06f);
        Vector3 backPos = toAnimate.transform.localPosition - new Vector3(0.1f,0,0);
        Vector3 fowPos = toAnimate.transform.localPosition;
        
        toAnimate.DOLocalMove(backPos, 0.6f)
            .onComplete += () =>
            toAnimate.DOLocalMove(fowPos, 0.6f)
                .onComplete += () => m_animating = false;
    }
}
