using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] List<GameObject> itemPrefabs;

    int itemCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnItem", 3f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if(itemCount >= 5) CancelInvoke("SpawnItem");
    }

    void SpawnItem()
    {
        System.Random rand = new System.Random();

        int itemNum = rand.Next(0, 2);
        float x = rand.Next(-10, 10);
        float z = rand.Next(-10, 10);

        // インスタンス生成
        GameObject itemObj = Instantiate(itemPrefabs[itemNum]);

        itemObj.name = itemPrefabs[itemNum].name;

        // 生成位置を設定
        itemObj.transform.position = new Vector3(x, 10f, z);
        itemCount++;
    }
}
