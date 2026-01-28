using UnityEngine;

public class combattest : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AttackCameraController.Instance.PlayAttackByIndex(3, 2);

        }
    }
}
