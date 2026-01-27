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
        yield return null;

        GenerateTurnOrder();
        NextTurn();
    }

    public void GenerateTurnOrder()
    {
        turnOrder.Clear();

        allCharacters.Sort((a, b) =>
            b.CharacterData.CharacterSpeed.CompareTo(a.CharacterData.CharacterSpeed));

        foreach (var c in allCharacters)
        {
            if (c != null)
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

        if (turnOrder.Count == 0)
            GenerateTurnOrder();

        currentCharacter = turnOrder.Dequeue();

        while (currentCharacter != null && !currentCharacter.CharacterData.IsAlive)
        {
            if (turnOrder.Count == 0)
                GenerateTurnOrder();

            currentCharacter = turnOrder.Dequeue();
        }

        if (currentCharacter != null && currentCharacter.turnIndicator != null)
            currentCharacter.turnIndicator.SetActive(true);

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

        if (!ability.usableFromPositions.Contains(data.position))
        {
            Debug.Log("Ability cannot be used from this position!");
            return;
        }

        selectedAbility = ability;
        waitingForTarget = true;
    }

    void TryPlayAttackCamera(characterData attacker, characterData target)
    {
        if (AttackCameraController.Instance == null) return;
        if (attacker == null || target == null) return;

        if (attacker.characterType == CharacterType.Player && target.characterType == CharacterType.Enemy)
        {
            int allyIndex = attacker.position;
            int enemyIndex = target.position;
            if (allyIndex < 0 || allyIndex > 3) return;
            if (enemyIndex < 0 || enemyIndex > 3) return;
            AttackCameraController.Instance.PlayAttackByIndex(allyIndex, enemyIndex);
            return;
        }

        if (attacker.characterType == CharacterType.Enemy && target.characterType == CharacterType.Player)
        {
            int enemyIndex = attacker.position;
            int allyIndex = target.position;
            if (allyIndex < 0 || allyIndex > 3) return;
            if (enemyIndex < 0 || enemyIndex > 3) return;
            AttackCameraController.Instance.PlayEnemyAttackByIndex(enemyIndex, allyIndex);
            return;
        }
    }

    IEnumerator WaitForCameraIfNeeded()
    {
        if (AttackCameraController.Instance == null)
            yield break;

        while (AttackCameraController.Instance.IsAttacking)
            yield return null;
    }

    public void TargetSelected(characterControl target)
    {
        var attacker = currentCharacter.CharacterData;
        var targetData = target.CharacterData;

        if (selectedAbility == null)
            return;

        if (!selectedAbility.validTargetPositions.Contains(targetData.position))
        {
            Debug.Log("Invalid target position!");
            return;
        }

        waitingForTarget = false;

        TryPlayAttackCamera(attacker, targetData);

        attacker._target = targetData;
        attacker.Attack(selectedAbility);

        StartCoroutine(EndTurnAfterCamera());
    }

    IEnumerator EndTurnAfterCamera()
    {
        yield return WaitForCameraIfNeeded();
        NextTurn();
    }

    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(0.5f);

        var enemy = currentCharacter.CharacterData;

        abilityData chosenAbility = enemy.Abilities[0];

        List<characterControl> possibleTargets = allCharacters.FindAll(c =>
            c.CharacterData.characterType == CharacterType.Player &&
            c.CharacterData.IsAlive
        );

        if (possibleTargets.Count == 0)
        {
            Debug.Log("All players dead!");
            yield break;
        }

        characterControl target = possibleTargets[Random.Range(0, possibleTargets.Count)];

        enemy._target = target.CharacterData;

        TryPlayAttackCamera(enemy, target.CharacterData);

        enemy.Attack(chosenAbility);

        Debug.Log(enemy.CharacterName + " attacks " + target.CharacterData.CharacterName);

        yield return WaitForCameraIfNeeded();

        yield return new WaitForSeconds(0.5f);

        NextTurn();
    }
}
