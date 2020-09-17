using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHolder : MonoBehaviour
{
    public EnemyCloseAttack enemyCloseAttack;
    public EnemyThrowAttack enemyThrowAttack;
    public EnemyLaserAttack enemyLaserAttack;
    public EnemyWalkingController enemyWalkingController;
    public CloseRangeController closeRangeController;

    void Start()
    {
        closeRangeController.SetHolder(this);
        if(enemyCloseAttack != null)
            enemyCloseAttack.SetHolder(this);
        if(enemyThrowAttack != null)
            enemyThrowAttack.SetHolder(this);
        if (enemyLaserAttack != null)
            enemyLaserAttack.SetHolder(this);
        enemyWalkingController.SetHolder(this);
        enemyWalkingController.PlaceDown();
    }
}
