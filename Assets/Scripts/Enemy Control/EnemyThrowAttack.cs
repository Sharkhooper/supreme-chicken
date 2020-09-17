using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThrowAttack : MonoBehaviour
{
    public float instantiateDistance, waitBetweenThrows;
    public GameObject platePrefab;
    private EnemyHolder holder;
    private bool running;

    public void StartAttack(Vector3 direction, float angle, Vector3 offset)
    {
        if (!running)
            StartCoroutine(Attack(direction, angle, offset));
    }

    private IEnumerator Attack(Vector3 direction, float angle, Vector3 offset)
    {
        running = true;
        GetComponent<Animator>().SetTrigger("throw");
        yield return new WaitForSeconds(waitBetweenThrows/2);
        Vector3 instantiatePos = transform.position + offset + holder.enemyWalkingController.direction * instantiateDistance;
        GameObject newPlate = Instantiate(platePrefab, instantiatePos, Quaternion.identity);
        Vector3 throwDirection = new Vector3(direction.x - instantiateDistance * holder.enemyWalkingController.direction.x, direction.y, direction.z);
        newPlate.GetComponent<PlateController>().direction = throwDirection ;
        newPlate.GetComponent<PlateController>().toRotate = angle * holder.enemyWalkingController.direction.x;
        yield return new WaitForSeconds(waitBetweenThrows);
        GetComponent<Animator>().ResetTrigger("throw");
        yield return new WaitForSeconds(waitBetweenThrows/2);
        running = false;
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
