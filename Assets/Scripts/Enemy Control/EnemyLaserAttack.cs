using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserAttack : MonoBehaviour
{
    private float aimDuration, shootDuration, growSpeed, waitBetweenShoots;
    public GameObject myCannon, myLaser, myAim, burnParticlesPrefab;
    private EnemyHolder holder;
    private bool shooting, running, aiming;
    private float timeElapsed = 0;
    private GameObject burnParticles;
    private Difficulty difficulty;
    public Vector3 offset;
    public float rotationZoffset;

    [SerializeField] private SoundMap charge;
    [SerializeField] private SoundMap fire;
    private AudioSource chargeSource;
    private AudioSource fireSource;

    private void Start()
    {
        difficulty = Difficulty.current;
        aimDuration = difficulty.cockroach.aimDuration;
        shootDuration = difficulty.cockroach.shootDuration;
        growSpeed = difficulty.cockroach.growSpeed;
        waitBetweenShoots = difficulty.cockroach.shootCooldown;

        if (charge != null) {
            chargeSource = gameObject.AddComponent<AudioSource>();
            chargeSource.spatialBlend = charge.SpatialBlend;
        }
        if (fire != null) {
            fireSource = gameObject.AddComponent<AudioSource>();
            fireSource.spatialBlend = fire.SpatialBlend;
        }
    }

    private void Update()
    {
        if(shooting && timeElapsed < shootDuration)
        {
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
        else{
            if(burnParticles != null)
            {
                Destroy(burnParticles);
            }
        }
    }

    public void StartAttack(float angle)
    {
        if(!shooting && !aiming)
        {
            StartCoroutine(Aiming(angle));
            holder.enemyWalkingController.move = false;
            if (charge != null) {
                charge.Play(chargeSource);
            }
        }
    }

    public void TurnLaser(float angle)
    {
        if(running)
        {
            Quaternion cannonRot = myCannon.transform.rotation;
            if(GetComponent<EnemyWalkingController>().rotated)
                cannonRot = Quaternion.Euler(0, 180, angle);
            else
                cannonRot = Quaternion.Euler(0, 0, angle);
            myCannon.transform.rotation = cannonRot;
        }
    }

    private IEnumerator Aiming(float angle)
    {
        Quaternion cannonRot = myCannon.transform.rotation;
        if (GetComponent<EnemyWalkingController>().rotated)
            cannonRot = Quaternion.Euler(0, 180, angle);
        else
            cannonRot = Quaternion.Euler(0, 0, angle);
        myCannon.transform.rotation = cannonRot;
        //StartCoroutine(WaitForNextShoot());
        aiming = true;
        myAim.SetActive(true);
        //myAim.transform.position = transform.position + offset;
        //myAim.transform.rotation = Quaternion.Euler(0, 0, (90 - angle) * holder.enemyWalkingController.direction.x * -1);
        //myCannon.transform.Rotate(new Vector3(0, 0, angle + rotationZoffset));
        //myCannon.transform.rotation = Quaternion.Euler(0, 0, angle * holder.enemyWalkingController.direction.x * -1);
        yield return new WaitForSeconds(aimDuration);
        myAim.SetActive(false);
        shooting = true;

        // FIREEEEEEEEEEEEEEEEEEEEEEEEEEEE
        if (charge) {
            Sequence s = DOTween.Sequence();
            s.Append(chargeSource.DOFade(0, 0.1f));
            s.Play();
        }
        if (fire) {
            fire.Play(fireSource);
        }
        myLaser.SetActive(true);
        burnParticles = Instantiate(burnParticlesPrefab);

        //myLaser.transform.position = transform.position + offset;
        myLaser.transform.localScale = Vector3.one;
        //myLaser.transform.rotation = Quaternion.Euler(0, 0, (90 - angle) * holder.enemyWalkingController.direction.x * -1);
        aiming = false;
        GetComponent<Animator>().ResetTrigger("attack");
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
