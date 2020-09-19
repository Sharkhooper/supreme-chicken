using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerOnHit : MonoBehaviour
{
    public bool shouldKill = true;

    [SerializeField] private SoundMap sounds;

    private void OnTriggerEnter(Collider other)
    {
        if (shouldKill && other.gameObject.layer == 10 && other.TryGetComponent<Killable>(out Killable k)) {
            k.GetKilled();
            if (sounds != null) {
                sounds.SpawnSource(k.transform);
            }
        }
    }
}
