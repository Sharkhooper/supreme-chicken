﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour {
    [Flags] private enum MovementState {
        None = 0,
        OnGround = 1,
        Jump = 2,
        WallGrab = 4,
        Dashing = 8,
    }

    // Maximum horizontal velocity
    [SerializeField] private float maxVelocity = 16;
    // Velocity damping - zero equals instant changes
    [SerializeField] private float velocitySmooth = 0.12f;

    // Ground is detected with a tolerance resulting in a slight hover
    // Distance of said tolerance in downwards direction
    [SerializeField] private float rayGroundTolerance = 0.09f;

    // Maximum angle at which a plane is walkable / counts as ground
    [SerializeField] private float slopeAngleThreshold = 45;

    // Maximum angle at which a ledge is considerd for a grab
    [SerializeField] private float wallGrabAngleThreshold = 33;
    // Distance tolerance for wall grab spherecast
    [SerializeField] private float wallGrabDistTolerance = 0.02f;

    // If on ground and hovering because of tolerance, how much hoverheight percentage should be corrected per fixedUpdate
    [SerializeField] [Range(0, 1)]  private float groundCorrectionScale = 0.98f;

    [SerializeField] private MovementState state;

    // Movement
    bool jumpInputDown = false;
    bool jumpInput = false;
    private float inputVelocity = 0;
    private float velocityMagnitude = 0;
    private float velocityChangeRate = 0;
    private Vector3 dashTarget;
    private Vector3 dashDirection;
    private bool dashInput = false;
    private bool dashInputDown = false;
    private bool attackInput = false;

    // Jump
    [SerializeField] private float lowJumpMultiplier = 5;
    [SerializeField] private float highJumpMultiplier = 3;
    [SerializeField] private float fallMultiplier = 6;
    [SerializeField] private float initialJumpVelocity = 15;

    // Coyote Time - Meep Meep
    [SerializeField] private double coyoteTime = 0.09;
    private double coyoteTimeCooldown;

    // Dash
    [SerializeField] private float dashCooldown = 1.5f;
    [SerializeField] private float dashVelocity = 22;
    [SerializeField] private float dashDuration = 0.5f;
    private float dashTime = 0;

    // Components
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private SphereCollider bodyCollider;
    [SerializeField] private SphereCollider feetCollider;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float cameraMaxDelta = 1.86f;
    [SerializeField] private LayerMask collisionLayers = ~0;
    private Camera cam;

    private struct AnimationParameters {
        public int air;
        public int dash;
        public int attack;
        public int walking;
    }

    private AnimationParameters animationParameters;

    [SerializeField] [Range(0,1)] private float debugScale = 0.2f;

    private void Start() {
        cam = Camera.main;
        Debug.Assert(dashCooldown > dashDuration);


        animationParameters = new AnimationParameters {
            air = Animator.StringToHash("Air"),
            dash = Animator.StringToHash("Dash"),
            attack = Animator.StringToHash("Attacking"),
            walking = Animator.StringToHash("Walking"),
        };
    }

    public void HandleMovementInput(InputAction.CallbackContext context) {
        inputVelocity = context.ReadValue<float>();
    }

    public void HandleJumpInput(InputAction.CallbackContext context) {
        bool b = context.ReadValueAsButton();
        if (!jumpInput && b) {
            jumpInputDown = true;
        }
        jumpInput = b;
    }

    public void HandleAttackInput(InputAction.CallbackContext context) {
        attackInput = context.ReadValueAsButton();
    }

    public void HandleDashInput(InputAction.CallbackContext context) {
        bool b = context.ReadValueAsButton();
        if (!dashInput && b) {
            dashInputDown = true;
        }
        dashInput = b;

        if (dashInputDown) {
            Ray r = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            intersectionPlane.Raycast(r, out float d);
            dashTarget = r.origin + d * r.direction.normalized;
            dashDirection = dashTarget - rb.transform.position;
            dashDirection.Normalize();
            dashDirection *= dashVelocity;
        }
    }

    private static readonly Plane intersectionPlane = new Plane(Vector3.forward, 0);
    private static readonly Quaternion planeNormalRotation = Quaternion.Euler(0, 0, -90);
    private Vector3 plane;
    private Vector3 gravity;
    private Vector3 velocity;

    private void FixedUpdate() {
        Vector3 onGroundCorrection = Vector3.zero;
        Vector3 feetPos = feetCollider.transform.position;
        Vector3 bodyPos = bodyCollider.transform.position;
        float feetRad = feetCollider.radius;
        float bodyRad = bodyCollider.radius;

        animator.SetLayerWeight(animator.GetLayerIndex("Walking"), 0);
        animator.SetFloat(animationParameters.walking, 0);
        animator.SetBool(animationParameters.attack, attackInput);

        if (dashInputDown && !state.HasFlag(MovementState.Dashing) && dashTime <= 0) {
            state |= MovementState.Dashing;
            state &= ~MovementState.OnGround;
            state &= ~MovementState.Jump;
            state &= ~MovementState.WallGrab;
            coyoteTimeCooldown = 0;
            dashTime = dashCooldown;
            gravity = Vector3.zero;
            velocity = Vector3.zero;
        }

        if (dashTime > 0) {
            dashTime -= Time.fixedDeltaTime;
        }

        if (state.HasFlag(MovementState.Dashing)) {
            if (dashTime >= dashCooldown - dashDuration)
                rb.velocity = dashDirection;
            else {
                state &= ~MovementState.Dashing;
            }
        }
        else {
            // ==================
            // #### Movement ####
            // ==================

            // Smooth input velocity
            velocityMagnitude = Mathf.SmoothDamp(velocityMagnitude, inputVelocity, ref velocityChangeRate, velocitySmooth);
            velocity = Vector3.right * velocityMagnitude * maxVelocity * Time.fixedDeltaTime;
            float orientation = velocityMagnitude > 0 ? 1 : -1;

            cameraTarget.localPosition =  Vector3.Lerp(cameraTarget.localPosition, cameraOffset * velocityMagnitude, cameraMaxDelta * Time.fixedDeltaTime);

            RaycastHit hit;
            float animWalk = Mathf.Abs(velocityMagnitude);
            if (animWalk < 0.05f) animWalk = 0;
            if (animWalk > 0.95f) animWalk = 1;
            animator.SetFloat(animationParameters.walking, 1);
            animator.SetLayerWeight(animator.GetLayerIndex("Walking"), animWalk);


            // Get ground Vector and project velocity accordingly
            state &= ~MovementState.OnGround;
            // Spherecast for platforms
            // Spherecast skips colliders if colliders overlap at start
            // As a workaround the startingposition ist set slightly above center
            // Proper workaround would be multiple colliders for the body - and one only for the legs / feet
            if (Physics.SphereCast(feetPos + Vector3.up * 0.01f, feetRad, Vector3.down, out hit, 0.01f + rayGroundTolerance, collisionLayers)) {
                float angle = Vector3.Angle(Vector3.up, hit.normal);
                if (angle < slopeAngleThreshold) {
                    state |= MovementState.OnGround;

                    coyoteTimeCooldown = coyoteTime;

                    plane = planeNormalRotation * hit.normal;
                    velocity = Vector3.Project(velocity, plane);
                    float distance = Vector3.Distance(feetPos, hit.point) - feetRad;
                    onGroundCorrection = distance * Vector3.down * groundCorrectionScale;
                }
            }

            if (!state.HasFlag(MovementState.OnGround)) {
                if (coyoteTimeCooldown > 0) {
                    coyoteTimeCooldown -= Time.fixedDeltaTime;
                }

                if (Physics.SphereCast(bodyPos + Vector3.up * 0.001f, bodyRad, Vector3.down, out hit, 0.001f + wallGrabDistTolerance, collisionLayers)) {
                    float angle = Vector3.Angle(Vector3.up, hit.normal);
                    if (angle < wallGrabAngleThreshold) {
                        state |= MovementState.WallGrab;
                        velocity = Vector3.zero;
                    }
                }
            }

            // Initialize jump
            bool canJump = coyoteTimeCooldown > 0 || state.HasFlag(MovementState.OnGround) || state.HasFlag(MovementState.WallGrab);
            if (canJump && jumpInputDown) {
                gravity = Vector3.up * initialJumpVelocity;
                state |= MovementState.Jump;
                state &= ~MovementState.OnGround;
                state &= ~MovementState.WallGrab;
                coyoteTimeCooldown = 0;
            }

            // Stop receiving new jump input midair
            if (state.HasFlag(MovementState.Jump) && (!jumpInput || gravity.y <= 0)) {
                state &= ~MovementState.Jump;
            }

            // =================
            // #### Gravity ####
            // =================

            // Handle jump gravity
            if (gravity.y > 0) {
                if (jumpInput) {
                    gravity += Physics.gravity * highJumpMultiplier * Time.fixedDeltaTime;
                }
                else {
                    gravity += Physics.gravity * lowJumpMultiplier * Time.fixedDeltaTime;
                }
            }
            // Regular gravity
            else if (!state.HasFlag(MovementState.OnGround) && !state.HasFlag(MovementState.WallGrab)) {
                gravity += Physics.gravity * fallMultiplier * Time.fixedDeltaTime;
            }
            // On ground special cases
            else {
                gravity = Vector3.zero;

                // Slope Fix if not jumping
                // Fixes speedloss on transition between planes with different angles
                Vector3 ground = hit.point;
                // Fire spherecast in velocity direction
                if (Mathf.Abs(velocityMagnitude) > 0.00001f && Physics.SphereCast(feetPos, feetRad, velocity, out hit, velocity.magnitude, collisionLayers)) {
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
                        }
                    }
                }
            }

            // Wall detection
            if (velocity.magnitude > 0.01f && Physics.SphereCast(bodyPos - Vector3.right * orientation * 0.01f, bodyRad, velocity, out hit, velocity.magnitude + 0.01f, collisionLayers)) {
                float angle = Vector3.Angle(Vector3.up, hit.normal);
                if (angle >= slopeAngleThreshold) {
                    velocity.Normalize();
                    float dist = Vector3.Distance(hit.point, bodyPos) - bodyRad;
                    velocity *= dist;
                }
            }

            velocity /= Time.fixedDeltaTime;
            rb.velocity = velocity;


            if (!state.HasFlag(MovementState.OnGround)) {
                rb.velocity += gravity;
                // Cap velocity to not glitch into ground
                // Capping this on velocity instead of on gravity and then correcting velocity like on slopes
                // results in a slowdown on landing, because horizontal movement gets skipped
                // Ideally remember the cut part and perform a slope fix on it with the landingplane as ground

                // Calculating this step on gravity instead of velocity can result in a glitch where walking
                // over a ledge causes swapping to WallGrab state without actually snapping to a wall, which
                // therefor results in infinite hover, cancelable through a jump
                Vector3 off = velocity.normalized * 0.01f;
                if (Physics.SphereCast(feetPos - off, feetRad, rb.velocity, out hit, rb.velocity.magnitude * Time.fixedDeltaTime + feetRad + 0.01f, collisionLayers)) {
                    float dist = Vector3.Distance(feetPos, hit.point) - feetRad;
                    rb.velocity = rb.velocity.normalized * dist / Time.fixedDeltaTime;
                }
            }
            else {
                rb.velocity += onGroundCorrection / Time.fixedDeltaTime;
            }
        }

        jumpInputDown = false;
        dashInputDown = false;
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
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(rb.transform.position + feetOffset, 0.1f);
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(rb.transform.position + feetOffset + rb.velocity * Time.deltaTime, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + rb.velocity * debugScale);
        Gizmos.DrawLine(transform.position + Vector3.right, transform.position + Vector3.right + gravity * debugScale);
        Gizmos.DrawLine(transform.position + Vector3.down, transform.position + Vector3.down + velocity * debugScale);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.magenta;
        Vector3 feetOffset = new Vector3(0, -0.5f, 0);
        Gizmos.DrawLine(rb.transform.position + feetOffset, rb.transform.position + feetOffset + plane);
    }
}
