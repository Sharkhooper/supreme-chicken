using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Killable : MonoBehaviour
{
    public GameObject splashPrefab, lostText, model;
    private GameObject _character;

    private void Awake()
    {
        _character = gameObject;
    }

    public void GetKilled()
    {
        if (_character.CompareTag("Player"))
        {
            StartCoroutine(Losing());
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

    private IEnumerator Losing()
    {
        if(lostText != null)
            lostText.SetActive(true);
        if (splashPrefab != null)
        {
            GameObject splash = Instantiate(splashPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            splash.transform.localScale *= 0.5f;
        }
        if(model != null)
            model.SetActive(false);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // Lädt die InGame Szene neu
    }
}
