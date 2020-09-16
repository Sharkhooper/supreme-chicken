using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserAttack : MonoBehaviour
{
    public float aimDuration, shootDuration, growSpeed, waitBetweenShoots, degreesAbove;
    public GameObject myLaser, myAim, burnParticlesPrefab;
    private EnemyHolder holder;
    private bool shooting, running, aiming;
    private float timeElapsed = 0;
    private GameObject burnParticles;

    private void Update() {
        //Debug.Log("Time elapsed: " + timeElapsed);
        if(shooting && timeElapsed < shootDuration)
        {
            //Quaternion currentRotation = myLaser.transform.rotation;
            //myLaser.transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotationSpeed * holder.enemyWalkingController.direction.x * -1));
            float change = Time.deltaTime * growSpeed;
            myLaser.transform.localScale += new Vector3(change, 0, change);
            burnParticles.transform.position = myLaser.GetComponentInChildren<ScaleToCollision>().GetHitPosition();
            timeElapsed += Time.deltaTime;
        }
        else if (!running && shooting && timeElapsed >= shootDuration)
        {
            Destroy(burnParticles);
            myLaser.SetActive(false);
            holder.enemyWalkingController.move = true;
            StartCoroutine(WaitForNextShoot());
        }
    }

    public void StartAttack(float angle)
    {
        if(!shooting && !aiming)
        {
            StartCoroutine(Aiming(angle));
            holder.enemyWalkingController.move = false;
        }
    }

    private IEnumerator Aiming(float angle)
    {
        aiming = true;
        myAim.SetActive(true);
        myAim.transform.position = transform.position;
        myAim.transform.rotation = Quaternion.Euler(0, 0, (90 - angle) * holder.enemyWalkingController.direction.x * -1);
        yield return new WaitForSeconds(aimDuration);
        myAim.SetActive(false);
        shooting = true;
        myLaser.SetActive(true);
        burnParticles = Instantiate(burnParticlesPrefab);

        myLaser.transform.position = transform.position;
        myLaser.transform.localScale = Vector3.one;
        myLaser.transform.rotation = Quaternion.Euler(0, 0, (90 - angle) * holder.enemyWalkingController.direction.x * -1);
        aiming = false;
    }

    private IEnumerator WaitForNextShoot()
    {
        running = true;
        yield return new WaitForSeconds(waitBetweenShoots);
        shooting = false;
        timeElapsed = 0;
        running = false;
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
