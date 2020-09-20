using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThrowAttack : MonoBehaviour
{
    public float instantiateDistance, waitBetweenThrows;
    public GameObject platePrefab;
    private EnemyHolder holder;
    private bool running;
    private Difficulty difficulty;
    private Animator animator;

    [SerializeField] private SoundMap sounds;
    private AudioSource audioSource;

    private void Start() {
        animator = GetComponent<Animator>();
        difficulty = Difficulty.current;
        waitBetweenThrows /= difficulty.waiter.attackSpeed;
        animator.SetFloat("attackSpeed", difficulty.waiter.attackSpeed);
        if (sounds != null) {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = sounds.SpatialBlend;
        }
    }

    public void StartAttack(Vector3 direction, float angle, Vector3 offset)
    {
        if (!running) {
            StartCoroutine(Attack(direction, angle, offset));
            if (sounds != null) {
                sounds.Play(audioSource);
            }
        }
    }

    private IEnumerator Attack(Vector3 direction, float angle, Vector3 offset)
    {
        running = true;
        animator.SetTrigger("throw");
 
        yield return new WaitForSeconds(waitBetweenThrows/2);
        
        Vector3 instantiatePos = transform.position + offset + holder.enemyWalkingController.direction * instantiateDistance;
        GameObject newPlate = Instantiate(platePrefab, instantiatePos, Quaternion.identity);
        Vector3 throwDirection = new Vector3(direction.x - instantiateDistance * holder.enemyWalkingController.direction.x, direction.y, direction.z);

        var plateController = newPlate.GetComponent<PlateController>();
        plateController.direction = throwDirection ;
        plateController.toRotate = angle * holder.enemyWalkingController.direction.x;

        yield return new WaitForSeconds(waitBetweenThrows);

        animator.ResetTrigger("throw");

        yield return new WaitForSeconds(waitBetweenThrows/2);

        running = false;
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
