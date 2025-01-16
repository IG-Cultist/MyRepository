using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // 生成するアイテムのプレハブリスト
    [SerializeField] List<GameObject> itemPrefabs;

    // 現在存在しているアイテム配列
    GameObject[] nowItems = new GameObject[3];

    GameObject[] nowItems_rival = new GameObject[3];

    // 操作するプレイヤーのカメラ
    public Camera playerCam;

    // アイテム生成上限用カウント
    int itemCount = 0;

    RoomHubModel roomModel;

    // Start is called before the first frame update
    void Start()
    {
        roomModel = GameObject.Find("RoomHubModel").GetComponent<RoomHubModel>();
        roomModel.OnSpawnItemUser += this.OnSpawnItemUser;
    }

    // Update is called once per frame
    void Update()
    {
        // 生成回数が9以上になった場合生成を停止
        if(itemCount >=9) CancelInvoke("SpawnItem");
    }

    void SpawnItem()
    {
        System.Random rand = new System.Random();

        // 生成位置番号のランダム設定
        int num = rand.Next(0, 3);

        // 生成するアイテムの番号のランダム設定
        int itemNum = rand.Next(0, itemPrefabs.Count);

        // インスタンス生成
        GameObject itemObj = Instantiate(itemPrefabs[itemNum]);

        // すでにスポーンポイントにアイテムが存在している場合
        if (nowItems[num] != null)
        {
            // そのアイテムを破壊
            Destroy(nowItems[num]);
        }

        nowItems[num] = itemObj;

        // 名前を訂正
        itemObj.name = itemPrefabs[itemNum].name;

        // 生成位置設定
        switch (num)
        {
            case 0:
                itemObj.transform.position = new Vector3(1f, 1.5f, 9f);
                break;
            case 1:
                itemObj.transform.position = new Vector3(8f, 1.5f, -2f);
                break;
            case 2:
                itemObj.transform.position = new Vector3(-7f, 1.5f, -5f);
                break;
            default:
                break;
        }

        // 各アイテムにプレイヤーのカメラを渡す
        itemObj.GetComponent<LookAtCamera>().GetCamera(playerCam);

        // 生成上限を加算
        itemCount++; 

        SpawnItem(num, itemNum);
    }

    /// <summary>
    ///  アイテム生成開始
    /// </summary>
    public void StartSpawn()
    {
        // 3秒後、1秒に1回アイテムを生成
        InvokeRepeating("SpawnItem", 3f, 3f);
    }

    /// <summary>
    /// アイテム生成
    /// </summary>
    /// <param name="spawnPoint">生成位置</param>
    /// <param name="itemNumber">アイテム値</param>
    public async void SpawnItem(int spawnPoint, int itemNumber)
    {
        await roomModel.SpawnItemAsync(spawnPoint, itemNumber);
    }

    void OnSpawnItemUser(int spawnPoint, int itemNumber)
    {
        // インスタンス生成
        GameObject itemObj = Instantiate(itemPrefabs[itemNumber]);

        // すでにスポーンポイントにアイテムが存在している場合
        if (nowItems[spawnPoint] != null)
        {
            // そのアイテムを破壊
            Destroy(nowItems[spawnPoint]);
        }

        nowItems[spawnPoint] = itemObj;

        // 名前を訂正
        itemObj.name = itemPrefabs[itemNumber].name;

        // 生成位置設定
        switch (spawnPoint)
        {
            case 0:
                itemObj.transform.position = new Vector3(1f, 1.5f, 9f);
                break;
            case 1:
                itemObj.transform.position = new Vector3(8f, 1.5f, -2f);
                break;
            case 2:
                itemObj.transform.position = new Vector3(-7f, 1.5f, -5f);
                break;
            default:
                break;
        }

        // 各アイテムにプレイヤーのカメラを渡す
        itemObj.GetComponent<LookAtCamera>().GetCamera(playerCam);

        // 生成上限を加算
        itemCount++;
    }
}
