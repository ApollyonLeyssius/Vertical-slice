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

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GenerateTurnOrder();
        NextTurn();
    }

    public void GenerateTurnOrder()
    {
        // Sorteer op Speed
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

        Debug.Log("Turn: " + currentCharacter.CharacterData.CharacterName);

        // Zet in actieve beurt
        currentCharacter.CharacterData.characterState = CharacterState.Ready;

        // Zet character terug in de queue voor volgende rondes
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

}
