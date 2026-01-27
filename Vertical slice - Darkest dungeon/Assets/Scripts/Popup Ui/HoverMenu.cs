using UnityEngine;

// Needs a Collider2D on this object (and an EventSystem is NOT required for OnMouse...).
[RequireComponent(typeof(BoxCollider2D))]
public class HoverMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    private void Awake()
    {
        if (menu != null) menu.SetActive(false); // start hidden
    }

    private void OnMouseEnter()
    {
        if (menu != null) menu.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (menu != null) menu.SetActive(false);
    }
}
