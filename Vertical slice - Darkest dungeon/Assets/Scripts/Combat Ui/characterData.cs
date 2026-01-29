using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class characterData
{
    public string CharacterName;

    public int maxCharacterHealth;
    public int CurrentHealth;
    public int position; // 0 = front, 3 = back
    public int MaxStress;
    public int CurrentStress;

    public float CharacterSpeed;
    public float CurrentSpeed;
    public Image Facial;

    public CharacterState characterState;
    public CharacterType characterType;

    [Space(10)]
    public characterData _target;
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
    public bool isAttackable
    {
        get
        {
            return characterState == CharacterState.Idle || characterState == CharacterState.Ready;
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

    public bool IsAlive
    {
        get
        {
            return characterState != CharacterState.Died;
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
        if (characterState == CharacterState.Died || _target == null)
            return;

        characterState = CharacterState.Attacking;

        switch (ability.outputType)
        {
            case AblilityOutputType.Damage:
                int damage = UnityEngine.Random.Range(ability.minDamage, ability.maxDamage + 1);
                _target.WasDamaged(damage);
                Debug.Log($"{CharacterName} dealt {damage} damage!");
                break;

            case AblilityOutputType.Heal:
                _target.Heal(ability.healAmount);
                break;
        }

        OnAttack?.Invoke();
    }



    public void Heal(int healAmount)
    {
        CurrentHealth = Math.Clamp(CurrentHealth + healAmount, 0, maxCharacterHealth);
        if (characterType == CharacterType.Player)
        {
            CharUI.UpdateHealth(CurrentHealth);
        }
    }

    public void WasDamaged(int amount)
    {
        CurrentHealth -= amount;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            characterState = CharacterState.Died;
        }
        if (_charCont != null)
        {
            _charCont.ShowDamagePopup(amount);
        }

        if (characterType == CharacterType.Player)
        {
            CharUI.UpdateHealth(CurrentHealth);
        }
        if (characterType == CharacterType.Enemy)
        {
            CharUI.UpdateHealthEnemy(CurrentHealth);
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
}

[Serializable]
public class characterUIData
{
    public Slider healthSlider;
    public TMP_Text healthText;
    public TMP_Text characterNameText;

    public void Init(int MaxHealth, int CurrentHealth, string CharName)
    {
        if (healthSlider == null)
        {
            Debug.LogError("HealthSlider is NULL!");
            return;
        }

        healthSlider.maxValue = MaxHealth;
        healthSlider.value = CurrentHealth;

        if (healthText != null)
            healthText.text = CurrentHealth + " / " + MaxHealth;

        if (characterNameText != null)
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

    public void UpdateHealthEnemy(int CurrentHealth)
    {
        healthSlider.value = CurrentHealth;
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
    Died,
    TryingToAttack
}

