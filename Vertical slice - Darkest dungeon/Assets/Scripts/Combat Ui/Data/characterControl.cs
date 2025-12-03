using System.Collections;
using UnityEngine;


public class characterControl : MonoBehaviour
{
    public characterData CharacterData;
    public characterControl targetData;

    private void Awake()
    {
        CharacterData._charCont = this;
    }

    private void Start()
    {
        CharacterData.Init();
        CharacterData._target = targetData.CharacterData;
        StartCoroutine(CharacterData.CharacterLoop());
    }

   /* private void Update()
    {
        if (CharacterData.isReadyForAction)
        {
            if (CharacterData.characterType == CharacterType.Player && Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Player Attacked!");
                if(CharacterData._target.canBeAttacked)
                {
                    CharacterData.Attack();
                }
            }
        }
    }*/
}

