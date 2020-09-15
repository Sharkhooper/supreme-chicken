using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour {
    [Flags] private enum MovementState {
        None = 0,
        OnGround = 1,
        CanJump = 2,
    }

    [SerializeField] private float colliderRadius = 0.5f;

    [SerializeField] private float maxVelocity = 1;
    [SerializeField] private float velocitySmooth = 0;
    [SerializeField] private float rayOffsetRadius = 0.5f;
    [SerializeField] private float rayDistanceCorrection = 0.5f;
    [SerializeField] private float rayGroundTolerance = 0.01f;
    [SerializeField] private float rayWallTolerance = 0.55f;
    [SerializeField] private float slopeAngleThreshold = 45;
    [SerializeField] [Range(0, 1)]  private float groundCorrectionScale = 0.96f;

    [SerializeField] private Vector3 feetOffset;

    [SerializeField] private bool capVelocityClip = true;

    [SerializeField] private MovementState state;

    // Movement
    [SerializeField] private float inputVelocity = 0;
    private float velocityMagnitude = 0;
    private float velocityChangeRate = 0;

    // Jump
    private bool jumping = false;
    private Vector3 jumpVelocity = Vector3.zero;

    // Coyote Time - Meep Meep
    [SerializeField] private double coyoteTime = 0.5;

    // Components
    [SerializeField] private Rigidbody rb;

    public void HandleMovementInput(InputAction.CallbackContext context) {
        inputVelocity = context.ReadValue<float>();
    }

    public void HandleJumpInput(InputAction.CallbackContext context) {
        jumping = context.ReadValueAsButton();
    }

    public void HandleDashInput(InputAction.CallbackContext context) {

    }

    private static readonly Quaternion planeNormalRotation = Quaternion.Euler(0, 0, -90);
    private Vector3 plane;

    private void FixedUpdate() {
        Vector3 pos = rb.transform.position;
        Vector3 onGroundCorrection = Vector3.zero;

        // ==================
        // #### Movement ####
        // ==================

        // Smooth input velocity
        velocityMagnitude = Mathf.SmoothDamp(velocityMagnitude, inputVelocity, ref velocityChangeRate, velocitySmooth);
        Vector3 velocity = Vector3.right * velocityMagnitude * maxVelocity * Time.fixedDeltaTime;
        float orientation = velocityMagnitude > 0 ? 1 : -1;

        RaycastHit hit;

        // Get ground Vector and project velocity accordingly
        state &= ~MovementState.OnGround;
        // Spherecast for platforms
        // Spherecast skips colliders if colliders overlap at start
        // As a workaround the startingposition ist set slightly above center
        // Proper workaround would be multiple colliders for the body - and one only for the legs / feet
        if (Physics.SphereCast(pos + Vector3.up * 0.01f, colliderRadius, Vector3.down, out hit, rayGroundTolerance/*, layerMask*/)) {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle < slopeAngleThreshold) {
                state |= MovementState.OnGround;

                plane = planeNormalRotation * hit.normal;
                velocity = Vector3.Project(velocity, plane);
                onGroundCorrection = hit.distance * Vector3.down * groundCorrectionScale;
            }
        }

        // Slope Fix
        Vector3 ground = hit.point;
        // Fire spherecast in velocity direction
        if (Mathf.Abs(velocityMagnitude) > 0.00001f && Physics.SphereCast(pos, colliderRadius, velocity, out hit, velocity.magnitude)) {
            // check if cast hits something and hitted plane is a walkable plane
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle < slopeAngleThreshold) {
                // Get intersection point of velocity line and plane tangent
                Vector3 tangent = planeNormalRotation * hit.normal;
                Vector3 intersect = GetIntersectionPointCoordinates(ground, ground + velocity, hit.point, hit.point + tangent, out bool found);
                if (found) {
                    // Slice velocity vector at intersection and "project" remaining piece to tangent while keeping the same magnitude
                    float f = Vector3.Dot(intersect - ground, velocity);
                    velocity = f * velocity + orientation * (1 - f) * velocity.magnitude * tangent.normalized;
                    Debug.Log(velocity);
                }
            }
        }

        if (velocity.magnitude > 0 && Physics.SphereCast(pos, colliderRadius, velocity, out hit, velocity.magnitude)) {
            velocity.Normalize();
            velocity *= hit.distance;
        }

        // =================
        // #### Gravity ####
        // =================


        // Move player
        rb.velocity = velocity / Time.fixedDeltaTime;
        if (!state.HasFlag(MovementState.OnGround))
            rb.velocity += Physics.gravity * Time.fixedDeltaTime;
        else {
            rb.velocity += onGroundCorrection / Time.fixedDeltaTime;
        }
    }

    // Currently only supports vectors on xy-plane. For more versatility project on some plane.
    public Vector3 GetIntersectionPointCoordinates(Vector3 A1, Vector3 A2, Vector3 B1, Vector3 B2, out bool found) {
        float z = A1.z;
        Vector2 sol = GetIntersectionPointCoordinates2D(A1, A2, B1, B2, out found);
        return new Vector3(sol.x, sol.y, z);
    }


    // Based on https://blog.dakwamine.fr/?p=1943
    public Vector2 GetIntersectionPointCoordinates2D(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found) {
        float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

        if (tmp == 0) {
            // No solution!
            found = false;
            return Vector2.zero;
        }

        float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

        found = true;

        return new Vector2(
            B1.x + (B2.x - B1.x) * mu,
            B1.y + (B2.y - B1.y) * mu
        );
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rb.transform.position + feetOffset, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(rb.transform.position + feetOffset + rb.velocity * Time.deltaTime, 0.1f);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(rb.transform.position + feetOffset, rb.transform.position + feetOffset + plane);
    }
}
