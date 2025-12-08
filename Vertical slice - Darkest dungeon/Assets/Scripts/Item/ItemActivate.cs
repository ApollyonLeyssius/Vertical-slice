using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActivate : MonoBehaviour
{
    private float timePassed = 0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        timePassed += Time.deltaTime;
        if (timePassed > 1.5f)
        {
            Destroy(gameObject);
        }

    }
}
