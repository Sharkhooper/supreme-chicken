using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Killable : MonoBehaviour
{
    public GameObject splashPrefab;
    private GameObject _character;

    private void Awake()
    {
        _character = gameObject;
    }

    public void GetKilled()
    {
        if (_character.CompareTag("Player"))
        {
           // SceneManager.LoadScene("Game");  // Lädt die InGame Szene neu
        }
        else
        {
            StartCoroutine(PlayAnim());
            Destroy(_character.transform.parent.gameObject);
            // Trigger Death Animation
        }
    }

    private IEnumerator PlayAnim()
    {
        GameObject splash = Instantiate(splashPrefab, transform.position, Quaternion.identity);
        float secondsToWait = splash.GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(secondsToWait);
    }
}
