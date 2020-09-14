using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalkingController : MonoBehaviour
{
    private EnemyHolder holder;
    public GameObject model;
    public float moveSpeed;
    public Vector3 direction = new Vector3(1, 0, 0);
    public bool rotated = false;
    public bool attacking;

    void Update()
    {
        if(!attacking)
            transform.parent.position += direction * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name != "Player" && other.name != "Enemy")
        {
            Debug.Log("Not Player: " + other.name);
            direction *= -1;
            transform.parent.RotateAround(model.transform.position, Vector3.up, 180);
            rotated = !rotated;
        }
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
