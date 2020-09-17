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
        running = true;
        GetComponent<Animator>().SetTrigger("hit");
        yield return new WaitForSeconds(2.25f / 2f);
        GetComponent<Animator>().ResetTrigger("hit");
        running = false;
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
