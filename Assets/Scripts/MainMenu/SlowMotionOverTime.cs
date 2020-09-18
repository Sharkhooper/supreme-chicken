using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionOverTime : MonoBehaviour
{
    public float afterSeconds;
    public float scale;

    private float timer;
    private bool running, done;

    void Start()
    {
        
    }

    private void Update() {
        if(running)
        {
            if(Time.timeScale > scale && !done)
            {
                Time.timeScale -= Time.deltaTime * Time.timeScale;
            }
            else
            {
                done = true;
                if(Time.deltaTime + Time.timeScale > 1)
                {
                    running = false;
                    Time.timeScale = 1;
                    timer = 0;
                }
                Time.timeScale += Time.deltaTime * Time.timeScale;
            }
        }
    }

    public void StartScaling()
    {
        StartCoroutine(TimeScaling());
    }

    private IEnumerator TimeScaling()
    {
        yield return new WaitForSeconds(afterSeconds);
        running = true;
    }
}
