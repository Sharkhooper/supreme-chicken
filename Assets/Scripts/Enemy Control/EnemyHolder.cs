using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHolder : MonoBehaviour
{
    public EnemyCloseAttack enemyCloseAttack;
    public EnemyWalkingController enemyWalkingController;
    public CloseRangeController closeRangeController;

    void Start()
    {
        enemyCloseAttack.SetHolder(this);
        enemyWalkingController.SetHolder(this);
        closeRangeController.SetHolder(this);
    }
}
