using DG.Tweening;
using EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicEvent : MonoBehaviour
{
    [SerializeField] private float delay = 0;
    private bool done = false;

    private void Start() {
        MainEventSystem.MainEvents.onGameStarts += StartMusic;
    }

    private void StartMusic() {
        if (done) return;
        done = true;
        if (delay > 0) {
            Sequence s = DOTween.Sequence();
            s.AppendInterval(delay);
            s.onComplete += () => {
                MusicController.Active.PlayOrFadeIn();
            };
            s.Play();
        }
        else {
            MusicController.Active.PlayOrFadeIn();
        }
    }

}
