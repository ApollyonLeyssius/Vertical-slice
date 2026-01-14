using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class abilityData
{
    public string abilityName;

    [Header("Damage")]
    public int minDamage = 3;
    public int maxDamage = 7;

    [Header("Heal")]
    public int healAmount = 0;

    public AblilityOutputType outputType;

    [Header("Position Rules")]
    public List<int> usableFromPositions;
    public List<int> validTargetPositions;

    public Sprite abilityIcon;

    [Header("Targeting")]
    public bool targetsAll;
    public bool targetsSelf;

    public int critChance = 5;
    public float critMultiplier = 1.5f;
}


public enum AbilityType
{
    Melee,
    Ranged
}

public enum AblilityOutputType
{
    Damage,
    Heal
}
