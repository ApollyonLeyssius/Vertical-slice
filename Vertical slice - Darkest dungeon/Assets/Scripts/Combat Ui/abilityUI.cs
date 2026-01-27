using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class abilityUI : MonoBehaviour, IPointerDownHandler
{
    public int abilityIndex;

    [SerializeField] private Image abilityIcon;

    public void init(abilityData data)
    {
        abilityIcon.sprite = data.abilityIcon;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        battleManager.instance.SelectAbility(abilityIndex);
    }
}
