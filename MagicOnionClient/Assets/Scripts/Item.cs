/// ==============================
/// アイテムスクリプト
/// Author: Nishiura Kouta
/// ==============================
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテムの処理を管理するクラス
/// </summary>
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

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
        // ルームモデルスクリプトをフィールド内オブジェクトから取得
        roomModel = GameObject.Find("RoomHubModel").GetComponent<RoomHubModel>();
        // 通知を設定
        roomModel.OnSpawnItemUser += this.OnSpawnItemUser;
    }

    /// <summary>
    /// このオブジェクトが破壊された際の処理
    /// </summary>
    void OnDestroy()
    {
        // 登録した各通知を解除
        roomModel.OnSpawnItemUser -= this.OnSpawnItemUser;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        // 生成回数が9以上になった場合生成を停止
        if(itemCount >=18) CancelInvoke("SpawnItem");
    }

    /// <summary>
    /// アイテム生成処理
    /// </summary>
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

        // 選ばれたスポーンポイントの持つアイテムに、生成したものを入れる
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
        
        // 生成上限を加算
        itemCount++; 
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
    /// アイテム生成同期処理
    /// </summary>
    /// <param name="spawnPoint">生成位置</param>
    /// <param name="itemNumber">アイテム値</param>
    public async void SpawnItem(int spawnPoint, int itemNumber)
    {
        await roomModel.SpawnItemAsync(spawnPoint, itemNumber);
    }

    /// <summary>
    /// アイテム生成通知
    /// </summary>
    /// <param name="spawnPoint">生成位置</param>
    /// <param name="itemNumber">生成アイテム識別番号</param>
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

        // 選ばれたスポーンポイントの持つアイテムに、生成したものを入れる
        nowItems[spawnPoint] = itemObj;

        // 名前を訂正
        itemObj.name = itemPrefabs[itemNumber].name + "_" + itemCount;

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
