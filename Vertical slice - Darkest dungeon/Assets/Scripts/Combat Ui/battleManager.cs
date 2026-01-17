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

    private bool turnInProgress;

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

        friendlyCharacters = allCharacters.FindAll(
            x => x.CharacterData.characterType == CharacterType.Player
        );

        GenerateTurnOrder();
        NextTurn();
    }

    public void GenerateTurnOrder()
    {
        turnOrder.Clear();

        allCharacters.Sort((a, b) =>
            b.CharacterData.CharacterSpeed.CompareTo(a.CharacterData.CharacterSpeed));

        foreach (var c in allCharacters)
            turnOrder.Enqueue(c);
    }

    public void NextTurn()
    {
        if (turnInProgress)
            return;

        currentCharacter = turnOrder.Dequeue();

        if (!currentCharacter.CharacterData.IsAlive)
        {
            NextTurn();
            return;
        }

        currentCharacter.CharacterData.characterState = CharacterState.Ready;

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
    }

    public void SelectAbility(int index)
    {
        var data = currentCharacter.CharacterData;
        var ability = data.Abilities[index];

        if (!ability.usableFromPositions.Contains(data.position))
            return;

        selectedAbility = ability;
        waitingForTarget = true;
    }

    public void TargetSelected(characterControl target)
    {
        if (turnInProgress || selectedAbility == null)
            return;

        var attacker = currentCharacter.CharacterData;
        var targetData = target.CharacterData;

        if (!selectedAbility.validTargetPositions.Contains(targetData.position))
            return;

        turnInProgress = true;

        attacker._target = targetData;

        if (AttackCameraController.Instance != null)
        {
            AttackCameraController.Instance.PlayAttack(
                attacker._charCont.transform,
                target.transform
            );
        }

        attacker.Attack(selectedAbility);

        StartCoroutine(EndTurnAfterDelay(0.25f));
    }

    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(0.6f);

        if (!currentCharacter.CharacterData.IsAlive)
        {
            NextTurn();
            yield break;
        }

        var enemyData = currentCharacter.CharacterData;
        abilityData ability = enemyData.basicAttack;

        var targets = allCharacters.FindAll(x =>
            x.CharacterData.characterType == CharacterType.Player &&
            x.CharacterData.IsAlive);

        if (targets.Count == 0)
        {
            NextTurn();
            yield break;
        }

        var target = targets[Random.Range(0, targets.Count)];

        turnInProgress = true;

        if (AttackCameraController.Instance != null)
        {
            AttackCameraController.Instance.PlayAttack(
                currentCharacter.transform,
                target.transform
            );
        }

        enemyData._target = target.CharacterData;
        enemyData.Attack(ability);

        yield return new WaitForSeconds(0.25f);
        turnInProgress = false;
        NextTurn();
    }

    private IEnumerator EndTurnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        turnInProgress = false;
        NextTurn();
    }
}