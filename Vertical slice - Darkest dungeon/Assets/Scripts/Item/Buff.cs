using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    private float speed = 3f;
    private float timePassed = 0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        transform.position += Vector3.up * Time.deltaTime * speed;

        if (timePassed > 0.5f) 
        { 
            Destroy(gameObject);
        }

    }
}
