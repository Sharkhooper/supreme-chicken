using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeController : MonoBehaviour
{
    private EnemyHolder holder;
    public float range;
    public GameObject player;

    void Update()
    {
        if(holder != null)
        {
            Vector3 fwd;
            Debug.Log("holder " + holder);
            Debug.Log("enemyWalkingController " + holder.enemyWalkingController);
            Debug.Log("rotated " + holder.enemyWalkingController.rotated);
            if (!holder.enemyWalkingController.rotated)
                fwd = Vector3.right;
            else
                fwd = Vector3.left;

            RaycastHit[] hits = Physics.RaycastAll(transform.position, fwd, range);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.name == "Player")
                    holder.enemyCloseAttack.StartAttack();
            }

            Debug.DrawLine(transform.position, transform.position + fwd * range, Color.red, Time.deltaTime);
        }
    }

    public void SetHolder(EnemyHolder h)
    {
        holder = h;
    }
}
