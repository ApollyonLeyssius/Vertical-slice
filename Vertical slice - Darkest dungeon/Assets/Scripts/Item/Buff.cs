using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Buff : MonoBehaviour
{
    private float speed = 3f;
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
        timePassed += Time.deltaTime;
        negTime -= Time.deltaTime;
        transform.position += Vector3.up * Time.deltaTime * speed;

        spriteRenderer.color = new Color(1f, 1f, 1f, negTime);

        if (timePassed > 1f) 
        { 
            Destroy(gameObject);
        }

    }
}
