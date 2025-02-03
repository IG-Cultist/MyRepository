/// ==============================
/// アイテムスクリプト
/// Name:西浦晃太 Update:02/03
/// ==============================
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // 生成するアイテムのプレハブリスト
    [SerializeField] List<GameObject> itemPrefabs;

    // 生成ポイントオブジェクトの配列
    [SerializeField] GameObject[] spawnPointObject;

    // 現在存在しているアイテム配列
    GameObject[] nowItems = new GameObject[3];

    // 現在存在しているアイテム配列(同期用)
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
        // 生成上限を加算
        itemCount++; 

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
        itemObj.name = itemPrefabs[itemNum].name + "_" + itemCount;

        // 生成位置のポジションを代入
        Vector3 pos = spawnPointObject[num].transform.position;
        // 生成位置より少し上にアイテムを生成
        itemObj.transform.position = new Vector3(pos.x, pos.y +1.0f, pos.z);

        // 各アイテムにプレイヤーのカメラを渡す
        itemObj.GetComponent<LookAtCamera>().GetCamera(playerCam);

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

        // 生成位置のポジションを代入
        Vector3 pos = spawnPointObject[spawnPoint].transform.position;
        // 生成位置より少し上にアイテムを生成
        itemObj.transform.position = new Vector3(pos.x, pos.y + 1.0f, pos.z);

        // 各アイテムにプレイヤーのカメラを渡す
        itemObj.GetComponent<LookAtCamera>().GetCamera(playerCam);

        // 生成上限を加算
        itemCount++;
    }
}
