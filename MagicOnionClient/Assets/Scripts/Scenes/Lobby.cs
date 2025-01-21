/// ==============================
/// ロビースクリプト
/// Name:西浦晃太 Update:01/20
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
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    // 参加者表示テキスト
    [SerializeField] Text[] joinedUserName;
    // 退室ボタン
    [SerializeField] GameObject exitButton;
    // 準備ボタン
    [SerializeField] GameObject readyButton;
    // 部屋モデル
    [SerializeField] RoomHubModel roomModel;
    // ロードパネル
    [SerializeField] GameObject loadingPanel;
    // 説明パネル
    [SerializeField] GameObject explainPanel;
    // 説明画像
    [SerializeField] GameObject[] explainImages;
    // 説明画像
    [SerializeField] GameObject[] explainTitles;
    // ロードアイコン
    [SerializeField] GameObject loadingIcon;
    // スキン変更パネル
    [SerializeField] GameObject skinPanel;
    // スキン変更ボタン
    [SerializeField] GameObject[] changeButton;
    // ヘッダーテキスト
    [SerializeField] Text headerText;
    // 参加ユーザの接続ID保存リスト
    List<Guid> idList = new List<Guid>();

    int count = 0;

    // 説明画像用変数
    int imageCnt = 0;

    // クリックorタップSE
    [SerializeField] AudioClip clickSE;

    AudioSource audioSource;

    // Start is called before the first frame update
    async void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // 非表示にする
        loadingPanel.SetActive(false);
        skinPanel.SetActive(false);
        readyButton.SetActive(false);
        explainPanel.SetActive(false);
        // ユーザが入室したときにメソッドを実行するようモデルに登録
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMatchingUser += this.OnMatchingUser;

        // 接続
        await roomModel.ConnectAsync();

        // 少し待つ
        await Task.Delay(300);

        // ロビー入室
        JoinRoom();
    }

    void Update()
    {
        // 左クリックもしくは画面タップ時クリックSEを出す
        if (Input.GetMouseButtonUp(0)) audioSource.PlayOneShot(clickSE);
    }

    /// <summary>
    /// ユーザ入室処理
    /// </summary>
    /// <param name="user"></param>
    void OnJoinedUser(JoinedUser user)
    {
        // 入室者が2名以上の場合処理しない
        if (idList.Count >= 2) return;
        // 参加者IDリストに入れる
        idList.Add(user.ConnectionID);

        int cnt = 0;
        // 表示用参加者名をIDにする
        foreach (var id in idList)
        {
            joinedUserName[cnt].text = id.ToString();
            cnt++;
        }
    }

    /// <summary>
    /// ユーザ退室処理
    /// </summary>
    /// <param name="connectionID"></param>
    void OnLeavedUser(Guid connectionID)
    {
        for (int i = 0; i < joinedUserName.Length; i++)
        {
            if (joinedUserName[i].text == connectionID.ToString())
            {
                // Player1が抜けた場合
                if (i == 0)
                {
                    // Player2の名前をPlayer1の場所に移す
                    joinedUserName[0].text = joinedUserName[1].text;
                    joinedUserName[1].text = "";
                }
                else // 離脱者の名前を削除
                {
                    joinedUserName[i].text = "";
                }
                
                idList.Remove(connectionID);
            }
        }
    }

    /// <summary>
    /// 入室処理
    /// </summary>
    public async void JoinRoom()
    {
        //System.Random rand = new System.Random();
        // 1～10までの乱数を代入
        //int id = rand.Next(1, 4);

        // 入室
        await roomModel.JoinLobbyAsync(SendData.userID);

        // 各ボタンを表示
        exitButton.SetActive(true);
        skinPanel.SetActive(true);
        readyButton.SetActive(true);
    }

    /// <summary>
    /// 退室ボタン処理
    /// </summary>
    public async void LeaveRoom()
    {
        // 退室
        await roomModel.LeaveAsync("Lobby", 1);
        //SceneManager.LoadScene("Title");

        Initiate.DoneFading();
        Initiate.Fade("Title", Color.black, 0.7f);
    }

    /// <summary>
    /// 準備完了ボタン処理
    /// </summary>
    public async void Ready()
    {
        // 選択スキン判定
        string sendStr;
        switch (count)
        {
            case 0:
                sendStr = "shadow_normal";
                break;
            case 1:
                sendStr = "shadow_eye";
                break;
            case 2:
                sendStr = "shadow_face";
                break;
            case 3:
                sendStr = "shadow_mouth";
                break;
            default:
                sendStr = "";
                break;
        }
        // 送信用スキン名に代入
        SendData.skinName = sendStr;

        // 準備完了処理
        await roomModel.ReadyAsync();
        // 準備完了ボタンを非表示に
        readyButton.SetActive(false);

        // スキン変更ボタンを非表示に
        changeButton[0].SetActive(false);
        changeButton[1].SetActive(false);
    }

    async void OnMatchingUser(string roomName)
    {
        // 送るデータを代入
        SendData.roomName = roomName;
        SendData.idList = idList;

        Loading();
        await Task.Delay(800);

        Initiate.DoneFading();
        Initiate.Fade("Game", Color.black, 0.7f);
    }

    /// <summary>
    /// ローディング
    /// </summary>
    async void Loading()
    {
        loadingPanel.SetActive(true);
        headerText.text = "まもなく開始...";

        float angle = 8;
        bool rot = true;

        for (int i = 0; i < 80; i++)
        {
            if (loadingIcon != null)
            {
                if (rot)
                {
                    loadingIcon.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.back);
                }
                else
                {
                    loadingIcon.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.forward);
                }
                await Task.Delay(10);
            }
        }
        loadingPanel.SetActive(false);
    }

    /// <summary>
    /// 次のスキン表示処理
    /// </summary>
    public void nextSkin()
    {
        count++;
        if (count >= 4) count = 0;

        // リソースから、アイコンを取得
        Texture2D texture = Resources.Load("Shadows/shadow_" + count) as Texture2D;

        Image skinPreview =  skinPanel.transform.GetChild(0).GetComponent<Image>();

        skinPreview.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                       Vector2.zero);
    }

    /// <summary>
    /// 1つ前のスキン表示処理
    /// </summary>
    public void backSkin()
    {
        count--;
        if (count < 0) count = 3;

        // リソースから、アイコンを取得
        Texture2D texture = Resources.Load("Shadows/shadow_" + count) as Texture2D;

        Image skinPreview = skinPanel.transform.GetChild(0).GetComponent<Image>();

        skinPreview.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                       Vector2.zero);
    }

    /// <summary>
    /// 説明パネル表示処理
    /// </summary>
    public void openExplain()
    {
        explainPanel.SetActive(true);
        explainImages[0].SetActive(true);
        explainImages[1].SetActive(false);
        explainTitles[0].SetActive(true);
        explainTitles[1].SetActive(false);
    }

    /// <summary>
    /// 説明パネル非表示処理
    /// </summary>
    public void closeExplain()
    {
        explainPanel.SetActive(false);
    }

    /// <summary>
    /// 次の説明画像表示処理
    /// </summary>
    public void nextImage()
    {
        if (imageCnt >= 1)
        {
            imageCnt = 0;
            explainImages[0].SetActive(true);
            explainImages[1].SetActive(false);
            explainTitles[0].SetActive(true);
            explainTitles[1].SetActive(false);
        }
        else
        {
            imageCnt++;
            explainImages[0].SetActive(false);
            explainImages[1].SetActive(true);
            explainTitles[0].SetActive(false);
            explainTitles[1].SetActive(true);
        }
    }
  
    /// <summary>
    /// 一つ前の説明画像表示処理
    /// </summary>
    public void backImage()
    {
        if (imageCnt <= 0)
        {
            imageCnt = 1;
            explainImages[0].SetActive(false);
            explainImages[1].SetActive(true);
            explainTitles[0].SetActive(false);
            explainTitles[1].SetActive(true);
        }
        else
        {
            imageCnt--;
            explainImages[0].SetActive(true);
            explainImages[1].SetActive(false);
            explainTitles[0].SetActive(true);
            explainTitles[1].SetActive(false);
        }
    }
}
