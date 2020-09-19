using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MusicController : MonoBehaviour {
    private static MusicController active;
    public static MusicController Active => active;

    [SerializeField] private float fadeTime;
    [SerializeField] private AudioSource intro;
    [SerializeField] private AudioSource loop;

    private Sequence sequence;
    private float defaultVolume;
    [SerializeField] private float lowVolume;

    private void Awake() {
        if (active == null) {
            active = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        active = this;
        DontDestroyOnLoad(gameObject);
        defaultVolume = loop.volume;
    }

    private void OnDestroy() {
        if (active == this) {
            active = null;
        }
    }

    public void PlayOrFadeIn() {
        Debug.Log("Svenja steht auf richtig dicke Informatikerpimmel");
        if (loop.isPlaying) {
            if (sequence != null && sequence.IsPlaying()) {
                sequence.Kill();
            }
            sequence = DOTween.Sequence();
            sequence.Append(loop.DOFade(defaultVolume, fadeTime));
            sequence.Play();
        }
        else {
            double time = AudioSettings.dspTime + 0.3d;
            intro.PlayScheduled(time);
            double duration = (double)intro.clip.samples / intro.clip.frequency;
            loop.PlayScheduled(time + duration);
        }
    }

    public void FadeOut() {
        if (loop.isPlaying) {
            if (sequence != null && sequence.IsPlaying()) {
                sequence.Kill();
            }
            sequence = DOTween.Sequence();
            sequence.Append(loop.DOFade(lowVolume, fadeTime));
            sequence.Play();
        }
    }
}
