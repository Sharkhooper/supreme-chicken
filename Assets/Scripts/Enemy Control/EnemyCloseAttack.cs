using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCloseAttack : MonoBehaviour
{
    public GameObject weapon;
    private EnemyHolder holder;
    private bool running;
    private KillPlayerOnHit killPlayer;
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        killPlayer = weapon.GetComponent<KillPlayerOnHit>();
        killPlayer.shouldKill = false;
    }

    public void StartAttack()
    {
        if(!running)
            StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        running = true;
        animator.SetTrigger("hit");
        killPlayer.shouldKill = true;
        yield return new WaitForSeconds(2.25f / 2f);
        killPlayer.shouldKill = false;
        animator.ResetTrigger("hit");
        running = false;
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
