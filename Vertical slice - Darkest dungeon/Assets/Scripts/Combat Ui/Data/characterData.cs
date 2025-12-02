using System;
using UnityEngine;

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
}

