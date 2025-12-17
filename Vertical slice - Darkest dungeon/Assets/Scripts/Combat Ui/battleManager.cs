using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class battleManager : MonoBehaviour
{
    [HideInInspector]
    public abilityData selectedAbility;

    public bool waitingForTarget = false;

    public static battleManager instance;

    public List<characterControl> allCharacters = new List<characterControl>();
    private Queue<characterControl> turnOrder = new Queue<characterControl>();

    public characterControl currentCharacter;
    public List<characterControl> friendlyCharacters = new List<characterControl>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        friendlyCharacters = allCharacters.FindAll(x => x.CharacterData.characterType == CharacterType.Player);
        GenerateTurnOrder();
        NextTurn();
    }

    public void GenerateTurnOrder()
    {
        allCharacters.Sort((a, b) =>
            b.CharacterData.CharacterSpeed.CompareTo(a.CharacterData.CharacterSpeed));

        foreach (var c in allCharacters)
        {
            turnOrder.Enqueue(c);
        }
    }

    public void NextTurn()
    {
        currentCharacter = turnOrder.Dequeue();

        if (!currentCharacter.CharacterData.IsAlive)
        {
            NextTurn();
            return;
        }

        currentCharacter.CharacterData.characterState = CharacterState.Ready;
        Debug.Log("Turn: " + currentCharacter.CharacterData.CharacterName);

        if (currentCharacter.CharacterData.characterType == CharacterType.Enemy)
        {
            StartCoroutine(EnemyTurn());
        }
        else
        {
            // Player turn → UI aanzetten
            waitingForTarget = false;
        }

        turnOrder.Enqueue(currentCharacter);
    }

    // UI → knop roept deze functie aan
    public void DoBasicAttackFromButton()
    {
        // Alleen player mag dit doen
        if (currentCharacter.CharacterData.characterType != CharacterType.Player)
            return;

        // Zet basic attack als geselecteerde ability
        selectedAbility = currentCharacter.CharacterData.basicAttack;

        // Wacht op enemy klik
        waitingForTarget = true;

        Debug.Log("Basic Attack selected, waiting for target...");
    }

    public void SelectAbility(int index)
    {
        var data = currentCharacter.CharacterData.Abilities;
        selectedAbility = data[index];

        waitingForTarget = true;
        Debug.Log("Selected ability: " + selectedAbility.abilityName);
    }

    public void TargetSelected(characterControl target)
    {
        // Alleen doorgaan als we echt targeten
        if (!waitingForTarget || selectedAbility == null)
            return;

        waitingForTarget = false;

        // Zet target
        currentCharacter.CharacterData._target = target.CharacterData;

        // Voer ability uit (basic attack of andere)
        currentCharacter.CharacterData.Attack(selectedAbility);

        Debug.Log(
            $"{currentCharacter.CharacterData.CharacterName} gebruikt {selectedAbility.abilityName} op {target.CharacterData.CharacterName}"
        );

        selectedAbility = null;

        // Volgende beurt
        NextTurn();
    }

    private IEnumerator EnemyTurn()
    {
        // Kleine delay zodat het leesbaar is
        yield return new WaitForSeconds(0.8f);

        var enemyData = currentCharacter.CharacterData;

        // Kies ability
        abilityData ability = enemyData.basicAttack;

        // Kies random levende player
        var targets = allCharacters.FindAll(x =>
            x.CharacterData.characterType == CharacterType.Player &&
            x.CharacterData.IsAlive);

        if (targets.Count == 0)
            yield break;

        var target = targets[Random.Range(0, targets.Count)];

        enemyData._target = target.CharacterData;
        enemyData.Attack(ability);

        Debug.Log($"{enemyData.CharacterName} gebruikt {ability.abilityName} op {target.CharacterData.CharacterName}");

        yield return new WaitForSeconds(0.5f);

        NextTurn();
    }

}
