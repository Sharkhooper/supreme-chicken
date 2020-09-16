using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplinterController : MonoBehaviour
{
    private Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        if(rb.velocity == Vector3.zero)
        {
            GetComponent<BoxCollider>().enabled = false;
            rb.detectCollisions = false;
            rb.freezeRotation = true;
        }
    }
}
