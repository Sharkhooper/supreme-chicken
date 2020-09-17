using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public GameObject splashPrefab;

    private void OnMouseDown() {
        KillMe();
    }

    public void KillMe()
    {
        Debug.Log("KILLED");
        GameObject splash = Instantiate(splashPrefab, transform.position, Quaternion.identity);
        splash.transform.SetParent(transform);
        Destroy(gameObject);
    }
}
