using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundMap", menuName = "ScriptableObjects/SoundMap", order = 1)]
public class SoundMap : ScriptableObject {
    [System.Serializable]
    public struct Sound {
        public AudioClip clip;
        public float volume;
    }
    [SerializeField] private Sound[] sounds;

    public Sound Get() {
        return sounds[Random.Range(0, sounds.Length)];
    }

    public void SpawnSource(Transform origin) {
        var sound = Get();
        GameObject go = new GameObject();
        go.transform.position = origin.position;
        var src = go.AddComponent<AudioSource>();
        src.clip = sound.clip;
        src.volume = sound.volume;
        src.Play();
        Destroy(go, sound.clip.length + Time.fixedDeltaTime);
    }

    public void Play(AudioSource source) {
        Sound s = Get();
        source.clip = s.clip;
        source.volume = s.volume;
        source.Play();
    }
}
