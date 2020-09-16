using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxLifeController : MonoBehaviour
{
    public float lifespan;
    void Start()
    {
        StartCoroutine(MaxLife());
    }

    private IEnumerator MaxLife()
    {
        yield return new WaitForSeconds(lifespan);
        Destroy(gameObject);
    }
}
