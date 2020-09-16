using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{
    public GameObject splinterPrefab;
    public float moveSpeed, randomPlusMinusThrow, randomPlusMinusRotate, toRotate, explosionForce, explosionRadius;
    public int splinters;
    public Vector3 direction;
    private Rigidbody rb;

    private void Start() {
        rb = gameObject.GetComponent<Rigidbody>();
        float randomizerThrow = Random.Range(-randomPlusMinusThrow, randomPlusMinusThrow);
        direction.y += randomizerThrow;
        float randomizerRotate = Random.Range(-randomPlusMinusRotate, randomPlusMinusRotate);
        rb.MoveRotation(Quaternion.Euler(0, 0, toRotate + randomizerRotate));
        rb.AddForce(direction.normalized * moveSpeed * 100);
        Debug.Log("direction: " + direction + " / randomizer: " + randomizerThrow);
    }

/*
    void FixedUpdate()
    {
        if(direction != null)

            //transform.position += direction * Time.deltaTime * moveSpeed;
    }
*/

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
        for (int i = 0; i < splinters; i++)
        {
            GameObject splinter = Instantiate(splinterPrefab, transform.position, Quaternion.identity);
            float xRot = Random.Range(0, 180);
            float yRot = Random.Range(0, 180);
            float zRot = Random.Range(0, 180);
            splinter.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(xRot, yRot, zRot));
            splinter.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }
        Destroy(gameObject);
    }
}
