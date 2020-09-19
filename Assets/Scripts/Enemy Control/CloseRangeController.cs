using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeController : MonoBehaviour
{
    public enum EnemyType {Chef, Waiter, Cockroach}
    public EnemyType type;
    public LayerMask layerMask;
    private EnemyHolder holder;
    private float range, maxAngle;
    private GameObject player;
    public Vector3 offset;
    private Difficulty difficulty;
    public GameObject shootPoint;

    private EnemyCloseAttack enemyCloseAttack;
    private EnemyLaserAttack enemyLaserAttack;
    private EnemyThrowAttack enemyThrowAttack;
    private EnemyWalkingController enemyWalkingController;

    private void Start() {
        if(GetComponent<EnemyLaserAttack>() != null)
        {
            offset.y += GetComponent<EnemyLaserAttack>().myLaser.transform.localPosition.y;
            offset.y += GetComponent<EnemyLaserAttack>().myCannon.transform.localPosition.y;
        }
        difficulty = Difficulty.current;
        //Debug.Log("DIFF = " + difficulty);
        player = FindObjectOfType<MovementController>().gameObject;
        switch(type)
        {
            case EnemyType.Chef:
                range = difficulty.chef.range;
                maxAngle = difficulty.chef.angle;
                break;
            case EnemyType.Waiter:
                range = difficulty.waiter.range;
                maxAngle = difficulty.waiter.angle;
                break;
            case EnemyType.Cockroach:
                range = difficulty.cockroach.range;
                maxAngle = difficulty.cockroach.angle;
                break;
            default:
                break;
        }

        enemyCloseAttack = GetComponent<EnemyCloseAttack>();
        enemyThrowAttack = GetComponent<EnemyThrowAttack>();
        enemyLaserAttack = GetComponent<EnemyLaserAttack>();
        enemyWalkingController = GetComponent<EnemyWalkingController>();
    }

    void Update()
    {
        if (!player)
            return;
        Vector3 distanceVector = player.transform.position - (transform.position + offset);
        Vector3 normalized = distanceVector.normalized;
        Vector3 direction = enemyWalkingController.direction;
        float angle = Vector3.Angle(distanceVector - offset, direction);
        //Debug.Log("angle " + angle);
        if(distanceVector.magnitude < range && angle < maxAngle)
        {
            if(enemyLaserAttack != null)
            {
                if (player.transform.position.y < offset.y)
                    angle *= -1;
                enemyLaserAttack.TurnLaser(angle);
            }
            Debug.DrawRay(transform.position + offset, distanceVector, Color.green, Time.deltaTime);
            //Debug.DrawLine(transform.position + offset, player.transform.position, Color.green, Time.deltaTime);
            RaycastHit hit;
            if (Physics.Raycast(transform.position + offset, distanceVector, out hit, range, layerMask))
            {
                if(hit.transform.CompareTag("Player"))
                {
                    if (enemyCloseAttack != null)
                        enemyCloseAttack.StartAttack();
                    else if (enemyThrowAttack != null)
                        enemyThrowAttack.StartAttack(distanceVector, angle, offset);
                    else if (enemyLaserAttack != null)
                    {
                        enemyLaserAttack.StartAttack(angle);
                    }
                }
                else
                {
                    enemyWalkingController.MoveUpdate();
                }
            }
            else
            {
                enemyWalkingController.MoveUpdate();
            }
        }
        else
        {
            enemyWalkingController.MoveUpdate();
        }

        Vector3 targetLine = transform.position + offset + direction * range;
        Debug.DrawLine(transform.position + offset, targetLine, Color.red, Time.deltaTime);
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
