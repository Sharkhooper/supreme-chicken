using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalkingController : MonoBehaviour
{
    public enum EnemyType { Chef, Waiter, Cockroach }
    public EnemyType type;
    public LayerMask layerMask;
    private EnemyHolder holder;
    private float moveSpeed;
    public float turnRange;
    public Vector3 direction = new Vector3(1, 0, 0);
    public bool rotated = false, move = true;
    private Animator animator;
    private Difficulty difficulty;

    private void Start() {
        animator = GetComponent<Animator>();
        difficulty = Difficulty.current;
        switch (type)
        {
            case EnemyType.Chef:
                moveSpeed = difficulty.chef.moveSpeed;
                break;
            case EnemyType.Waiter:
                moveSpeed = difficulty.waiter.moveSpeed;
                break;
            case EnemyType.Cockroach:
                moveSpeed = difficulty.cockroach.moveSpeed;
                break;
            default:
                break;
        }
        if(animator != null)
            animator.SetFloat("moveSpeed", moveSpeed);
    }

    public void PlaceDown() {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 5))
        {
            holder.transform.position = hit.point;
        }
    }

    void Update()
    {
        if(animator != null)
            animator.SetBool("isWalking", move);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, direction, out hit, turnRange, layerMask))
        {
            if (hit.transform.name != "Player" && hit.transform.name != "Plate(Clone)")
            {
                //Debug.Log("Not Player (turn): " + hit.transform.name);
                direction *= -1;
                transform.parent.RotateAround(transform.position, Vector3.up, 180);
                rotated = !rotated;
            }
        }
    }

    public void MoveUpdate()
    {
        if(move)
        {
            Vector3 newPos = holder.transform.position + direction * Time.deltaTime * moveSpeed *4 ;
            holder.transform.position = newPos;
        }

        //transform.position += direction * Time.deltaTime * moveSpeed;
        //holder.closeRangeController.gameObject.GetComponent<Rigidbody>().MovePosition(transform.parent.position + direction * Time.fixedDeltaTime * moveSpeed);
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
