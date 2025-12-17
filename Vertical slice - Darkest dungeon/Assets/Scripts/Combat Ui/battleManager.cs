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
        var data = currentCharacter.CharacterData;

        if (data.characterState != CharacterState.Ready)
            return;

        if (data._target.canBeAttacked)
        {
            data.Attack(data.basicAttack);
        }

        NextTurn();
    }

    public void EnemyBasicAttack()
    {
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
        if (!waitingForTarget) return;

        waitingForTarget = false;

        // Voer ability uit
        currentCharacter.CharacterData._target = target.CharacterData;
        currentCharacter.CharacterData.Attack(selectedAbility);

        // Reset
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
