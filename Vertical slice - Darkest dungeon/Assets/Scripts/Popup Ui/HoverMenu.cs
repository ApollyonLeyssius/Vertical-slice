using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

// this script needs box collider2D
[RequireComponent(typeof(BoxCollider2D))]
public class HoverMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    [SerializeField] private Animator anim;

    private void Start()
    {
    }

    private void OnMouseOver()
    {
        // checks if its off
        if (!menu.activeSelf)
        {
            // turns on
            anim.SetTrigger("TurningOn");
            menu.SetActive(true);
            Debug.Log("menu turned on");
        }
    }

    private void OnMouseExit()
    {
        // checks if its on
        if (menu.activeSelf)
        {
            // turns off
            anim.SetTrigger("TurningOff");
            menu.SetActive(false);

            Debug.Log("menu turned off");
        }
    }
}