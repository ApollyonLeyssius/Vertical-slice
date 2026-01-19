using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battleManager : MonoBehaviour
{
    [HideInInspector]
    public abilityData selectedAbility;

    public bool waitingForTarget = false;

    public static battleManager instance;
    public uiManagment uiManager;

    public List<characterControl> allCharacters = new List<characterControl>();
    private Queue<characterControl> turnOrder = new Queue<characterControl>();

    public characterControl currentCharacter;
    public List<characterControl> friendlyCharacters = new List<characterControl>();

    private bool turnInProgress = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartBattleCoroutine());
    }

    private IEnumerator StartBattleCoroutine()
    {
        // Wacht één frame zodat alle characterControl.Start() klaar zijn
        yield return null;

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
        foreach (var c in allCharacters)
        {
            if (c != null && c.turnIndicator != null)
                c.turnIndicator.SetActive(false);
        }

        if (currentCharacter != null && currentCharacter.turnIndicator != null)
            currentCharacter.turnIndicator.SetActive(true);

        currentCharacter = turnOrder.Dequeue();
        currentCharacter.turnIndicator.SetActive(true);

        if (!currentCharacter.CharacterData.IsAlive)
        {
            currentCharacter.turnIndicator.SetActive(true);
            NextTurn();
            return;
        }

        currentCharacter.CharacterData.characterState = CharacterState.Ready;

        Debug.Log("Turn: " + currentCharacter.CharacterData.CharacterName);

        if (currentCharacter.CharacterData.characterType == CharacterType.Player)
        {
            waitingForTarget = false;
            selectedAbility = null;

            uiManager.ShowAbilities(currentCharacter.CharacterData);
        }
        else 
        {
            uiManager.HideAbilities();
            waitingForTarget = false;

            StartCoroutine(EnemyTurn());
        }

        turnOrder.Enqueue(currentCharacter);
    }

    public void DoBasicAttackFromButton()
    {
        if (currentCharacter.CharacterData.characterType != CharacterType.Player)
            return;

        selectedAbility = currentCharacter.CharacterData.basicAttack;

        waitingForTarget = true;

        Debug.Log("Basic Attack selected, waiting for target...");
    }

    public void SelectAbility(int index)
    {
        var data = currentCharacter.CharacterData;
        var ability = data.Abilities[index];

        // Check: mag deze ability vanaf deze positie?
        if (!ability.usableFromPositions.Contains(data.position))
        {
            Debug.Log("Ability cannot be used from this position!");
            return;
        }

        selectedAbility = ability;
        waitingForTarget = true;
    }
    public void TargetSelected(characterControl target)
    {
        var attacker = currentCharacter.CharacterData;
        var targetData = target.CharacterData;

        // Check: mag dit target geraakt worden?
        if (!selectedAbility.validTargetPositions.Contains(targetData.position))
        {
            Debug.Log("Invalid target position!");
            return;
        }

        AttackCameraController.Instance.PlayAttackByIndex(2, 1);

        attacker._target = targetData;
        attacker.Attack(selectedAbility);

        NextTurn();
    }

    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(0.5f); // kleine denkpauze

        var enemy = currentCharacter.CharacterData;

        // Kies ability (voor nu gewoon eerste)
        abilityData chosenAbility = enemy.Abilities[0];

        // Zoek levende player targets
        List<characterControl> possibleTargets = allCharacters.FindAll(c =>
            c.CharacterData.characterType == CharacterType.Player &&
            c.CharacterData.IsAlive
        );

        if (possibleTargets.Count == 0)
        {
            Debug.Log("All players dead!");
            yield break;
        }

        // Kies random target
        characterControl target = possibleTargets[Random.Range(0, possibleTargets.Count)];

        // Zet target
        enemy._target = target.CharacterData;

        // Voer attack uit
        enemy.Attack(chosenAbility);

        Debug.Log(enemy.CharacterName + " attacks " + target.CharacterData.CharacterName);

        yield return new WaitForSeconds(0.5f);

        NextTurn();
    }

}
