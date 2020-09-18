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
    private Difficulty difficulty;
    private void Start()
    {
        difficulty = Difficulty.current;
        animator = GetComponent<Animator>();
        killPlayer = weapon.GetComponent<KillPlayerOnHit>();
        killPlayer.shouldKill = false;
        animator.SetFloat("attackSpeed", difficulty.chef.attackSpeed);
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
        yield return new WaitForSeconds(2.25f / difficulty.chef.attackSpeed);
        killPlayer.shouldKill = false;
        animator.ResetTrigger("hit");
        running = false;
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
