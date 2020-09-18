using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dismemberment : MonoBehaviour {
    [System.Serializable]
    public struct DismemberPart {
        public Transform bone;
        public Rigidbody Limb;
    }

    [SerializeField] private float forceScale;
    [SerializeField] private float directionScatter = 15;
    [SerializeField] private float rotationForce = 2;
    [SerializeField] private float destroyDelay;
    [SerializeField] private Transform origin;
    [SerializeField] private DismemberPart[] exploders;

    public float DestroyDelay => destroyDelay;
    public Transform Origin => origin;

    public bool dodo = false;
    private bool done = false;

    void Update() {
        if (dodo) Dismember();
    }

    public void Dismember() {
        if (done) return;
        done = true;
        foreach (var ex in exploders) {
            var rb = Instantiate(ex.Limb);
            rb.transform.position = ex.bone.position;
            rb.transform.rotation = ex.bone.rotation;
            ex.bone.localScale = Vector3.zero;
            rb.AddExplosionForce(forceScale, origin.position, 2, 1, ForceMode.Impulse);
            Destroy(rb.gameObject, destroyDelay);
        }
    }

    public void DismemberWithForce(in Vector3 direction) {
        if (done) return;
        done = true;

        foreach (var ex in exploders) {
            var rb = Instantiate(ex.Limb);
            rb.transform.position = ex.bone.position;
            rb.transform.rotation = ex.bone.rotation;
            Vector3 randoDir = Quaternion.Euler(0, 0, Random.Range(-directionScatter, directionScatter)) * direction;
            rb.AddTorque(0, 0, Random.Range(-rotationForce, rotationForce), ForceMode.Force);
            rb.AddForce(forceScale * randoDir, ForceMode.Impulse);
            Destroy(rb.gameObject, destroyDelay);
        }
    }
}
