using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class characterControl : MonoBehaviour, IPointerClickHandler
{
    public characterData CharacterData;
    public characterControl targetData;

    public Coroutine AttackQueue;
    public Coroutine Enemybehaviour;

    public Transform damagePopupPoint;
    public GameObject damagePopupPrefab;

    public GameObject turnIndicator;

    private void Awake()
    {
        CharacterData._charCont = this;
    }

    public void ShowDamagePopup(int amount)
    {
        var go = Instantiate(damagePopupPrefab, damagePopupPoint.position, Quaternion.identity, damagePopupPoint);
        go.GetComponent<DamagePopup>().Setup(amount);
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