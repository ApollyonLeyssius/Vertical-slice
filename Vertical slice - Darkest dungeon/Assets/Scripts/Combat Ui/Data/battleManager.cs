using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class battleManager : MonoBehaviour
{
    public characterControl currentCharacter;
    public battleManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void DoBasicAttackOnTarget()
    {
        if (currentCharacter.CharacterData.isReadyForAction)
        {
            if (currentCharacter.CharacterData.characterType == CharacterType.Player && Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Player Attacked!");
                if (currentCharacter.CharacterData._target.canBeAttacked)
                {
                    currentCharacter.CharacterData.Attack(currentCharacter.CharacterData.basicAttack);
                }
            }
        }


    }
}
