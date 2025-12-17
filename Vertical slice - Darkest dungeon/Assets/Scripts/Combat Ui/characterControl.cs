using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class characterControl : MonoBehaviour, IPointerClickHandler
{
    public characterData CharacterData;
    public characterControl targetData;

    public Coroutine AttackQueue;
    public Coroutine Enemybehaviour;

    private void Awake()
    {
        CharacterData._charCont = this;
    }

    private void Start()
    {
        CharacterData.Init();
        CharacterData._target = targetData.CharacterData;

        battleManager.instance.allCharacters.Add(this);
     /*   if (CharacterData.characterType == CharacterType.Enemy)
        {
            Enemybehaviour = StartCoroutine(AttackRandomFriendly());
        }*/
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

    public IEnumerator AttackRandomFriendly()
    {
        if (CharacterData.characterType == CharacterType.Enemy)
        {
            yield return new WaitForSeconds(1f); // Wacht een seconde voor de aanval
            var friendlyCharacters = battleManager.instance.friendlyCharacters;
            if (friendlyCharacters.Count > 0)
            {
                int randomIndex = Random.Range(0, friendlyCharacters.Count);
                var target = friendlyCharacters[randomIndex];
                Debug.Log($"{CharacterData.CharacterName} valt {target.CharacterData.CharacterName} aan!");
                // Voeg hier de logica toe om schade toe te brengen aan het doelwit
                characterData targetData = target.CharacterData;
                battleManager.instance.selectedAbility = CharacterData.basicAttack;

            }
        }
    }
}