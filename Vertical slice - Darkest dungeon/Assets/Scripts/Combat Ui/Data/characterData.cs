using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Diagnostics;

[Serializable]
public class characterData
{
    public string CharacterName;

    public int maxCharacterHealth;
    public int CurrentHealth;

    public float CharacterSpeed;
    public float CurrentSpeed;

    public CharacterState characterState;
    public CharacterType characterType;

    [Space(10)]

    public characterData _target;

    [Space(10)]

    public characterUIData CharUI;

    [Space(10)]

    public List<abilityData> Abilities;
    public abilityData basicAttack;

    [Space(10)]

    [HideInInspector]
    public characterControl _charCont;

    public UnityEvent OnAttack;
    public UnityEvent OnWasAttacked;



    public bool PlayerJustAttacked;
    public bool canAttack
    {
        get
        {
            return _target.characterState == CharacterState.Idle  || _target.characterState == CharacterState.Ready;
        }
    }

    public bool isReadyForAction
    {
        get
        {
            return CurrentSpeed >= CharacterSpeed;
        }

    }

    public bool canBeAttacked
    {
        get
        {
            return characterState == CharacterState.Idle || characterState == CharacterState.Ready;
        }
    }

    public void Init()
    {
        if (characterType == CharacterType.Player)
        {
            CharUI.Init(maxCharacterHealth, CurrentHealth, CharacterName);
        }
        else
        {
            CharUI.InitEnemy(maxCharacterHealth, CurrentHealth);
        }
        OnAttack.AddListener(CharacterAttack);
        OnWasAttacked.AddListener(CharacterWasAttacked);

        characterState = CharacterState.Idle;
    }

    public void Attack(abilityData ability)
    {
        if(characterState == CharacterState.Died)
            return;

        switch (ability.outputType)
        {
            case AblilityOutputType.Heal:
                _target.Heal(ability.abValue);
                break;
            case AblilityOutputType.Damage:
                _target.WasDamaged(ability.abValue);
                break;
        }
        
        OnAttack?.Invoke();

        _target.WasDamaged(ability.abValue);
        characterState = CharacterState.Attacking;
    }

    public void Heal(int healAmount)
    {
       CurrentHealth = Math.Clamp(CurrentHealth + healAmount, 0, maxCharacterHealth);
         if (characterType == CharacterType.Player)
         {
              CharUI.UpdateHealth(CurrentHealth);
        }
    }

    public void WasDamaged(int damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            characterState = CharacterState.Died;
        }

        if (characterType == CharacterType.Player)
        {
            CharUI.UpdateHealth(CurrentHealth);
        }

        OnWasAttacked?.Invoke();

    }

    private void CharacterAttack()
    {
        PlayerJustAttacked = false;
        CurrentSpeed = 0;
    }

    private void CharacterWasAttacked()
    {
        UnityEngine.Debug.Log(CharacterName + " was attacked!");
    }

    public IEnumerator CharacterLoop()
    {
        while (characterState != CharacterState.Died)
        {
            if (CurrentSpeed >= CharacterSpeed)
            {
                CurrentSpeed = CharacterSpeed;
            }
            else
            {
                CurrentSpeed += Time.deltaTime;
                characterState = CharacterState.Idle;
            }
            yield return null;
        }
    }
}

[Serializable]
public class characterUIData
{
    public Slider healthSlider;
    public TMP_Text healthText;
    public TMP_Text characterNameText;

    public void Init(int MaxHealth, int CurrentHealth, string CharName)
    {
        healthSlider.maxValue = MaxHealth;
        healthSlider.value = CurrentHealth;
        healthText.text = CurrentHealth + " /" + MaxHealth;

        characterNameText.text = CharName;
    }

    public void InitEnemy(int MaxHealth, int CurrentHealth)
    {
        healthSlider.maxValue = MaxHealth;
        healthSlider.value = CurrentHealth;
    }

    public void UpdateHealth(int CurrentHealth)
    {
        healthSlider.value = CurrentHealth;
        healthText.text = CurrentHealth + " /" + healthSlider.maxValue;
    }
}

public enum CharacterType
{
    Player,
    Enemy,
}

public enum CharacterState
{
    Loading,
    Idle,
    Ready,
    Attacked,
    Attacking,
    Died
}

