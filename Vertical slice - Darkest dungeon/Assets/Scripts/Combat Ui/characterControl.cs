using UnityEngine;
using UnityEngine.EventSystems;


public class characterControl : MonoBehaviour
{
    public characterData CharacterData;
    public characterControl targetData;

    private void Awake()
    {
        CharacterData._charCont = this;
    }

    private void Start()
    {
        CharacterData.Init();
        CharacterData._target = targetData.CharacterData;

        battleManager.instance.allCharacters.Add(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Alleen als battleManager op een target wacht
        if (battleManager.instance.waitingForTarget)
        {
            battleManager.instance.TargetSelected(this);
        }
    }
}

