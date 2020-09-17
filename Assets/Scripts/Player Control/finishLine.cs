using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class finishLine : MonoBehaviour
{

    [SerializeField] private int playerLayer = 10;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(playerLayer))
        {
            Application.Quit();
        }
    }
}
