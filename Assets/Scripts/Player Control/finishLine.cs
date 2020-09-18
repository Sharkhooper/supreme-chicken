using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class finishLine : MonoBehaviour
{
    [SerializeField] private int playerLayer = 10;
    public int waitUntilReload;
    public GameObject wonText, timerText;
    public bool finished;

    private float time;
    void Start()
    {
        time = 0;
    }

    void Update()
    {
        time += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer && !finished)
        {
            StartCoroutine(Winning(other.gameObject));
        }
    }

    private IEnumerator Winning(GameObject player)
    {
        player.GetComponentInParent<PlayerInput>().enabled = false;
        finished = true;
        wonText.SetActive(true);
        timerText.SetActive(true);
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milli = Mathf.FloorToInt((time * 100) % 100);
        string timeString = String.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milli);
        timerText.GetComponent<TextMeshProUGUI>().text = "Your Time: " + timeString;
        yield return new WaitForSeconds(waitUntilReload);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
