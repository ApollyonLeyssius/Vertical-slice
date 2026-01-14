using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActivate : MonoBehaviour
{
    private float timePassed = 0f;
    private float negTime = 0.9f;

    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        negTime -= Time.deltaTime;
        timePassed += Time.deltaTime;

        

        if (timePassed > 0.6f) 
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, negTime);
        }

        if (timePassed > 1.5f)
        {
            Destroy(gameObject);
        }

    }
}
