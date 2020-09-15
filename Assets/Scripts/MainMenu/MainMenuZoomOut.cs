using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class MainMenuZoomOut : MonoBehaviour
{
    private CinemachineTrackedDolly m_cam;
    void Start()
    {
        EventSystems.MainEventSystem.MainEvents.onGameStarts += GameStarts;
        m_cam = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    private void GameStarts()
    {
        // Animate the cam and its rotation
        transform.DORotate(new Vector3(0, transform.rotation.eulerAngles.y + 90, 0), 1);

        DOTween.To(() => m_cam.m_PathPosition, x => m_cam.m_PathPosition = x, 2, 2);
    }
}
