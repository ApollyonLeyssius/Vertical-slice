using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class characterControl : MonoBehaviour, IPointerClickHandler
{
    public characterData CharacterData;
    public characterControl targetData;

    public Coroutine AttackQueue;

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

    private void attackRandomFriendlyCharacter()
    {
        if (CharacterData.characterType == CharacterType.Player)
        {
            // Add logic here to attack a random friendly character
        }
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

