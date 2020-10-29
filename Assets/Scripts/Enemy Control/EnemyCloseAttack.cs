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
    private Difficulty difficulty => Difficulty.current;

    [SerializeField] private SoundMap sounds;
    private AudioSource source;

    private void Start()
    {
        //difficulty = Difficulty.current;
        animator = GetComponent<Animator>();
        killPlayer = weapon.GetComponent<KillPlayerOnHit>();
        killPlayer.shouldKill = false;
        animator.SetFloat("attackSpeed", difficulty.chef.attackSpeed);

        if (sounds != null) {
            source = gameObject.AddComponent<AudioSource>();
            source.spatialBlend = sounds.SpatialBlend;
        }
    }

    private void OnEnable() {
        Difficulty.OnDifficultyChange += UpdateDifficulty;
    }

    private void OnDisable() {
        Difficulty.OnDifficultyChange -= UpdateDifficulty;
    }

    public void UpdateDifficulty(Difficulty d) {
        animator.SetFloat("attackSpeed", d.chef.attackSpeed);
    }

    public void StartAttack()
    {
        if(!running) {
            StartCoroutine(Attack());
            if (sounds != null) {
                sounds.Play(source);
            }
        }
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
