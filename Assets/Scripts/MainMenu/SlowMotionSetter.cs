using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class SlowMotionSetter : MonoBehaviour
{
    [SerializeField] private float m_timeScale = 1.0f;

    private float m_lastTime;
    public PlayableDirector m_director;
    private void Start()
    {
        m_director = GetComponent<PlayableDirector>();
        m_lastTime = m_timeScale;
    }

    private void Update()
    {
        if (m_timeScale > m_lastTime * 0.01f || m_timeScale < m_lastTime * 0.01f)
        {
            SetTimeScale();
        }

        m_lastTime = m_timeScale;
    }

    public void SetTimeScale()
    {
        m_director.playableGraph.GetRootPlayable(0).SetSpeed(m_timeScale);
    }
}
