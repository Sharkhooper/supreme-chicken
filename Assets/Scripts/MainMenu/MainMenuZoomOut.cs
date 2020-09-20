using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class MainMenuZoomOut : MonoBehaviour
{
    private PlayableDirector m_cam;
    public Transform m_door;
    void Start()
    {
        EventSystems.MainEventSystem.MainEvents.onGameStarts += GameStarts;
        m_cam = GetComponent<PlayableDirector>();
    }

    private void GameStarts()
    {
        // Animate the cam and its rotation
        m_cam.Play();
        StartCoroutine(AllowReload());
        
    }

    private IEnumerator AllowReload() {
        yield return new WaitForSecondsRealtime((float)m_cam.duration);
        GameManager.Current.AllowReload = true;
    }
}
