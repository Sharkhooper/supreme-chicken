using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalkingController : MonoBehaviour
{
    public LayerMask layerMask;
    private EnemyHolder holder;
    public float moveSpeed, turnRange;
    public Vector3 direction = new Vector3(1, 0, 0);
    public bool rotated = false, move = true;

    public void PlaceDown() {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 5))
        {
            holder.transform.position = hit.point + new Vector3(0, GetComponent<Renderer>().bounds.size.y / 2, 0);
        }
    }

    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, direction, out hit, turnRange, layerMask))
        {
            if (hit.transform.name != "Player" && hit.transform.name != "Plate(Clone)")
            {
                //Debug.Log("Not Player (turn): " + hit.transform.name);
                direction *= -1;
                transform.parent.RotateAround(transform.position, Vector3.up, 180);
                rotated = !rotated;
            }
        }
    }

    public void MoveUpdate()
    {
        if(move)
        {
            Vector3 newPos = holder.transform.position + direction * Time.deltaTime * moveSpeed;
            holder.transform.position = newPos;
        }

        //transform.position += direction * Time.deltaTime * moveSpeed;
        //holder.closeRangeController.gameObject.GetComponent<Rigidbody>().MovePosition(transform.parent.position + direction * Time.fixedDeltaTime * moveSpeed);
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
