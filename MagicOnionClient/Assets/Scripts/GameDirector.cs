using DG.Tweening;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    // 生成するユーザプレハブ
    [SerializeField] GameObject characterPrefabs;
    // 入力されたユーザID
    [SerializeField] Text userID;
    // 部屋モデル
    [SerializeField] RoomHubModel roomModel;
    // 生成ユーザのディクショナリー
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();
    // カウントダウン表示パネル
    [SerializeField] GameObject countPanel;
    // カウントダウンテキスト
    [SerializeField] Text countText;
    // 準備完了ボタン
    [SerializeField] GameObject readyButton;
    // ゲーム開始判定変数
    bool isStart = false;

    // Start is called before the first frame update
    async void Start()
    {
        // 非表示にする
        countPanel.SetActive(false);
        readyButton.SetActive(false);

        // ユーザが入室したときにメソッドを実行するようモデルに登録
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMovedUser += this.OnMovedUser;
        roomModel.OnReadyUser += this.OnReadyUser;
        // 接続
        await roomModel.ConnectAsync();
    }

    void Update()
    {
        MovePlayer();
    }

    /// <summary>
    /// ユーザ入室処理
    /// </summary>
    /// <param name="user"></param>
    void OnJoinedUser(JoinedUser user)
    {
        GameObject characterGameObject = Instantiate(characterPrefabs); //インスタンス生成
        // 生成位置を設定
        characterGameObject.transform.position = new Vector3(-6 + user.UserData.Id, 0, 0);
        characterList[user.ConnectionID] = characterGameObject; // フィールドで保持
        readyButton.SetActive(true);
    }

    /// <summary>
    /// ユーザ退室処理
    /// </summary>
    /// <param name="connectionID"></param>
    void OnLeavedUser(Guid connectionID)
    {
        // 受け取った接続IDと自信の接続IDが一致している場合
        if (connectionID == roomModel.ConnectionID)
        {  
            // 準備完了ボタンを非表示
            readyButton.SetActive(false);
            // 表示されているすべてのオブジェクトを破壊
            foreach (var character in characterList.Values)
            {
                Destroy(character);
            }
        }
        // 退室ユーザのオブジェクトのみ破壊
        else Destroy(characterList[connectionID]);

    }

    /// <summary>
    /// ユーザ移動処理
    /// </summary>
    /// <param name="pos">位置</param>
    void OnMovedUser(Guid connectionID, Vector3 pos, Vector3 rot)
    {
        // 各トランスフォームをアニメーション
        characterList[connectionID].transform.DOLocalMove(pos,0.1f).SetEase(Ease.Linear);
        characterList[connectionID].transform.DOLocalRotate(rot,0.1f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// ユーザ準備処理
    /// </summary>
    void OnReadyUser()
    {
        StartGame();
    }

    /// <summary>
    /// ゲーム開始処理
    /// </summary>
    void StartGame()
    {
        readyButton.SetActive(false);
        countPanel.SetActive(true);
        float countdownSeconds = 4;

        while (countdownSeconds > 0)
        {
            countdownSeconds -= Time.deltaTime;
            var span = new TimeSpan(0, 0, (int)countdownSeconds);
            countText.text = span.ToString(@"mm\:ss");
        }
        isStart = true;
    }

    /// <summary>
    /// 入室ボタン処理
    /// </summary>
    public async void JoinRoom()
    {
        int.TryParse(userID.text, out int id);
        // 入室
        await roomModel.JoinAsync("sampleRoom", id);
        InvokeRepeating("Move", 0.1f, 0.1f);
    }

    /// <summary>
    /// 退室ボタン処理
    /// </summary>
    public async void LeaveRoom()
    { 
        CancelInvoke("Move");
        int.TryParse(userID.text, out int id);
        // 退室
        await roomModel.LeaveAsync("sampleRoom", id);
    }

    /// <summary>
    /// 移動キー処理
    /// </summary>
    public async void Move()
    {
        // 移動
        await roomModel.MoveAsync(characterList[roomModel.ConnectionID].transform.position,
            characterList[roomModel.ConnectionID].transform.eulerAngles);
    }

    /// <summary>
    /// 準備完了ボタン処理
    /// </summary>
    public async void Ready()
    {
        await roomModel.ReadyAsync();
    }

    /// <summary>
    /// キー入力移動処理
    /// </summary>
    void MovePlayer()
    {
        if (isStart != true) return;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            characterList[roomModel.ConnectionID].transform.position += new Vector3(0.1f, 0f, 0f);
            characterList[roomModel.ConnectionID].transform.eulerAngles += new Vector3(0f, 5f, 0f);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            characterList[roomModel.ConnectionID].transform.position -= new Vector3(0.1f, 0f, 0f);
            characterList[roomModel.ConnectionID].transform.eulerAngles -= new Vector3(0f, 5f, 0f);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            characterList[roomModel.ConnectionID].transform.position += new Vector3(0f, 0f, 0.1f);
            characterList[roomModel.ConnectionID].transform.eulerAngles += new Vector3(0f, 5f, 0f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            characterList[roomModel.ConnectionID].transform.position -= new Vector3(0f, 0f, 0.1f);
            characterList[roomModel.ConnectionID].transform.eulerAngles -= new Vector3(0f, 5f, 0f);
        }
    }
}
