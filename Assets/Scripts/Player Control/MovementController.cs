using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour {

    [SerializeField] private float maxVelocity = 1;
    [SerializeField] private float velocitySmooth = 0;

    [SerializeField] private Vector3 feetOffset;

    private float inputVelocity = 0;
    private float velocity = 0;
    private float velocityChangeRate = 0;

    [SerializeField] private Rigidbody rb;

    public void HandleMovementInput(InputAction.CallbackContext context) {
        inputVelocity = context.ReadValue<float>();
    }

    public void HandleJumpInput(InputAction.CallbackContext context) {

    }

    public void HandleDashInput(InputAction.CallbackContext context) {

    }

    private static readonly Quaternion planeNormalRotation = Quaternion.Euler(0, 0, -90);
    private Vector3 plane;

    private void Update() {
        Vector3 pos = rb.transform.position;

        // Smooth input velocity
        velocity = Mathf.SmoothDamp(velocity, inputVelocity, ref velocityChangeRate, velocitySmooth);
        Vector3 v = Vector3.right * velocity * maxVelocity * Time.deltaTime;
        //Vector3 plane;

        // Get ground Vector and project velocity accordingly
        if (Physics.Raycast(pos + feetOffset, Vector3.down, out RaycastHit hit, 0.1f)) {
            if (hit.normal == Vector3.up) {
                plane = Vector3.right;
            }
            else {
                plane = planeNormalRotation * hit.normal;
            }
            v = Vector3.Project(v, plane);
        }

        // Move player
        rb.MovePosition(pos + v);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(rb.transform.position + feetOffset, 0.1f);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawLine(rb.transform.position + feetOffset, rb.transform.position + feetOffset + plane);
    }
}
