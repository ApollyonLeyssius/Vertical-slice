using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class abilityData
{
    public string abilityName = "New Ability";
    public int abValue = 10;
    public AbilityType type = AbilityType.Melee;
    public AblilityOutputType outputType = AblilityOutputType.Damage;
    public int abilityCost = 10;
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
