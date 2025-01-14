using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] GameObject heartPrefabs;


    void Awake()
    {
        SetHP();        
    }

    public void SetHP()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject itemObj = Instantiate(heartPrefabs);
            itemObj.name = "Heart_" + (i +1);
            itemObj.transform.position = new Vector3(5f + (1.5f * i), -16.5f, 10f);
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject itemObj = Instantiate(heartPrefabs);
            itemObj.name = "Rival_Heart_" + (i + 1);
            itemObj.transform.position = new Vector3(-8f + (1.5f * i), -16.5f, 10f);
        }
    }
}
