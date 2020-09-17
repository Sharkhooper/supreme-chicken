using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeController : MonoBehaviour
{
    public LayerMask layerMask;
    private EnemyHolder holder;
    public float range, maxAngle;
    private GameObject player;
    public Vector3 offset;

    private void Start() {
        player = FindObjectOfType<MovementController>().gameObject;
    }

    void Update()
    {
        Vector3 distanceVector = player.transform.position - (transform.position + offset);
        Vector3 normalized = distanceVector.normalized;
        Vector3 direction = GetComponent<EnemyWalkingController>().direction;
        float angle = Vector3.Angle(distanceVector, direction);
        if(distanceVector.magnitude < range && angle < maxAngle)
        {
            Debug.DrawLine(transform.position + offset, distanceVector, Color.green, Time.deltaTime);
            RaycastHit hit;
            if (Physics.Raycast(transform.position + offset, distanceVector, out hit, range, layerMask))
            {
                if (holder.enemyCloseAttack != null)
                    holder.enemyCloseAttack.StartAttack();
                else if (GetComponent<EnemyThrowAttack>() != null)
                    GetComponent<EnemyThrowAttack>().StartAttack(distanceVector, angle, offset);
                else if (holder.enemyLaserAttack != null)
                    holder.enemyLaserAttack.StartAttack(angle);
            }
            else
            {
                holder.enemyWalkingController.MoveUpdate();
            }
        }
        else
        {
            holder.enemyWalkingController.MoveUpdate();
        }

        Vector3 targetLine = transform.position + offset + direction * range;
        Debug.DrawLine(transform.position + offset, targetLine, Color.red, Time.deltaTime);
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
