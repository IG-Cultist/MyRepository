/// ==============================
/// ゲームディレクタースクリプト
/// Name:西浦晃太 Update:11/26
/// ==============================
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    // 生成するユーザプレハブ
    [SerializeField] GameObject characterPrefabs;

    // 接続パネル
    [SerializeField] GameObject ConnectPanel;

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
    Player playerScript;
    // ゲーム開始判定変数
    bool isStart = false;

    bool isFinish = false;

    bool isJoin = false;

    float moveSpeed = 1f;

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
        roomModel.OnFinishUser += this.OnFinishUser;
        roomModel.OnAttackUser += this.OnAttackUser;
        // 接続
        await roomModel.ConnectAsync();
    }

    void Update()
    {
        MovePlayer();

        if (Input.GetKeyDown(KeyCode.X) && isStart == true)
        {
            Finish();
        }

        // ゲーム終了状態かつ画面タップをした場合
        if (isFinish == true && Input.GetMouseButtonDown(0))
        {
            LeaveRoom();
            SceneManager.LoadScene("Result");
        }
    }

    /// <summary>
    /// ユーザ入室処理
    /// </summary>
    /// <param name="user"></param>
    void OnJoinedUser(JoinedUser user)
    {  
        // インスタンス生成
        GameObject characterGameObject = Instantiate(characterPrefabs); 

        // プレイヤースクリプト取得
        playerScript =  characterGameObject.GetComponent<Player>();

        // 生成位置を設定
        characterGameObject.transform.position = new Vector3(-7.5f + (user.UserData.Id *2), 2f, -10f);

        // 自分以外のカメラを非アクティブにする
        if(roomModel.ConnectionID != user.ConnectionID){
            characterGameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        // 識別番号を各子オブジェクトの名前に付ける
        foreach (Transform obj in characterGameObject.transform)
        {
            obj.name += "_" + user.UserData.Id;
        }
        characterGameObject.name = characterPrefabs.name + "_" + user.UserData.Id;

        characterList[user.ConnectionID] = characterGameObject; // フィールドで保持
        playerScript.connectionID = user.ConnectionID;
        readyButton.SetActive(true);
    }

    /// <summary>
    /// ユーザ退室処理
    /// </summary>
    /// <param name="connectionID"></param>
    void OnLeavedUser(Guid connectionID)
    {
        // 受け取った接続IDと自身の接続IDが一致している場合
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
    void OnMovedUser(Guid connectionID, Vector3 pos, Vector3 rot, IRoomHubReceiver.PlayerState state)
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
    /// ユーザゲーム終了処理
    /// </summary>
    void OnFinishUser()
    {
        FinishGame();
    }

    /// <summary>
    /// ゲーム開始処理
    /// </summary>
    void StartGame()
    {
        readyButton.SetActive(false);
        countPanel.SetActive(true);
        float limitTime = 4;

        while (limitTime > 0)
        {
            limitTime -= Time.deltaTime;

            if (limitTime < 0)
            {
                limitTime = 0;
            }

            countText.text = limitTime.ToString("F0"); // 残り時間を整数で表示

        }
        isStart = true;
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    void FinishGame()
    {
        countText.text = "クリックで退室";
        isFinish = true;
    }

    /// <summary>
    /// 入室ボタン処理
    /// </summary>
    public async void JoinRoom()
    {
        //ConnectPanel.SetActive(false);
        int.TryParse(userID.text, out int id);
        // 入室
        await roomModel.JoinAsync("sampleRoom", id);
        InvokeRepeating("Move", 0.1f, 0.1f);
        await Task.Delay(60);
        isJoin= true;
    }

    /// <summary>
    /// 退室ボタン処理
    /// </summary>
    public async void LeaveRoom()
    {
        ConnectPanel.SetActive(true);
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
            characterList[roomModel.ConnectionID].transform.eulerAngles,
            IRoomHubReceiver.PlayerState.Move);
    }

    /// <summary>
    /// 準備完了ボタン処理
    /// </summary>
    public async void Ready()
    {
        await roomModel.ReadyAsync();
    }

    public async void Finish()
    {
        await roomModel.FinishAsync();
    }

    public async void Attack(Guid connectionID)
    {
        await roomModel.AttackAsync(connectionID);
    }

    /// <summary>
    /// ユーザ攻撃通知
    /// </summary>
    void OnAttackUser(Guid connectionID, int health)
    {
        Debug.Log(characterList[roomModel.ConnectionID] + "のHP：" + health);
        if (health <= 0)
        { 
            Finish();
        }
    }

    /// <summary>
    /// キー入力移動処理
    /// </summary>
    void MovePlayer()
    {
        // ゲームが開始していない、かつ終了している場合処理しない
        if (isStart != true || isFinish == true || isJoin != true) return;

        Rigidbody rb = characterList[roomModel.ConnectionID].GetComponent<Rigidbody>();  // rigidbodyを取得

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            rb.AddForce(new Vector3(moveSpeed, 0f, 0f), ForceMode.Impulse);
            //characterList[roomModel.ConnectionID].transform.position += new Vector3(moveSpeed, 0f, 0f);
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            rb.AddForce(new Vector3(-moveSpeed, 0f, 0f), ForceMode.Impulse);
        }
        
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            rb.AddForce(new Vector3(0f, 0f, moveSpeed), ForceMode.Impulse);
        }
        
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            rb.AddForce(new Vector3(0f, 0f, -moveSpeed), ForceMode.Impulse);
        }
    }
}
