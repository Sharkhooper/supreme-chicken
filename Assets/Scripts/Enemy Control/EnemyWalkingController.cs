using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalkingController : MonoBehaviour
{
    public GameObject model;
    public float moveSpeed;
    private Vector3 direction = new Vector3(1, 0, 0);
    private bool rotating;

    void Update()
    {
        if(!rotating)
            transform.parent.position += direction * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        direction *= -1;
        if(!rotating)
            StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        rotating = true;
        float currentRotation = transform.parent.rotation.y;
        if (currentRotation < 180)
        {
            while (transform.parent.rotation.y < 180)
            {
                transform.parent.RotateAround(model.transform.position, new Vector3(0, 1, 0), Time.deltaTime * moveSpeed * 100);
                yield return null;
            }
        }
        else if(currentRotation >= 180)
        {
            while(transform.parent.rotation.y > 0)
            {
                transform.parent.RotateAround(model.transform.position, new Vector3(0, 1, 0), Time.deltaTime * -moveSpeed * 100);
                yield return null;
            }
        }
        rotating = false;
    }
}
