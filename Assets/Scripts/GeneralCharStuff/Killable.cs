using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Killable : MonoBehaviour
{

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
            Destroy(_character);
        }
    }
}
