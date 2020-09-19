using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Killable : MonoBehaviour
{
    public GameObject splashPrefab, deadChickenPrefab, lostText, model;
    private GameObject _character;
    private bool dead;

    private void Awake()
    {
        _character = gameObject;
    }

    public void GetKilled()
    {
        if (dead) return;
        dead = true;
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

            Dismemberment d = GetComponent<Dismemberment>();
            if (d != null) {
                Destroy(_character.transform.parent.gameObject);
                Vector3 direction = d.Origin.position - MovementController.Active.transform.position;
                direction.y *= 0.15f;
                direction.Normalize();
                d.DismemberWithForce(direction);
            }
            else {
                Destroy(_character.transform.parent.gameObject);
            }
        }
    }

    private IEnumerator Losing()
    {
        Debug.Log("Losing Text = " + lostText.name);
        if(lostText != null)
        {
            Debug.Log("Set LostText");
            lostText.SetActive(true);
        }
        if (splashPrefab != null)
        {
            GameObject splash = Instantiate(splashPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            splash.transform.localScale *= 0.5f;
            Debug.Log("Test");
        }
        if(model != null)
            model.SetActive(false);

        GameObject deadChick = Instantiate(deadChickenPrefab, transform.position, Quaternion.identity);
        Rigidbody rb = deadChick.GetComponent<Rigidbody>();
        AudioSource src = deadChick.AddComponent<AudioSource>();
        src.clip = MovementController.Active.Sound.Death;
        src.volume = MovementController.Active.Sound.Volume;
        rb.AddForce(Vector3.up * 200);
        rb.AddTorque(new Vector3(UnityEngine.Random.Range(10, 180), UnityEngine.Random.Range(10, 180), UnityEngine.Random.Range(10, 180)));
        GetComponent<BoxCollider>().enabled = false;
        GetComponentInParent<PlayerInput>().enabled = false;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // Lädt die InGame Szene neu
    }
}
