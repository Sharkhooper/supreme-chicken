using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalkingController : MonoBehaviour
{
    private EnemyHolder holder;
    public float moveSpeed, turnRange;
    public Vector3 direction = new Vector3(1, 0, 0);
    public bool rotated = false;

    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, direction, out hit, turnRange))
        {
            if (hit.transform.name != "Player" && hit.transform.name != "Plate(Clone)")
            {
                Debug.Log("Not Player (turn): " + hit.transform.name);
                direction *= -1;
                transform.parent.RotateAround(transform.position, Vector3.up, 180);
                rotated = !rotated;
            }
        }
    }

    public void MoveUpdate()
    {
        transform.position += direction * Time.deltaTime * moveSpeed;
        //holder.closeRangeController.gameObject.GetComponent<Rigidbody>().MovePosition(transform.parent.position + direction * Time.fixedDeltaTime * moveSpeed);
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
