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
            Debug.Log("Killed Player!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // Lädt die InGame Szene neu
        }
        else if(_character.GetComponent<PlateController>() != null)
        {
            _character.GetComponent<PlateController>().Explode();
        }
        else
        {
            if(splashPrefab != null)
                Instantiate(splashPrefab, transform.position + new Vector3(0,1,0), Quaternion.identity);
            Destroy(_character.transform.parent.gameObject);
            // Trigger Death Animation
        }
    }
}
