using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // 生成するアイテムのプレハブリスト
    [SerializeField] List<GameObject> itemPrefabs;

    // 操作するプレイヤーのカメラ
    public Camera playerCam;

    // アイテム生成上限用カウント
    int itemCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        // 3秒後、1秒に1回アイテムを生成
        InvokeRepeating("SpawnItem", 3f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        // 生成回数が5以上になった場合生成を停止
        if(itemCount >= 5) CancelInvoke("SpawnItem");
    }

    void SpawnItem()
    {
        System.Random rand = new System.Random();

        // 生成するアイテムの番号のランダム設定
        int itemNum = rand.Next(0, itemPrefabs.Count);

        // 生成位置のランダム設定
        float x = rand.Next(-10, 10);
        float z = rand.Next(-10, 10);

        // インスタンス生成
        GameObject itemObj = Instantiate(itemPrefabs[itemNum]);

        // 名前を訂正
        itemObj.name = itemPrefabs[itemNum].name;

        // 生成位置を設定
        itemObj.transform.position = new Vector3(x, 10f, z);

        // 各アイテムにプレイヤーのカメラを渡す
        itemObj.GetComponent<LookAtCamera>().GetCamera(playerCam);

        // 生成上限を加算
        itemCount++;
    }
}
