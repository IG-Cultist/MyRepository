/// ==============================
/// ゲームディレクタースクリプト
/// Name:西浦晃太 Update:12/11
/// ==============================
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;

public class GameDirector : MonoBehaviour
{
    // 生成するユーザプレハブ
    [SerializeField] GameObject characterPrefabs;
    // 退室ボタン
    [SerializeField] GameObject exitButton;
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

    List<GameObject> skinList = new List<GameObject>();
    int skinNum = 0;

    GameObject myCamera;

    Player playerScript;
    // ゲーム開始判定変数
    bool isStart = false;

    bool isFinish = false;

    float moveSpeed = 1f;

    // Start is called before the first frame update
    async void Start()
    {
#if UNITY_EDITOR
        //エディター実行時
        SendData.roomName = "RoomRoom";
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

        // 非表示にする
        exitButton.SetActive(false);
        countPanel.SetActive(false);

        // ユーザが入室したときにメソッドを実行するようモデルに登録
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMovedUser += this.OnMovedUser;
        roomModel.OnReadyUser += this.OnReadyUser;
        roomModel.OnFinishUser += this.OnFinishUser;
        roomModel.OnAttackUser += this.OnAttackUser;
        // 接続
        await roomModel.ConnectAsync();

        await Task.Delay(600);

        JoinRoom();
    }

    void Update()
    {
        MovePlayer();

        // ゲーム終了状態かつ画面タップをした場合
        if (isFinish == true && Input.GetMouseButtonDown(0))
        {
            LeaveRoom();
            SceneManager.LoadScene("Result");
        }

        // ゲーム終了状態かつ画面タップをした場合
        if (Input.GetKeyDown(KeyCode.L))
        {
            isStart = true;
        }

        // ゲーム終了状態かつ画面タップをした場合
        if (Input.GetKeyDown(KeyCode.C))
        {
            SpawnItem();
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            ChangeSkin();
        }

        if (myCamera != null)
        {
            LookUp();
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

        int count;
        if (characterList.Count <= 0)
        {
            count =1;
        }
        else count = 3;

        // 生成位置を設定
        characterGameObject.transform.position = new Vector3(-7.5f + (2 * count), 2f, -10f);

        // 自分以外のカメラを非アクティブにする
        if(roomModel.ConnectionID != user.ConnectionID){
            // 対象ののカメラを取得
            GameObject obj = characterGameObject.transform.GetChild(0).gameObject;
            // カメラを非アクティブ化
            obj.SetActive(false);
        }
        else if (roomModel.ConnectionID == user.ConnectionID)
        {
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
        }

        Transform children = characterGameObject.GetComponentInChildren<Transform>();

        foreach (Transform ob in children)
        {
            GameObject obj;
            switch (ob.name)
            {
                case "shadow":
                    obj = GameObject.Find (ob.name);
                    obj.SetActive(true);
                    skinList.Add(obj);
                    break;
                case "shadow_face":
                    obj = GameObject.Find(ob.name);
                    obj.SetActive(false);
                    skinList.Add(obj);
                    break;
                case "shadow_eye":
                    obj = GameObject.Find(ob.name);
                    obj.SetActive(false);
                    skinList.Add (obj);
                    break;
                default:
                    break;
            }
        }

        // 識別番号を各子オブジェクトの名前に付ける
        foreach (Transform obj in characterGameObject.transform)
        {
            obj.name += "_" + user.UserData.Id;
        }

        characterGameObject.name = user.UserData.Id.ToString();

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
        countText.text = "Start";
        isStart = true;
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    void FinishGame()
    {
        exitButton.SetActive(false);
        countText.text = "クリックで退室";
        isFinish = true;
    }

    public async void JoinRoom()
    {
        System.Random rand = new System.Random();
        int id = rand.Next(1, 4);

        //ConnectPanel.SetActive(false);
        //int.TryParse(userID.text, out int id);

        await roomModel.JoinAsync(SendData.roomName, id);
        InvokeRepeating("Move", 0.1f, 0.1f);
        Ready();
    }

    /// <summary>
    /// 退室ボタン処理
    /// </summary>
    public async void LeaveRoom()
    {
        CancelInvoke("Move");
        int.TryParse(userID.text, out int id);
        // 退室
        await roomModel.LeaveAsync(SendData.roomName, id);
        SceneManager.LoadScene("Title");
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
        // 開始前とゲーム終了後は移動させない
        if (isStart != true || isFinish == true) return;
        Rigidbody rb = characterList[roomModel.ConnectionID].GetComponent<Rigidbody>();  // rigidbodyを取得

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
    /// スキン(影)変更処理
    /// </summary>
    void ChangeSkin()
    {
        switch (skinNum)
        {
            case 0:
                skinList[0].SetActive(false);
                skinList[1].SetActive(true);
                skinList[2].SetActive(false);
                skinNum = 1;
                break;
            case 1:
                skinList[0].SetActive(false);
                skinList[1].SetActive(false);
                skinList[2].SetActive(true);
                skinNum = 2;
                break;
            case 2:
                skinList[0].SetActive(true);
                skinList[1].SetActive(false);
                skinList[2].SetActive(false);
                skinNum = 0;
                break;
        }
    }

    /// <summary>
    /// 視点変更処理
    /// </summary>
    void LookUp()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            myCamera.transform.DOLocalRotate(new Vector3(5f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
            moveSpeed = 0.2f;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            myCamera.transform.DOLocalRotate(new Vector3(30f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
            moveSpeed = 1f;
        }
    }

    /// <summary>
    /// アイテム生成時の処理
    /// </summary>
    void SpawnItem()
    {
        LookAtCamera camScript = FindAnyObjectByType<LookAtCamera>();
        Camera camera = myCamera.GetComponent<Camera>();
        camScript.GetCamera(camera);
    }
}
