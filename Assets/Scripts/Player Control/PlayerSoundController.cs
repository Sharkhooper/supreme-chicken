using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour {
    [SerializeField] private AudioClip attack;
    [SerializeField] private AudioClip dash;
    [SerializeField] private AudioClip death;
    [SerializeField] private AudioClip scream;
    [SerializeField] private AudioClip scream2;
    [SerializeField] private AudioClip walking;

    [SerializeField] [Range(0,1)] private float volume = 1;
    [SerializeField] private float fadeTime = 0.2f;

    private AudioSource source;
    private AudioSource source2;

    private Sequence walkSequence;
    private bool isWalking;

    public AudioClip Death => death;
    public float Volume => volume;

    private void Start() {
        source = gameObject.AddComponent<AudioSource>();
        source2 = gameObject.AddComponent<AudioSource>();
        source2.clip = walking;

        source.volume = volume;
        source2.volume = 0;
        source2.loop = true;
        source2.Play();

        walkSequence = DOTween.Sequence();
    }

    public void Attack(float duration = 0.4f) {
        //duration = attack.length - 0.15f;
        Play(attack);
        DampWalking(duration);
    }

    public void Die() {
        StopWalk();
        Play(death);
    }

    public void Dash(float duration = 0.2f) {
        //duration = dash.length - 0.15f;
        Play(dash);
        DampWalking(duration);
    }

    public void Jump() {
        Play(dash);
        DampWalking(dash.length - 0.2f);
    }

    public void StartWalk() {
        if (isWalking) return;
        isWalking = true;
        CheckSequence();
        walkSequence = DOTween.Sequence();
        walkSequence.Append(source2.DOFade(volume, fadeTime));
        walkSequence.Play();
    }

    public void StopWalk() {
        if (!isWalking) return;
        CheckSequence();
        isWalking = false;
        walkSequence.Append(source2.DOFade(0, fadeTime));
        walkSequence.Play();
    }

    private void DampWalking(float time) {
        CheckSequence();
        walkSequence = DOTween.Sequence();
        walkSequence.Append(source2.DOFade(0, fadeTime));
        walkSequence.AppendInterval(Mathf.Max(0, time - 2 * time));
        walkSequence.onComplete += () => {
            if (isWalking) {
                walkSequence = DOTween.Sequence();
                walkSequence.Append(source2.DOFade(volume, fadeTime));
                walkSequence.Play();
            }
        };
        walkSequence.Play();
    }

    private void CheckSequence() {
        //if (walkSequence.IsPlaying()) {
            walkSequence.Kill();
        //}
    }

    private bool Play(AudioClip clip) {
        if (source.isPlaying) return false;
        source.clip = clip;
        source.Play();
        return true;
    }
}
