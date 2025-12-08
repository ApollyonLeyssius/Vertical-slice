using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class abilityUI : MonoBehaviour, IPointerDownHandler
{
    public int abilityIndex = 0;
    [SerializeField] private TextMeshProUGUI abilityName;

    public void init(string abilityName)
    {
        this.abilityName.text = abilityName;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Ability selecteren in battleManager
        battleManager.instance.SelectAbility(abilityIndex);
    }
 }
