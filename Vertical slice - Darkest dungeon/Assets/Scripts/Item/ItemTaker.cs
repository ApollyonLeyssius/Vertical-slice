using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTaker : MonoBehaviour
{
    [SerializeField] private GameObject ItemEffects;
    void Start()
    {
        Item.Clicked += Effects;
    }

    private void Effects()
    {
        GameObject effectGameObject = Instantiate(ItemEffects);
        effectGameObject.transform.position = transform.position;
    }
    void Update()
    {
        
    }
}
