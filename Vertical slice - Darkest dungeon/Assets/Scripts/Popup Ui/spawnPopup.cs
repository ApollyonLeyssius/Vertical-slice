using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnPopup : MonoBehaviour
{
    public GameObject spawnPrefab;   // test 1
    public GameObject spawnPrefab2;  // test 2
    public GameObject spawnPrefab3;  // test 3
    public GameObject spawnPrefab4;  // test 4

    public Transform spawnParent; // optional parent (UI canvas)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SpawnAndTest(1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SpawnAndTest(2);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SpawnAndTest(3);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SpawnAndTest(4);
    }

    private void SpawnAndTest(int testCase)
    {
        GameObject prefabToSpawn = null;
        int popupDirection = 2; // default direction, can change per case

        switch (testCase)
        {
            case 1:
                prefabToSpawn = spawnPrefab;
                popupDirection = Random.Range(1, 4);
                break;

            case 2:
                prefabToSpawn = spawnPrefab2;
                popupDirection = Random.Range(1, 4);
                break;

            case 3:
                prefabToSpawn = spawnPrefab3;
                popupDirection = Random.Range(1, 4);
                break;

            case 4:
                prefabToSpawn = spawnPrefab4;
                popupDirection = Random.Range(1, 4); // random direction
                break;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab assigned for testCase: " + testCase);
            return;
        }

        // Spawn the popup
        GameObject instance = Instantiate(prefabToSpawn, spawnParent);

        // Call the popup movement script
        BuffAndDebuffmanager popup = instance.GetComponent<BuffAndDebuffmanager>();
        if (popup != null)
        {
            popup.Popup(popupDirection);
        }
        
    }
}
