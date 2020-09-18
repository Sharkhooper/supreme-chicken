using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerOnHit : MonoBehaviour
{
    public bool shouldKill = true;

    private void OnTriggerEnter(Collider other)
    {
        if (shouldKill && other.gameObject.layer == 10 && other.GetComponent<Killable>() != null)
        {
            Debug.Log("Player gets killed");
            other.GetComponent<Killable>().GetKilled();
        }
    }
}
