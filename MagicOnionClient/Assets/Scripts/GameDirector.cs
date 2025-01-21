/// ==============================
/// ゲームディレクタースクリプト
/// Name:西浦晃太 Update:12/24
/// ==============================
using DG.Tweening;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TedLab;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class GameDirector : MonoBehaviour
{
    // 生成するユーザプレハブ
    [SerializeField] GameObject characterPrefabs; 
    // 生成するトラッププレハブ
    [SerializeField] GameObject trapPrefabs;
    // 退室ボタン
    [SerializeField] GameObject exitButton;
    // カウントダウン表示パネル
    [SerializeField] GameObject countPanel;

    // 移動速度上昇アイコン
    [SerializeField] GameObject speedUpEffect;
    // 位置表示アイコン
    [SerializeField] GameObject localizationEffect;
    // 影生成アイコン
    [SerializeField] GameObject spawnShadowEffect;

    // 視点変更ボタン
    [SerializeField] GameObject viewButton;
    // カウントダウンテキスト
    [SerializeField] Text countText;
    // 所有アイテムパネル
    [SerializeField] Image itemPanel;
    // 操作用ジョイスティック
    [SerializeField] FixedJoystick joystick;

    // 生成ユーザのディクショナリー
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();
    // 部屋モデル
    [SerializeField] RoomHubModel roomModel;

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
    // SE
    [SerializeField] AudioClip compassSE;
    // ローラブレード使用SE
    [SerializeField] AudioClip rollerBladeSE;

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

    // ゲーム開始判定変数
    bool isStart = false;
    // ゲーム終了判定変数
    bool isFinish = false;

    // 移動速度
    public float moveSpeed = 1f;

    public int userID;

    // 現在所有しているアイテム名
    string nowItemName = "";

    // 移動速度ブースト判定
    bool isBoost = false;

    // 影生成判定
    bool isShadowSpawn = false;

    // 位置特定判定
    bool isLocate = false;

    // マスタークライアント判定
    bool isMaster;

    // 制限時間
    int time = 30;
    // タイムアップ時SE再生回数用変数
    int timeUpCnt = 0;

    // 視点変更用変数
    int viewCount = 2;
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
        if (Input.GetKey(KeyCode.Escape))
        {//ESC押した際の処理
#if UNITY_EDITOR
            //エディター実行時
            UnityEditor.EditorApplication.isPlaying = false;
#else
            //ビルド時
            Application.Quit();
#endif
        }

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

        itemScript = GameObject.Find("ItemManager").GetComponent<Item>();

        // 非表示にする
        exitButton.SetActive(false);
        spawnShadowEffect.SetActive(false);
        localizationEffect.SetActive(false);
        speedUpEffect.SetActive(false);

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

        await Task.Delay(600);

        JoinRoom();

    }

    void Update()
    {
        MovePlayer();

        // 押下中処理
        if (buttonDownFlag)
        {
            // 時間切れになったら強制的に視点を戻す
            if (viewCount <= 0) OnButtonUp();
        }

        // ゲーム終了状態かつ画面タップをした場合
        if (isFinish == true && Input.GetMouseButtonDown(0))
        {
            LeaveRoom();
        }

        if (timeUpCnt >= 4) CancelInvoke("TimeUp");
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

        int count;
        if (characterList.Count <= 0)
        {
            count =1;
        }
        else count = 3;

        // 生成位置を設定
        characterGameObject.transform.position = new Vector3(-7.5f + (2 * count), 2f, -10f);

        characterGameObject.name = user.UserData.Id.ToString();

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
            obj.tag = "Camera_Rival";
            characterGameObject.tag = "Rival";
        }
        else if (roomModel.ConnectionID == user.ConnectionID)
        {
            if (user.JoinOrder == 1)
            {
                isMaster = true;
            }

            // 自身のカメラを取得
            GameObject obj = characterGameObject.transform.GetChild(0).gameObject;
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

        ChangeSkin(user.UserData.Id, user.SkinName);

        characterList[user.ConnectionID] = characterGameObject; // フィールドで保持
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
        // 退室ユーザのオブジェクトのみ破壊
        else Destroy(characterList[connectionID]);
    }

    /// <summary>
    /// ユーザ移動処理
    /// </summary>
    /// <param name="pos">位置</param>
    void OnMovedUser(Guid connectionID, Vector3 pos, Vector3 rot, IRoomHubReceiver.PlayerState state)
    {
        if(characterList.Count == 0) return;
        // 各トランスフォームをアニメーション
        characterList[connectionID].transform.DOLocalMove(pos,0.1f).SetEase(Ease.Linear);
        characterList[connectionID].transform.DOLocalRotate(rot,0.1f).SetEase(Ease.Linear);
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
        itemScript.StartSpawn();
        countPanel.SetActive(true);
        InvokeRepeating("CountDown", 0.1f, 1f);
        isStart = true;
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    void FinishGame()
    {
        exitButton.SetActive(false);
        CancelInvoke("CountDown");
        isFinish = true;
        InvokeRepeating("TimeUp", 0.1f, 2f);   
    }

    public async void JoinRoom()
    {
        await roomModel.JoinAsync(SendData.roomName, SendData.userID, SendData.skinName);

        InvokeRepeating("Move", 0.1f, 0.1f);

        isStart = true;
        if (isMaster == true) StartGame();
    }

    /// <summary>
    /// 退室ボタン処理
    /// </summary>
    public async void LeaveRoom()
    {
        CancelInvoke("Move");
        // 退室
        await roomModel.LeaveAsync(SendData.roomName, SendData.userID);
        Initiate.DoneFading();
        Initiate.Fade("Title", Color.black, 0.7f);
    }

    /// <summary>
    /// 移動キー処理
    /// </summary>
    public async void Move()
    {
        if (characterList[roomModel.ConnectionID] == null) return;
        // 移動
        await roomModel.MoveAsync(characterList[roomModel.ConnectionID].transform.position,
            characterList[roomModel.ConnectionID].transform.eulerAngles,
            IRoomHubReceiver.PlayerState.Move);
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

        // 受け取った接続IDと自身の接続IDが一致している場合
        if (connectionID == roomModel.ConnectionID)
        {
            // カメラを揺らす
            characterList[roomModel.ConnectionID].transform.GetChild(0).DOShakePosition(0.6f, 1.5f, 45, 15, false, true);

            Destroy(heartList[health]); 
            //heartList.Remove(heartList[health]);
        }
        else
        {
            Destroy(rivalHeartList[health]);
            //rivalHeartList.Remove(heartList[health]);
        }

        if (health <= 0)
        {
            characterList[connectionID].SetActive(false);
            Finish();
        }
    }

    /// <summary>
    /// キー入力移動処理
    /// </summary>
    void MovePlayer()
    {
        // 開始前とゲーム終了後は移動させない
        if (isStart != true || isFinish == true) return;
        Rigidbody rb = characterList[roomModel.ConnectionID].GetComponent<Rigidbody>();  // rigidbodyを取得

        float axisZ = joystick.Vertical;
        float axisX = joystick.Horizontal;

        rb.AddRelativeForce(new Vector3(90 * axisX, 0, 90 * axisZ));

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
        // リソースから、アイテムテクスチャを取得
        Texture2D texture = Resources.Load("Items/" + name) as Texture2D;

        itemPanel.sprite = 
            Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        nowItemName = name;

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

        // アイテム別処理
        switch (nowItemName)
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
                // フィールドにトラップを設置
                Vector3 playerPos = characterList[roomModel.ConnectionID].transform.position;
                // インスタンス生成
                GameObject trapObj = Instantiate(trapPrefabs);
                trapObj.name = "Trap(active)";
                // 生成位置を設定
                trapObj.transform.position = new Vector3(playerPos.x, playerPos.y + 1.0f, playerPos.z + 3.0f); 

                await roomModel.UseItemAsync(roomModel.ConnectionID, nowItemName);
                break;

            case "Projector": // 投影機の場合
                // 5秒間自由に動く偽の影を召喚する
                audioSource.PlayOneShot(projectorSE);

                spawnShadowEffect.SetActive(true);
                isShadowSpawn = true;
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
            moveSpeed = 1.0f;
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

        // 投影機を使用していた場合
        if (isShadowSpawn == true)
        {       
            // 5秒後に影を削除
            await Task.Delay(3000);
            spawnShadowEffect.SetActive(false);
            isShadowSpawn = false;
        }
    }

    public async void OnUseItemUser(Guid connectionID, string itemName)
    {
        switch (itemName)
        {
            case "Trap": // トラップの場合
                audioSource.PlayOneShot(trapSE);
                // フィールドにトラップを設置
                Vector3 playerPos = characterList[connectionID].transform.position;
                // インスタンス生成
                GameObject trapObj = Instantiate(trapPrefabs);
                trapObj.name = "Trap(active)";
                // 生成位置を設定
                trapObj.transform.position = new Vector3(playerPos.x, playerPos.y + 1.0f, playerPos.z + 3.0f);

                break;

            case "Projector": // 投影機の場合
                // 5秒間自由に動く偽の影を召喚する
                audioSource.PlayOneShot(projectorSE);
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
        // 減算結果をUIに適応
        countText.text = time.ToString();
        // タイムアップ時
        if (time <= 0)
        {
            // UIにタイムアップと表記する
            countText.text = "Time UP";
            // カウントダウン処理を停止
            CancelInvoke("CountDown");
            // ゲームエンド処理を呼ぶ
            Finish();
        }
    }
    void OnCountUser(int time)
    {
        this.time = time;

        // 減算結果をUIに適応
        countText.text = this.time.ToString();
        // タイムアップ時
        if (this.time <= 0)
        {
            // UIにタイムアップと表記する
            countText.text = "Time UP";
            // カウントダウン処理を停止
            CancelInvoke("CountDown");
            // ゲームエンド処理を呼ぶ
            Finish();
        }
    }

    void OnChangeSkinUser(int userID, string skinName)
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
        if (isCooldown) return; // クールダウン中の場合、処理しない

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
        if (isCooldown) return; // クールダウン中の場合、処理しない

        // クールダウン中にする
        isCooldown = true;
        // カウントダウンを停止
        CancelInvoke("ViewTime");
        // 押下フラグをfalseに
        buttonDownFlag = false;

        // カメラの回転を元に戻す
        myCamera.transform.DOLocalRotate(new Vector3(30f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
        //移動速度を元に戻す
        moveSpeed = 1f;

        // リソースから、アイコンを取得
        Texture2D texture = Resources.Load("UI/eye_close") as Texture2D;
        
        Image buttonTexture = viewButton.GetComponent<Image>();

        buttonTexture.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                       Vector2.zero);

        InvokeRepeating("ViewCooldown", 3f, 1f);

    }

    void ViewTime()
    {
        viewCount--;
    }

    void ViewCooldown()
    {
        CancelInvoke("ViewCooldown");

        // リソースから、アイコンを取得
        Texture2D texture = Resources.Load("UI/eye_open") as Texture2D;

        Image buttonTexture = viewButton.GetComponent<Image>();

        buttonTexture.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                       Vector2.zero);
        isCooldown = false;
    }
}