﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToCollision : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform originTrans;
    public float defaultLength;
    private Vector3 hitPosition;


    void Update()
    {
        Vector3 myDirection = transform.parent.rotation * Vector3.up;
        RaycastHit hit;
        Vector3 origin = originTrans.position;
        //Debug.DrawRay(origin,myDirection, Color.green, Time.deltaTime);
        if(Physics.Raycast(origin, myDirection, out hit, 100, layerMask))
        {
            float size = Vector3.Distance(hit.point, origin);
            gameObject.transform.localScale = new Vector3(transform.localScale.x, size / 2, transform.localScale.z);
            gameObject.transform.localPosition = new Vector3(0, size / 2, 0);
            hitPosition = hit.point;
            //Debug.Log("Hitted " + hit.transform.name);
        }
        else
        {
            //Debug.Log("Default length");
            gameObject.transform.localScale = new Vector3(transform.localScale.x, defaultLength, transform.localScale.z);
            gameObject.transform.localPosition = new Vector3(0, defaultLength, 0);
            hitPosition = Vector3.zero;
        }
    }

/*
    void Update()
    {
        Vector3 myDirection = transform.parent.rotation * Vector3.up;
        Vector3 origin = originTrans.position;
        RaycastHit[] hits = Physics.RaycastAll(origin, myDirection, 100);
        RaycastHit hit = new RaycastHit();
        bool foundOne = false;
        Debug.Log("# of hits: " + hits.Length);
        for (int i = 0; i < hits.Length; i++)
        {
            Debug.Log("Number " + i + " is " + hits[i].transform.name);
            if(hits[i].transform.gameObject.layer != 8)
            {
                hit = hits[i];
                foundOne = true;
                break;
            }
            else{
                Debug.Log("IS INVISIBLE WALL");
            }
            foundOne = false;
        }

        if (foundOne)
        {
            float size = Vector3.Distance(hit.point, origin);
            gameObject.transform.localScale = new Vector3(transform.localScale.x, size / 2, transform.localScale.z);
            gameObject.transform.localPosition = new Vector3(0, size / 2, 0);
            hitPosition = hit.point;
        }
        else
        {
            gameObject.transform.localScale = new Vector3(transform.localScale.x, defaultLength, transform.localScale.z);
            gameObject.transform.localPosition = new Vector3(0, defaultLength, 0);
            hitPosition = Vector3.zero;
        }
    }
*/
    public Vector3 GetHitPosition()
    {
        return hitPosition;
    }
}
