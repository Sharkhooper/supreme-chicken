using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DismemberSound : MonoBehaviour
{
	private AudioSource[] src;
	private int currentSource = 0;
	[SerializeField] private SoundMap sounds;
	[SerializeField] private int sources;
	[SerializeField] private LayerMask layers = ~0;

	private void Awake() {
		src = new AudioSource[sources];
		for (int i = 0; i < sources; ++i) {
			src[i] = gameObject.AddComponent<AudioSource>();
			src[i].spatialBlend = sounds.SpatialBlend;
		}
	}
	private void OnCollisionEnter(Collision collision) {
		if (src == null) return;

		var otherLayer = collision.transform.gameObject.layer;
		if ((1 << otherLayer & layers.value) == 0) return;

		var source = src[currentSource];
		if (source.isPlaying) return;

		currentSource = (1 + currentSource) % src.Length;
		var sound = sounds.Get();
		source.clip = sound.clip;
		source.volume = sound.volume;
		source.Play();
	}
}
