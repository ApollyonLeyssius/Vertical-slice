using UnityEngine;

public class uiManagment : MonoBehaviour
{
    public Transform abilityUiHolder;
    public GameObject abilityUiPrefab;


    public void Abilitywindow(characterData data)
    {
        // Eerst oude knoppen verwijderen
        foreach (Transform child in abilityUiHolder)
        {
            Destroy(child.gameObject);
        }

        // Nieuwe ability knoppen aanmaken
        for (int i = 0; i < data.Abilities.Count; i++)
        {
            var ability = data.Abilities[i];

            GameObject btn = Instantiate(abilityUiPrefab, abilityUiHolder);
            var ui = btn.GetComponent<abilityUI>();

            ui.abilityIndex = i;
            ui.init(ability.abilityName);
        }
    }
}
