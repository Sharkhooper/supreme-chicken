﻿using System.Collections;
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

    private void Start() {
        difficulty = Difficulty.current;
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
    }

    void Update()
    {
        if (!player)
            return;
        Vector3 distanceVector = player.transform.position - (transform.position + offset);
        Vector3 normalized = distanceVector.normalized;
        Vector3 direction = GetComponent<EnemyWalkingController>().direction;
        float angle = Vector3.Angle(distanceVector, direction);
        if(distanceVector.magnitude < range && angle < maxAngle)
        {
            Debug.DrawRay(transform.position + offset, distanceVector, Color.green, Time.deltaTime);
            //Debug.DrawLine(transform.position + offset, player.transform.position, Color.green, Time.deltaTime);
            RaycastHit hit;
            if (Physics.Raycast(transform.position + offset, distanceVector, out hit, range, layerMask))
            {
                if (GetComponent<EnemyCloseAttack>() != null)
                    GetComponent<EnemyCloseAttack>().StartAttack();
                else if (GetComponent<EnemyThrowAttack>() != null)
                    GetComponent<EnemyThrowAttack>().StartAttack(distanceVector, angle, offset);
                else if (GetComponent<EnemyLaserAttack>() != null)
                    GetComponent<EnemyLaserAttack>().StartAttack(angle);
            }
            else
            {
                GetComponent<EnemyWalkingController>().MoveUpdate();
            }
        }
        else
        {
            GetComponent<EnemyWalkingController>().MoveUpdate();
        }

        Vector3 targetLine = transform.position + offset + direction * range;
        Debug.DrawLine(transform.position + offset, targetLine, Color.red, Time.deltaTime);
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
