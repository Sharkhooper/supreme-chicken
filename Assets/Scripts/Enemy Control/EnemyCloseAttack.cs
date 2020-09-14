using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCloseAttack : MonoBehaviour
{
    private EnemyHolder holder;
    private bool running;
    private void Start() {
        
    }

    public void StartAttack()
    {
        if(!running)
            StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        running = holder.enemyWalkingController.attacking = true;
        transform.localScale *= 1.1f;
        yield return new WaitForSeconds(0.5f);
        transform.localScale /= 1.1f;
        yield return new WaitForSeconds(0.5f);
        running = holder.enemyWalkingController.attacking = false;
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
