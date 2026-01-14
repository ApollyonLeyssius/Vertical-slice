using UnityEngine;

public class uiManagment : MonoBehaviour
{
    public Transform abilityUiHolder;
    public GameObject abilityUiPrefab;

    public GameObject abilityPanel; // hele ability UI (panel)

    public void ShowAbilities(characterData data)
    {

        abilityPanel.SetActive(true);

        // Oude abilities verwijderen
        foreach (Transform child in abilityUiHolder)
        {
            Destroy(child.gameObject);
        }

        // Nieuwe abilities maken
        for (int i = 0; i < data.Abilities.Count; i++)
        {
            var ability = data.Abilities[i];
            var go = Instantiate(abilityUiPrefab, abilityUiHolder);
            var ui = go.GetComponent<abilityUI>();

            ui.abilityIndex = i;
            ui.init(ability);
        }
    }

    public void HideAbilities()
    {
        abilityPanel.SetActive(false);
    }
}
