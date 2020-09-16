using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToCollision : MonoBehaviour
{
    public Transform originTrans;
    public float defaultLength;
    private Vector3 hitPosition;

    void Update()
    {
        Vector3 myDirection = transform.parent.rotation * Vector3.up;
        RaycastHit hit;
        Vector3 origin = originTrans.position;
        Debug.DrawRay(origin,myDirection, Color.green, Time.deltaTime);
        if(Physics.Raycast(origin, myDirection * 100, out hit, 100))
        {
            float size = Vector3.Distance(hit.point, origin);
            gameObject.transform.localScale = new Vector3(transform.localScale.x, size/2, transform.localScale.z);
            gameObject.transform.localPosition = new Vector3(0, size/2, 0);
            hitPosition = hit.point;
        }
        else
        {
            gameObject.transform.localScale = new Vector3(transform.localScale.x, defaultLength, transform.localScale.z);
            gameObject.transform.localPosition = new Vector3(0, defaultLength, 0);
            hitPosition = Vector3.zero;
        }
    }

    public Vector3 GetHitPosition()
    {
        return hitPosition;
    }
}
