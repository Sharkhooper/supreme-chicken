using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeController : MonoBehaviour
{
    private EnemyHolder holder;
    public float range, maxAngle;
    private GameObject player;

    private void Start() {
        player = FindObjectOfType<MovementController>().gameObject;
    }

    void Update()
    {
        Vector3 distanceVector = player.transform.position - transform.position;
        Vector3 normalized = distanceVector.normalized;
        Vector3 direction = holder.enemyWalkingController.direction;
        float angle = Vector3.Angle(distanceVector, direction);

        if(distanceVector.magnitude < range && angle < maxAngle)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, distanceVector, out hit, range) &&  hit.transform.name == "Player")
            {
                if (holder.enemyCloseAttack != null)
                    holder.enemyCloseAttack.StartAttack();
                if (holder.enemyThrowAttack != null)
                    holder.enemyThrowAttack.StartAttack(distanceVector, angle);
            }
            else
            {
                RaycastHit[] hits = Physics.RaycastAll(transform.position, distanceVector, range);
                /*
                for (int i = 0; i < hits.Length; i++)
                {
                    Debug.Log("Hit " + i + ": " + hits[i].transform.name);
                }
                */

                //if (hits.Length > 1 && hits[0].collider.tag == "InvisibleWall" && hits[1].transform.name == "Player")
                if (hits.Length > 1 && hits[0].transform.gameObject.layer == 8 && hits[1].transform.name == "Player")
                {
                    //Debug.Log("Behind invisiwall");
                    if (holder.enemyCloseAttack != null)
                        holder.enemyCloseAttack.StartAttack();
                    if (holder.enemyThrowAttack != null)
                        holder.enemyThrowAttack.StartAttack(distanceVector, angle);
                }
                else
                {
                    holder.enemyWalkingController.MoveUpdate();
                }
            }
            
        }
        else
        {
            holder.enemyWalkingController.MoveUpdate();
        }

        Vector3 targetLine = transform.position + direction * range;
        Debug.DrawLine(transform.position, targetLine, Color.red, Time.deltaTime);
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
