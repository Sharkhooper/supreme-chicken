using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour {
    private struct ButtonHelper {
        public bool pressed;
        public bool down;
        public void Update(bool b) {
            if (!pressed && b) {
                down = true;
            }
            pressed = b;
        }
    }

    [Flags] private enum MovementState {
        None = 0,
        OnGround = 1,
        Jump = 2,
        WallGrab = 4,
        Dashing = 8,
    }

    public static MovementController Active { get; private set; }

    // Maximum horizontal velocity
    [SerializeField] private float maxVelocity = 16;
    // Velocity damping - zero equals instant changes
    [SerializeField] private float velocitySmooth = 0.12f;

    // Ground is detected with a tolerance resulting in a slight hover
    // Distance of said tolerance in downwards direction
    [SerializeField] private float rayGroundTolerance = 0.09f;

    // Maximum angle at which a plane is walkable / counts as ground
    [SerializeField] private float slopeAngleThreshold = 45;

    [SerializeField] private bool allowWallgrab = true;
    // Maximum angle at which a ledge is considerd for a grab
    [SerializeField] private float wallGrabAngleThreshold = 33;
    // Distance tolerance for wall grab spherecast
    [SerializeField] private float wallGrabDistTolerance = 0.02f;

    // If on ground and hovering because of tolerance, how much hoverheight percentage should be corrected per fixedUpdate
    [SerializeField] [Range(0, 1)]  private float groundCorrectionScale = 0.98f;

    [SerializeField] private MovementState state;
    private MovementState prevState;

    // Movement
    private ButtonHelper jumpButton;
    private float inputVelocity = 0;
    private float velocityMagnitude = 0;
    private float velocityChangeRate = 0;
    private float orientation;
    private Vector3 dashDirection;
    private ButtonHelper dashButton;
    private ButtonHelper attackButton;

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
    [SerializeField] private float dashResetFallTime = 0.25f;
    [SerializeField] private float dashVelocity = 22;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private int   dashTicks;
    private int dashActiveTicks;
    private float dashTime = 0;

    // Attack
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackRange;
    [SerializeField] private int attackTicks = 10;
    private float attackTime = 0;
    private int attackActiveTicks = 0;

    // Components
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private SphereCollider bodyCollider;
    [SerializeField] private SphereCollider feetCollider;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private Transform model;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float cameraMaxDelta = 1.86f;
    [SerializeField] private LayerMask collisionLayers = ~0;
    [SerializeField] private LayerMask damageLayers = 0;
    [SerializeField] private PlayerSoundController sound;
    private Camera cam;

    public PlayerSoundController Sound => sound;

    private struct AnimationParameters {
        public int air;
        public int dash;
        public int attack;
        public int walking;
        public int ascending;
        public int layerWalking;
    }

    private AnimationParameters animationParameters;

    [SerializeField] [Range(0,1)] private float debugScale = 0.2f;

    // Sequences
    private Sequence walkingHobble;
    [SerializeField] private float hobbleInterval = 0.15f;
    [SerializeField] private float hobbleHeight = 0.1f;
    private bool walkingHobbleCondition => state.HasFlag(MovementState.OnGround) && !walkingHobble.IsPlaying() && velocity.magnitude > 0.1f;

    private void Start() {
        cam = Camera.main;
        Debug.Assert(dashCooldown > dashDuration);

        Active = this;


        animationParameters = new AnimationParameters {
            air = Animator.StringToHash("Air"),
            dash = Animator.StringToHash("Dash"),
            attack = Animator.StringToHash("Attacking"),
            walking = Animator.StringToHash("Walking"),
            ascending = Animator.StringToHash("Ascending"),
            layerWalking = animator.GetLayerIndex("Walking"),
        };

        // Hobble sequence
        walkingHobble = DOTween.Sequence();
        walkingHobble.SetAutoKill(false);
        walkingHobble.Append(model.DOLocalMoveY(hobbleHeight, hobbleInterval).SetEase(Ease.OutSine));
        walkingHobble.Append(model.DOLocalMoveY(0, hobbleInterval).SetEase(Ease.InSine));

        walkingHobble.onComplete += () => {
            if (walkingHobbleCondition) walkingHobble.Restart();
        };
    }

    public void HandleMovementInput(InputAction.CallbackContext context) {
        inputVelocity = context.ReadValue<float>();
    }

    public void HandleJumpInput(InputAction.CallbackContext context) {
        bool b = context.ReadValueAsButton();
        jumpButton.Update(b);
    }

    public void HandleAttackInput(InputAction.CallbackContext context) {
        attackButton.Update(context.ReadValueAsButton());
    }

    public void HandleDashInput(InputAction.CallbackContext context) {
        bool b = context.ReadValueAsButton();
        dashButton.Update(context.ReadValueAsButton());

        if (dashButton.down) {
            Ray r = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            intersectionPlane.Raycast(r, out float d);
            Vector3 dashTarget = r.origin + d * r.direction.normalized;
            dashDirection = dashTarget - rb.transform.position;
 
            if (dashDirection.magnitude < 0.01f) {
                dashDirection = Vector3.zero;
            }
            else {
                dashDirection.Normalize();
                dashDirection *= dashVelocity;
            }
        }
    }

    public void HandleDashInputGamepad(InputAction.CallbackContext context) {
        dashButton.Update(context.ReadValueAsButton());

        if (dashButton.down) {
            Vector2 target = Gamepad.current.rightStick.ReadValue();
            if (target.magnitude < 0.01f) {
                dashDirection = Vector3.zero;
            }
            else {
                dashDirection = (Vector3)(target.normalized * dashVelocity);
            }
        }
    }

    public void Kill() {
        sound.Die();
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

        animator.SetLayerWeight(animationParameters.layerWalking, 0);
        animator.SetFloat(animationParameters.walking, 0);

        if (dashButton.down && !state.HasFlag(MovementState.Dashing) && dashTime <= 0) {
            state |= MovementState.Dashing;
            state &= ~MovementState.OnGround;
            state &= ~MovementState.Jump;
            state &= ~MovementState.WallGrab;
            coyoteTimeCooldown = 0;
            dashTime = dashCooldown;
            gravity = Vector3.zero;
            velocity = Vector3.zero;
            attackTime = 0;

            sound.Dash(dashDuration);

            if (dashDirection == Vector3.zero) {
                dashDirection = (orientation > 0 ? Vector3.right : Vector3.left) * dashVelocity;
            }

            trail.emitting = true;
            animator.SetBool(animationParameters.attack, false);
            animator.SetBool(animationParameters.dash, true);
            animator.SetBool(animationParameters.ascending, false);
            dashActiveTicks = 0;
        }
        else if (attackButton.down && !state.HasFlag(MovementState.WallGrab) && attackTime <= 0) {
            attackTime = attackCooldown;
            sound.Attack(attackDuration);
            // Skip first attack to account for animation delay
            attackActiveTicks = attackTicks;
        }


        if (dashTime > 0) {
            dashTime -= Time.fixedDeltaTime;
        }

        if (attackTime > 0) {
            attackTime -= Time.fixedDeltaTime;
        }


        if (state.HasFlag(MovementState.Dashing)) {
            if (dashTime < dashCooldown - dashDuration) {
                state &= ~MovementState.Dashing;
                trail.emitting = false;
                animator.SetBool(animationParameters.dash, false);
            }
            else {
                model.rotation = Quaternion.Euler(0, Vector3.Angle(Vector3.right, dashDirection) < 90 ? 0 : 180, 0);
                rb.velocity = dashDirection;
                if (dashActiveTicks <= 0) {
                    DoDamage();
                    dashActiveTicks = dashTicks;
                }
            }
        }

        // not else so this path is called when dashing flag is cleared
        if (!state.HasFlag(MovementState.Dashing)) {
            if (attackTime >= attackCooldown - attackDuration) {
                trail.emitting = true;
                animator.SetBool(animationParameters.attack, true);
                --attackActiveTicks;

                if (attackActiveTicks <= 0) {
                    DoDamage();
                    attackActiveTicks = attackTicks;
                }

            }
            else {
                trail.emitting = false;
                animator.SetBool(animationParameters.attack, false);
            }

            // ==================
            // #### Movement ####
            // ==================

            // Smooth input velocity
            velocityMagnitude = Mathf.SmoothDamp(velocityMagnitude, inputVelocity, ref velocityChangeRate, velocitySmooth);
            velocity = Vector3.right * velocityMagnitude * maxVelocity * Time.fixedDeltaTime;

            if (Mathf.Abs(velocityMagnitude) > 0.1f) {
                orientation = velocityMagnitude > 0 ? 1 : -1;
                model.rotation = Quaternion.Euler(0, velocityMagnitude > 0 ? 0 : 180, 0);
                sound.StartWalk();
            }
            else {
                sound.StopWalk();
            }

            cameraTarget.localPosition =  Vector3.Lerp(cameraTarget.localPosition, cameraOffset * velocityMagnitude, cameraMaxDelta * Time.fixedDeltaTime);

            RaycastHit hit;

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

                    // Reset Dash Cooldown on Landing
                    // Only reset if enogh time has passed since end of dash
                    if (!prevState.HasFlag(MovementState.Dashing) && !prevState.HasFlag(MovementState.OnGround) && dashTime < dashCooldown - dashDuration - dashResetFallTime) {
                        dashTime = 0;
                        Debug.Log("Cooldown Reset");
                    }

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


                // Wall Grab
                if (allowWallgrab && Physics.SphereCast(bodyPos + Vector3.up * 0.001f, bodyRad, Vector3.down, out hit, 0.001f + wallGrabDistTolerance, collisionLayers)) {
                    float angle = Vector3.Angle(Vector3.up, hit.normal);
                    if (angle < wallGrabAngleThreshold) {
                        state |= MovementState.WallGrab;
                        velocity = Vector3.zero;
                        animator.SetBool(animationParameters.ascending, false);
                        animator.SetBool(animationParameters.air, false);
                    }
                }
            }

            // Initialize jump
            bool canJump = coyoteTimeCooldown > 0 || state.HasFlag(MovementState.OnGround) || state.HasFlag(MovementState.WallGrab);
            if (canJump && jumpButton.down) {
                gravity = Vector3.up * initialJumpVelocity;
                state |= MovementState.Jump;
                state &= ~MovementState.OnGround;
                state &= ~MovementState.WallGrab;
                coyoteTimeCooldown = 0;

                sound.Jump();
            }

            // Stop receiving new jump input midair
            if (state.HasFlag(MovementState.Jump) && (!jumpButton.pressed || gravity.y <= 0)) {
                state &= ~MovementState.Jump;
            }

            // =================
            // #### Gravity ####
            // =================

            // if GRAVITY
            if (!state.HasFlag(MovementState.WallGrab)) {
                // Handle jump gravity
                if (gravity.y > 0) {
                    animator.SetBool(animationParameters.ascending, true);
                    if (jumpButton.pressed) {
                        gravity += Physics.gravity * highJumpMultiplier * Time.fixedDeltaTime;
                    }
                    else {
                        gravity += Physics.gravity * lowJumpMultiplier * Time.fixedDeltaTime;
                    }
                }
                // Regular gravity
                else if (!state.HasFlag(MovementState.OnGround)) {
                    animator.SetBool(animationParameters.ascending, false);
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

                rb.velocity = velocity / Time.fixedDeltaTime;


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
                        float angle = Vector3.Angle(Vector3.up, hit.normal);
                        if (angle < slopeAngleThreshold) {
                            float dist = Vector3.Distance(feetPos, hit.point) - feetRad;
                            rb.velocity = rb.velocity.normalized * dist / Time.fixedDeltaTime;
                        }
                    }
                }
                else {
                    rb.velocity += onGroundCorrection / Time.fixedDeltaTime;
                }
            }
            else { // elseif GRAVITY
                rb.velocity = Vector3.zero;
            }

        }

        jumpButton.down = false;
        attackButton.down = false;
        dashButton.down = false;

        if (state != 0 && !state.HasFlag(MovementState.Jump)) {
            animator.SetBool(animationParameters.air, false);
        }
        else {
            animator.SetBool(animationParameters.air, true);
        }

        if (state.HasFlag(MovementState.OnGround)) {
            float animWalk = Mathf.Abs(velocityMagnitude);
            if (animWalk < 0.05f) animWalk = 0;
            if (animWalk > 0.95f) animWalk = 1;
            animator.SetFloat(animationParameters.walking, 1);
            animator.SetLayerWeight(animationParameters.layerWalking, animWalk);
        }
        else {
            animator.SetLayerWeight(animationParameters.layerWalking, 0);
        }

        if (walkingHobbleCondition) {
            walkingHobble.Rewind();
            walkingHobble.Play();
        }

        prevState = state;
    }

    private void DoDamage() {
        Vector3 origin = rb.transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, attackRange);
        foreach (Collider c in hits) {
            if ((1 << c.gameObject.layer & damageLayers.value) != 0) {
                if (c.TryGetComponent(out Killable k)) {
                    k.GetKilled();
                }
                else {
                    Debug.LogWarning($"Gameobject {c.gameObject.name} is on Killable layer, but has no Killable component!");
                }
            }
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
