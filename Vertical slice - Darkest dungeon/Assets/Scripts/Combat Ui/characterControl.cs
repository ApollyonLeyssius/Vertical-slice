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

        battleManager.instance.allCharacters.Add(this);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on: " + CharacterData.CharacterName);

        if (battleManager.instance.waitingForTarget)
        {
            battleManager.instance.TargetSelected(this);
        }
    }
}