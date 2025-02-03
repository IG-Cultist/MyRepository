/// ==============================
/// ゲームディレクタースクリプト
/// Name:西浦晃太 Update:02/03
/// ==============================
using DG.Tweening;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TedLab;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    // カウントダウンゲームオブジェクト
    [SerializeField] GameObject[] coundDownObjects;
    // 開始合図ゲームオブジェクト
    [SerializeField] GameObject[] readyTextObjects;
    // ゲーム結果オブジェクト
    [SerializeField] GameObject[] resultObjects;

    // 生成するユーザプレハブ
    [SerializeField] GameObject characterPrefabs;
    // 生成するトラッププレハブ
    [SerializeField] GameObject trapPrefabs;
    // 生成する偽影プレハブ
    [SerializeField] GameObject fakeShadowPrefabs;

    // 退室ボタン
    [SerializeField] GameObject exitButton;
    // 視点変更ボタン
    [SerializeField] GameObject viewButton;
    // カウントダウン表示パネル
    [SerializeField] GameObject countPanel;
    // 警告パネル
    [SerializeField] GameObject warningPanel;

    // 移動速度上昇アイコン
    [SerializeField] GameObject speedUpEffect;
    // 位置表示アイコン
    [SerializeField] GameObject localizationEffect;
    // 影生成アイコン
    [SerializeField] GameObject spawnShadowEffect;

    // カウントダウンテキスト
    [SerializeField] Text countText;
    // 所有アイテムパネル
    [SerializeField] Image itemPanel;
    // 操作用ジョイスティック
    [SerializeField] FixedJoystick joystick;

    // ストップウォッチ使用SE
    [SerializeField] AudioClip stopWatchSE;
    // 踏みつけ時SE
    [SerializeField] AudioClip stompSE;
    // プロジェクター使用SE
    [SerializeField] AudioClip projectorSE;
    // トラップ使用SE
    [SerializeField] AudioClip trapSE;
    // タイムアップSE
    [SerializeField] AudioClip timeUpSE;
    // 時計の針SE
    [SerializeField] AudioClip clockSE;
    // トラップ発動SE
    [SerializeField] AudioClip trapBiteSE;
    // コンパス使用SE
    [SerializeField] AudioClip compassSE;
    // ローラブレード使用SE
    [SerializeField] AudioClip rollerBladeSE;

    [SerializeField] Light Light;

    // 部屋モデル
    [SerializeField] RoomHubModel roomModel;
    
    // 生成ユーザのディクショナリー
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    // オーディオソース
    AudioSource audioSource;
    // 自分のカメラ
    GameObject myCamera;

    // プレイヤーのHPリスト
    List<GameObject> heartList;
    List<GameObject> rivalHeartList;

    // プレイヤースクリプト
    Player playerScript;
    // アイテムスクリプト
    Item itemScript;

    // 移動速度
    public float moveSpeed;
    // 通常の移動速度
    float defaultSpeed = 1.2f;

    // ユーザID
    public int userID;
    // 制限時間
    public int time = 61;
    // プレイヤーのHP
    int playerHP = 3;
    // ライバルのHP
    int rivalHP = 3;
    // タイムアップ時SE再生回数用変数
    int timeUpCnt = 0;
    // 視点変更用変数
    int viewCount = 2;
    // 投影機使用回数カウント
    int useProjector = 0;

    // 現在所有しているアイテム名
    public string nowItemName = "";

    // ゲーム開始判定変数
    bool isStart = false;
    // ゲーム終了判定変数
    bool isFinish = false;
    // 移動速度ブースト判定
    bool isBoost = false;
    // 位置特定判定
    bool isLocate = false;
    // マスタークライアント判定
    bool isMaster;
    // 相手切断判定
    bool isStop = false;
    // クールダウン判定
    bool isCooldown = false;
    // ボタンを押したときtrue、離したときfalseになるフラグ
    bool buttonDownFlag = false;

    // Start is called before the first frame update
    async void Start()
    {
#if UNITY_EDITOR
        //エディター実行時
        if (SendData.roomName == null)
        {
            SendData.roomName = "RoomRoom";
            SendData.userID = 1;
        }
#endif
        // ユーザが入室したときにメソッドを実行するようモデルに登録
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMovedUser += this.OnMovedUser;
        roomModel.OnFinishUser += this.OnFinishUser;
        roomModel.OnAttackUser += this.OnAttackUser;
        roomModel.OnCountUser += this.OnCountUser;
        roomModel.OnChangeSkinUser += this.OnChangeSkinUser;
        roomModel.OnUseItemUser += this.OnUseItemUser;
        roomModel.OnStompItemUser += this.OnStompItemUser;

        // オーディオソースのコンポネント取得
        audioSource = GetComponent<AudioSource>();

        // アイテムスクリプトをフィールド内から取得
        itemScript = GameObject.Find("ItemManager").GetComponent<Item>();

        // 移動速度の初期化
        moveSpeed = defaultSpeed;

        // ライト強度の初期値を設定
        Light.intensity = 2.2f;

        // 各画像を非表示にする
        exitButton.SetActive(false);
        spawnShadowEffect.SetActive(false);
        localizationEffect.SetActive(false);
        speedUpEffect.SetActive(false);
        warningPanel.SetActive(false);

        // カウントダウン用テキスト、勝敗判定用テキストを非表示
        for (int i = 0; i < 3; i++)
        {
            coundDownObjects[i].SetActive(false);
            resultObjects[i].SetActive(false);
        }
        // 開始通知用テキストを非表示
        for (int i = 0; i < readyTextObjects.Length; i++)
        {
            readyTextObjects[i].SetActive(false);
        }

        // ハートのゲームオブジェクトを取得
        heartList = new List<GameObject>();
        rivalHeartList = new List<GameObject>();

        // 各ハートをリストに入れる
        for (int i = 0; i < 3; i++)
        {
            rivalHeartList.Add(GameObject.Find("Rival_Heart_" + (i + 1)));
            heartList.Add(GameObject.Find("Heart_" + (i + 1)));
        }

        // 接続
        await roomModel.ConnectAsync();

        // 1秒待機
        await Task.Delay(600);

        // 部屋に参加
        JoinRoom();
    }

    void Update()
    {
        // 移動処理
        MovePlayer();

        // 見上げボタン押下中処理
        if (buttonDownFlag)
        {
            // 時間切れになったら強制的に視点を戻す
            if (viewCount <= 0) OnButtonUp();
        }

        // ゲーム終了状態かつ画面タップをした場合
        if (isFinish == true && Input.GetMouseButtonDown(0))
        {
            //ルームから退室
            LeaveRoom();
        }

        // タイムアップSEが4回鳴ったらSE再生ループ処理を停止
        if (timeUpCnt >= 4) CancelInvoke("TimeUp");

        // 残り時間が3秒を超えている場合
        if (this.time > 3)
        {
            // カウントダウン用テキストを非表示
            for (int i = 0; i < coundDownObjects.Length; i++)　// 3秒を強調しているときにストップウォッチで時間を延長した際に表示を消すため
            {
                coundDownObjects[i].SetActive(false);
            }
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
        playerScript = characterGameObject.GetComponent<Player>();

        // 生成位置を設定
        characterGameObject.transform.position = new Vector3(-7.5f + (2 * user.JoinOrder), 2f, -10f);

        // ユーザオブジェクト名を参加順にする
        characterGameObject.name = user.JoinOrder.ToString();

        // 自分以外の各オブジェクトを非アクティブにする
        if (roomModel.ConnectionID != user.ConnectionID)
        {
            // 対象ののカメラを取得
            GameObject obj = characterGameObject.transform.GetChild(0).gameObject;

            // カメラを非アクティブ化
            obj.SetActive(false);
            // アタックゾーンを非アクティブ化
            GameObject zone = characterGameObject.transform.GetChild(1).gameObject;
            zone.SetActive(false);

            // 自身の影と区別するためタグを変更
            foreach (Transform item in characterGameObject.transform.GetChild(2))
            {
                item.tag = "Shadow_Rival";
            }
            // マップから非表示に
            characterGameObject.transform.GetChild(3).GetComponent<MeshRenderer>().enabled = false;
            // カメラのタグを変更
            obj.tag = "Camera_Rival";
            // ライバルオブジェクトのタグを変更
            characterGameObject.tag = "Rival";
        }
        else if (roomModel.ConnectionID == user.ConnectionID) // 送信されてきた接続IDと自身の接続IDが一致していた場合
        {
            // 参加順が最初の場合
            if (user.JoinOrder == 1)
            {
                //自身をマスターとする
                isMaster = true;
            }

            // 自身のカメラを取得
            GameObject obj = characterGameObject.transform.GetChild(0).gameObject;
            // カメラを保存
            myCamera = obj;
            // カメラのコンポネントを取得
            Camera camera = obj.GetComponent<Camera>();
            // カメラのコンポネントからデータを取得
            var cameraData = camera.GetUniversalAdditionalCameraData();
            // シーン内にあるヘルスカメラを取得
            GameObject stackObj = GameObject.Find("HealthCamera");
            // ハートカメラのコンポネントを取得
            Camera stackCamera = stackObj.GetComponent<Camera>();
            // 自身のカメラにハートカメラをスタックさせる
            cameraData.cameraStack.Add(stackCamera);

            // アイテムスクリプトへ自身のカメラを渡す
            itemScript.playerCam = camera;

            // 解像度設定を生成したカメラに設定
            RectScalerWithViewport rectScalerWithViewport = GameObject.Find("RectScalerPanel").GetComponent<RectScalerWithViewport>();
            rectScalerWithViewport.refCamera = camera;
        }

        // ユーザの設定したスキン反映処理
        ChangeSkin(user.JoinOrder, user.SkinName);

        characterList[user.ConnectionID] = characterGameObject; // フィールドで保持
        // プレイヤースクリプトに接続IDを渡す
        playerScript.connectionID = user.ConnectionID;
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
            // 表示されているすべてのオブジェクトを破壊
            foreach (var character in characterList.Values)
            {
                Destroy(character);
            }
        }
        else 
        {
            // 退室ユーザのオブジェクトのみ破壊
            Destroy(characterList[connectionID]);
            // ゲームが開始されているかつまだゲームが終了していない場合
            if (isStart == true && isFinish == false)
            {
                // 退室確認処理
                CheckExit();
            }
        }
    }

    /// <summary>
    /// ユーザ移動処理
    /// </summary>
    /// <param name="connectionID">接続ID</param>
    /// <param name="pos">位置</param>
    /// <param name="rot">回転</param>
    /// <param state="rot">プレイヤー状態</param>   
    void OnMovedUser(Guid connectionID, Vector3 pos, Vector3 rot, IRoomHubReceiver.PlayerState state)
    {
        // プレイヤーがいない場合、処理しない
        if (characterList.Count == 0) return;
        // 各トランスフォームをアニメーション
        characterList[connectionID].transform.DOLocalMove(pos, 0.1f).SetEase(Ease.Linear);
        characterList[connectionID].transform.DOLocalRotate(rot, 0.1f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// ユーザゲーム終了処理
    /// </summary>
    void OnFinishUser()
    {
        // ゲーム終了処理
        FinishGame();
    }

    /// <summary>
    /// ゲーム開始処理
    /// </summary>
    void StartGame()
    {
        // アイテム生成処理
        itemScript.StartSpawn();
        // カウントダウンパネルを表示
        countPanel.SetActive(true);
        // カウントダウン処理をループさせる
        InvokeRepeating("CountDown", 0.1f, 1f);
    }

    /// <summary>
    /// ゲーム開始合図処理
    /// </summary>
    async void ReadyGo()
    {
        // Readyを表示
        readyTextObjects[0].SetActive(true);
        await Task.Delay(1200); // 2秒待つ

        // Ready江尾非表示にし、Goを表示
        readyTextObjects[0].SetActive(false);
        readyTextObjects[1].SetActive(true);
        InvokeRepeating("DecreaseIntensity", 0.1f, 0.3f);
        // 自身がマスターである場合
        if (isMaster == true)
        {
            // ゲーム開始処理
            StartGame();
        }
        await Task.Delay(800); // 1.2秒待つ
        // Goを非表示
        readyTextObjects[1].SetActive(false);

    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    void FinishGame()
    {
        exitButton.SetActive(false);
        // カウントダウンを停止
        CancelInvoke("CountDown");
        // ゲーム終了をtrueに
        isFinish = true;
        // ゲーム終了SEをループ
        InvokeRepeating("TimeUp", 0.1f, 2f);
    }

    /// <summary>
    /// 入室処理
    /// </summary>
    public async void JoinRoom()
    {
        // 自身の参加をサーバに送信
        await roomModel.JoinAsync(SendData.roomName, SendData.skinName);

        // 移動同期をループ
        InvokeRepeating("Move", 0.1f, 0.1f);
        // ゲーム開始合図表示処理
        ReadyGo();

  
    }

    /// <summary>
    /// 退室ボタン処理
    /// </summary>
    public async void LeaveRoom()
    {
        // 移動同期を停止
        CancelInvoke("Move");

        // カウントダウンテキストを空に
        countText.text = "";

        // 退室
        await roomModel.LeaveAsync(SendData.roomName, SendData.userID);
        // リザルトシーンへ遷移
        Initiate.DoneFading();
        Initiate.Fade("Result", Color.black, 0.7f);
    }

    /// <summary>
    /// タイトル遷移処理
    /// </summary>
    public async void BackTitle()
    {
        // 退室
        await roomModel.LeaveAsync(SendData.roomName, SendData.userID);
        // タイトルシーンへ遷移
        Initiate.DoneFading();
        Initiate.Fade("Title", Color.black, 0.7f);
    }

    /// <summary>
    /// 移動キー処理
    /// </summary>
    public async void Move()
    {
        // 自身のキャラが生成されていない場合、処理しない
        if (characterList[roomModel.ConnectionID] == null) return;
        // 自身の移動をサーバに送信
        await roomModel.MoveAsync(characterList[roomModel.ConnectionID].transform.position,
            characterList[roomModel.ConnectionID].transform.eulerAngles,
            IRoomHubReceiver.PlayerState.Move);
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    public async void Finish()
    {
        // ゲーム終了をサーバに送信
        await roomModel.FinishAsync();
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    /// <param name="connectionID"></param>
    public async void Attack(Guid connectionID)
    {
        // 攻撃の処理結果をサーバに送信
        await roomModel.AttackAsync(connectionID);
    }

    /// <summary>
    /// ユーザ攻撃通知
    /// </summary>
    void OnAttackUser(Guid connectionID, int health)
    {
        // 受け取った接続IDと自身の接続IDが一致している場合
        if (connectionID == roomModel.ConnectionID)
        {
            // カメラを揺らす
            characterList[roomModel.ConnectionID].transform.GetChild(0).DOShakePosition(0.6f, 1.5f, 45, 15, false, true);

            // HPオブジェクトを破壊
            Destroy(heartList[health]);
            // 現在のHPを保存
            playerHP = health;

            // 結果を表示
            resultObjects[1].SetActive(true);
        }
        else
        {
            // ライバルのHPオブジェクトを破壊
            Destroy(rivalHeartList[health]);
            // ライバルのHPを保存
            rivalHP = health;
            // 結果を表示
            resultObjects[0].SetActive(true);

        }

        // 被弾対象のHPが0以下の場合
        if (health <= 0)
        {
            for (int i = 0; i < coundDownObjects.Length; i++)
            {
                coundDownObjects[i].SetActive(false);
            }

            // 影と踏みつけエリアを非表示
            characterList[connectionID].transform.GetChild(1).gameObject.SetActive(false);
            characterList[connectionID].transform.GetChild(2).gameObject.SetActive(false);

            Finish();
        }
    }

    /// <summary>
    /// キー入力移動処理
    /// </summary>
    void MovePlayer()
    {
        // 開始前とゲーム終了後は移動させない
        if (isStop == true || isStart != true || isFinish == true) return;

        Rigidbody rb = characterList[roomModel.ConnectionID].GetComponent<Rigidbody>();  // rigidbodyを取得

        //ジョイスティックの位置を代入
        float axisZ = joystick.Vertical;
        float axisX = joystick.Horizontal;

        rb.AddRelativeForce(new Vector3((90 * moveSpeed) * axisX, 0, (90 * moveSpeed) * axisZ));

        // RightArrowキーまたはDキーを押した場合
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            // 自身のオブジェクトの東方面に力を加える
            rb.AddForce(new Vector3(moveSpeed, 0f, 0f), ForceMode.Impulse);
        }

        // LeftArrowキーまたはAキーを押した場合
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            // 自身のオブジェクトの西方面に力を加える
            rb.AddForce(new Vector3(-moveSpeed, 0f, 0f), ForceMode.Impulse);
        }

        // UpArrowキーまたはWキーを押した場合
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            // 自身のオブジェクトの南方面に力を加える
            rb.AddForce(new Vector3(0f, 0f, moveSpeed), ForceMode.Impulse);
        }

        // DownArrowキーまたはSキーを押した場合
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            // 自身のオブジェクトの北方面に力を加える
            rb.AddForce(new Vector3(0f, 0f, -moveSpeed), ForceMode.Impulse);
        }
    }

    /// <summary>
    /// アイテムを踏んだ際の処理
    /// </summary>
    public async void StompItem(string name)
    {
        // 受け取った文字列を_で分割
        string[] words = name.Split("_");

        // 踏んだのが偽影でない場合
        if (words[0] != "Fake")
        {
            // リソースから、アイテムテクスチャを取得
            Texture2D texture = Resources.Load("Items/" + words[0]) as Texture2D;

            // 取得したスプライトを反映
            itemPanel.sprite =
                Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            // 現在所持しているアイテム名を保存
            nowItemName = name;
        }
        // 踏んだことをサーバに送信
        await roomModel.StompItemAsync(name);
    }

    /// <summary>
    /// 踏んだアイテム破壊通知
    /// </summary>
    /// <param name="itemName"></param>
    async void OnStompItemUser(string itemName)
    {
        GameObject obj = GameObject.Find(itemName);
        if (obj != null) Destroy(obj);
    }

    /// <summary>
    /// アイテム別効果処理
    /// </summary>
    public async void UseItem()
    {
        // 開始前とゲーム終了後はアイテムを使用させない
        if (isStop == true || isStart != true || isFinish == true) return;

        // 受け取った文字列を_で分割
        string[] words = nowItemName.Split("_");

        GameObject rivalObj = new GameObject();
        Destroy(rivalObj);

        // 自分でないプレイヤーのゲームオブジェクトを取得
        foreach (var id in characterList.Keys)
        {
            if (id != roomModel.ConnectionID)
            {
                rivalObj = characterList[id];
                break;
            }
        }
        // 自身の位置を取得
        Vector3 playerPos = characterList[roomModel.ConnectionID].transform.position;

        // アイテム別処理
        switch (words[0])
        {
            case "Compass": // コンパスの場合
                // 相手の位置をマップに3秒間表示する
                audioSource.PlayOneShot(compassSE);

                //ライバルの位置を表示
                rivalObj.transform.GetChild(3).GetComponent<MeshRenderer>().enabled = true;
                localizationEffect.SetActive(true);
                isLocate = true;
                break;

            case "RollerBlade": // ローラーブレードの場合
                audioSource.PlayOneShot(rollerBladeSE);
                speedUpEffect.SetActive(true);
                // 移動速度を3秒間2倍にする
                moveSpeed = 2.0f;
                isBoost = true;
                break;

            case "StopWatch": // ストップウォッチの場合
                // ゲーム時間を3秒延長する
                audioSource.PlayOneShot(stopWatchSE);
                time += 3;
                await roomModel.CountTimer(time);
                break;

            case "Trap": // トラップの場合
                audioSource.PlayOneShot(trapSE);

                // インスタンス生成
                GameObject trapObj = Instantiate(trapPrefabs);
                trapObj.name = "Trap(active)";
                // 生成位置を設定
                trapObj.transform.position = new Vector3(playerPos.x, playerPos.y + 1.0f, playerPos.z + 3.0f);
                // アイテム使用をサーバに送信
                await roomModel.UseItemAsync(roomModel.ConnectionID, nowItemName);
                break;

            case "Projector": // 投影機の場合

                useProjector++;
                // 5秒間自由に動く偽の影を召喚する
                audioSource.PlayOneShot(projectorSE);

                // インスタンス生成
                GameObject fakeObj = Instantiate(fakeShadowPrefabs);
                // オブジェクト名を変更
                fakeObj.name = "Fake_Shadow" + "_" + useProjector;

                // 生成位置を設定
                fakeObj.transform.position = new Vector3(playerPos.x, 1.7f, playerPos.z + 3.0f);
                // 影生成判定をtrueに
                spawnShadowEffect.SetActive(true);
                // 影破壊処理
                KillFake(fakeObj);
                // アイテム使用をサーバに送信
                await roomModel.UseItemAsync(roomModel.ConnectionID, nowItemName);
                break;

            default:
                break;
        }

        // 現在所有しているアイテムの名前を初期化
        nowItemName = "";

        // リソースから、アイテムテクスチャを取得
        Texture2D texture = Resources.Load("Items/None") as Texture2D;

        // アイテムテクスチャを未所持のテクスチャに変更
        itemPanel.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                               Vector2.zero);

        // ローラーブレードを使用していた場合
        if (isBoost == true)
        {
            // 3秒後に速度を戻す
            await Task.Delay(1800);
            speedUpEffect.SetActive(false);
            moveSpeed = defaultSpeed;
            isBoost = false;
        }

        // コンパスを使用していた場合
        if (isLocate == true)
        {
            // 3秒後に位置特定を解除
            await Task.Delay(1800);
            rivalObj.transform.GetChild(3).GetComponent<MeshRenderer>().enabled = false;
            localizationEffect.SetActive(false);
            isLocate = false;
        }
    }

    public async void OnUseItemUser(Guid connectionID, string itemName)
    {
        Vector3 playerPos = characterList[connectionID].transform.position;

        switch (itemName)
        {
            case "Trap": // トラップの場合
                audioSource.PlayOneShot(trapSE);
                // インスタンス生成
                GameObject trapObj = Instantiate(trapPrefabs);
                trapObj.name = "Trap(active)";
                // 生成位置を設定
                trapObj.transform.position = new Vector3(playerPos.x, playerPos.y + 1.0f, playerPos.z + 3.0f);

                break;

            case "Projector": // 投影機の場合
                useProjector++;
                // 5秒間自由に動く偽の影を召喚する
                audioSource.PlayOneShot(projectorSE);

                // インスタンス生成
                GameObject fakeObj = Instantiate(fakeShadowPrefabs);

                fakeObj.name = "Fake_Shadow" + "_" + useProjector;

                // 生成位置を設定
                fakeObj.transform.position = new Vector3(playerPos.x, 1.7f, playerPos.z + 3.0f);
                break;

            case "Trap(active)": //トラップ(アクティブ)の場合
                // 破壊
                audioSource.PlayOneShot(trapBiteSE);
                GameObject obj = GameObject.Find(itemName);
                if (obj != null) Destroy(obj);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// カウントダウン処理
    /// </summary>
    async void CountDown()
    {
        // 制限時間を-1
        time -= 1;
        // 残り時間を全ユーザに通知
        await roomModel.CountTimer(time);
    }

    /// <summary>
    /// カウントダウン通知
    /// </summary>
    /// <param name="time"></param>
    void OnCountUser(int time)
    {
        isStart = true;

        // 制限時間を送られてきた時間と同期する
        this.time = time;

        switch (this.time)
        {
            case 3: // 3を強調 カウントダウン用オブジェクトを空にする
                countText.text = "";
                coundDownObjects[0].SetActive(true);
                break;

            case 2: // 2を強調
                coundDownObjects[0].SetActive(false);
                coundDownObjects[1].SetActive(true);
                break;

            case 1: // 1を強調
                coundDownObjects[1].SetActive(false);
                coundDownObjects[2].SetActive(true);
                break;

            case <= 0: // タイムアップ
                coundDownObjects[2].SetActive(false);
                // UIにタイムアップと表記する
                countText.text = "Time UP";
                // カウントダウン処理を停止
                CancelInvoke("CountDown");
                // ライト強度低下処理を停止
                CancelInvoke("DecreaseIntensity");
                if (playerHP < rivalHP) resultObjects[1].SetActive(true);
                else if (playerHP > rivalHP) resultObjects[0].SetActive(true);
                else resultObjects[2].SetActive(true);
                // ゲームエンド処理を呼ぶ
                Finish();
                break;

            default: // それ以外はカウントを画面に反映
                // 減算結果をUIに適応
                countText.text = this.time.ToString();
                break;
        }
    }

    void OnChangeSkinUser(int userID, string skinName)
    {
        if (GameObject.Find(userID.ToString()).transform.GetChild(2) != null)
        {
            // 取得したこの数分ループ
            foreach (Transform ob in GameObject.Find(userID.ToString()).transform.GetChild(2))
            {
                // ロビーで設定したスキン名と一致しない場合
                if (skinName != ob.name)
                {
                    // コライダーを消す
                    ob.GetComponent<BoxCollider>().enabled = false;
                    // 透明度を0にする
                    ob.GetComponent<Renderer>().material.color = new Color(255, 255, 255, 0);
                }
            }
        }
    }

    async void ChangeSkin(int userID, string skinName)
    {
        await roomModel.ChangeSkinAsync(userID, skinName);
    }

    void TimeUp()
    {
        audioSource.PlayOneShot(timeUpSE);
        timeUpCnt++;
    }

    /// <summary>
    /// 視点変更ボタン押下時処理
    /// </summary>
    public void OnButtonDown()
    {  
        // 開始前とゲーム終了後はアイテムを使用させない
        if (isStop == true || isStart != true || isFinish == true || isCooldown) return;

        // 視点変更時間を初期化
        viewCount = 3;
        // 押下フラグをtrueに
        buttonDownFlag = true;

        // カメラを見上げる視点に回転
        myCamera.transform.DOLocalRotate(new Vector3(5f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
        // 移動速度を少し下げる
        moveSpeed = 0.2f;
        // 視点変更時間をカウントダウン
        InvokeRepeating("ViewTime", 0.1f, 1f);
    }
    /// <summary>
    /// 視点変更解除処理
    /// </summary>
    public void OnButtonUp()
    {
        // まだ開始していない、またはゲームが終了している場合処理しない
        if (isStart != true　|| isFinish == true) return;

        if (isCooldown) return; // クールダウン中の場合、処理しない

        // クールダウン中にする
        isCooldown = true;
        // カウントダウンを停止
        CancelInvoke("ViewTime");
        // 押下フラグをfalseに
        buttonDownFlag = false;

        // カメラの回転を元に戻す
        myCamera.transform.DOLocalRotate(new Vector3(27f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
        //移動速度を元に戻す
        moveSpeed = defaultSpeed;

        // リソースから、アイコンを取得
        Texture2D texture = Resources.Load("UI/eye_close") as Texture2D;

        Image buttonTexture = viewButton.GetComponent<Image>();

        buttonTexture.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                       Vector2.zero);

        // カウントダウン処理のループを開始
        InvokeRepeating("ViewCooldown", 3f, 1f);
    }

    /// <summary>
    /// 見上げ視点制限時間カウント処理
    /// </summary>
    void ViewTime()
    {
        viewCount--;
    }

    /// <summary>
    /// 見上げ視点クールダウン処理
    /// </summary>
    void ViewCooldown()
    {
        // クールダウンのループ処理を停止
        CancelInvoke("ViewCooldown");

        // リソースから、アイコンを取得
        Texture2D texture = Resources.Load("UI/eye_open") as Texture2D;

        Image buttonTexture = viewButton.GetComponent<Image>();

        buttonTexture.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                       Vector2.zero);
        // クールダウンしていない状態に
        isCooldown = false;
    }

    /// <summary>
    /// 偽影削除処理
    /// </summary>
    /// <param name="fakeObj"></param>
    async void KillFake(GameObject fakeObj)
    {
        // 10秒後に影を削除
        await Task.Delay(6000);

        // 削除通知
        Destroy(fakeObj);
        StompItem(fakeObj.name);

        // 影生成確認用アイコンを非表示
        spawnShadowEffect.SetActive(false);
    }

    /// <summary>
    /// 途中退室処理
    /// </summary>
    void CheckExit()
    {
        isStop = true;
        // 各繰り返しを停止
        CancelInvoke("ViewCooldown");
        CancelInvoke("Move");
        // 警告パネルを表示
        warningPanel.SetActive(true);
    }

    /// <summary>
    /// ライト強度減少処理
    /// </summary>
    void DecreaseIntensity()
    {  
        // ライトの強度が0以下になった場合、処理を停止
        if (Light.intensity <= 0) CancelInvoke("DecreaseIntensity");
        else Light.intensity -= 0.01f; // ライトの強度を少し減らす
    }
}