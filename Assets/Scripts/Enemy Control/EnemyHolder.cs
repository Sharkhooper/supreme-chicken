﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHolder : MonoBehaviour
{
    public EnemyCloseAttack enemyCloseAttack;
    public EnemyThrowAttack enemyThrowAttack;
    public EnemyWalkingController enemyWalkingController;
    public CloseRangeController closeRangeController;

    void Start()
    {
        if(enemyCloseAttack != null)
            enemyCloseAttack.SetHolder(this);
        if(enemyThrowAttack != null)
            enemyThrowAttack.SetHolder(this);
        enemyWalkingController.SetHolder(this);
        closeRangeController.SetHolder(this);
    }
}
