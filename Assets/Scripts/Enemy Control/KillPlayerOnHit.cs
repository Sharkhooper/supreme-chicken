using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerOnHit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10 && other.GetComponent<Killable>() != null)
        {
            Debug.Log("Player gets killed");
            other.GetComponent<Killable>().GetKilled();
        }
    }
}
