using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{
    public float moveSpeed, randomPlusMinusThrow, randomPlusMinusRotate, toRotate, explosionForce, explosionRadius;
    public Vector3 direction;
    private Rigidbody rb;
    private GameObject mesh, particle; 

    private void Start() {
        mesh = transform.Find("PlateObject").gameObject;
        particle = transform.Find("PlateExplosion").gameObject;
        rb = gameObject.GetComponent<Rigidbody>();
        float randomizerThrow = Random.Range(-randomPlusMinusThrow, randomPlusMinusThrow);
        direction.y += randomizerThrow;
        float randomizerRotate = Random.Range(-randomPlusMinusRotate, randomPlusMinusRotate);
        rb.MoveRotation(Quaternion.Euler(0, 0, toRotate + randomizerRotate));
        rb.AddForce(direction.normalized * moveSpeed * 100);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name != "Enemy" && other.gameObject.layer != 8)
        {
            Explode();
        }
    }

    public void Explode()
    {
        rb.velocity = Vector3.zero;
        Destroy(mesh);
        particle.SetActive(true);
    }
}
