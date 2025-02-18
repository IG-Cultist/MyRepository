/// ==============================
/// タイトルスクリプト
/// Author: Nishiura Kouta
/// ==============================
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タイトルシーンの処理を管理するクラス
/// </summary>
public class Title : MonoBehaviour
{
    // ゲーム開始促し画像
    [SerializeField] Image img;
    // 影のプレハブ配列
    [SerializeField] GameObject[] shadows;

    // クリックorタップSE
    [SerializeField] AudioClip clickSE;
    // オーディオソース
    AudioSource audioSource;

    /// <summary>
    /// 最速処理
    /// </summary>
    void Awake()
    {
        Application.targetFrameRate = 60; // 初期状態は-1になっている
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // ローカルの部屋名を初期化
        SendData.roomName = "";
        // 初期表示の影を設定
        ChangeShadow();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        // 画像をゆっくり点滅させる
        img.color = Color.Lerp(new Color32(255,255,255,255), new Color32(255,255,255,0), Mathf.PingPong(Time.time / 1.0f, 1.0f));

        // クリック時にクリックSEを鳴らし、ロビーへ遷移
        if (Input.GetMouseButtonUp(0))
        {
            audioSource.PlayOneShot(clickSE);
            GoLobby();
        }

        // SPACEキー押下時、ロビーに遷移
        if (Input.GetKeyDown(KeyCode.Space)) GoLobby();

        //TABキーで影を変更(隠し機能)
        if (Input.GetKeyDown(KeyCode.Tab)) ChangeShadow();
    }

    /// <summary>
    /// 影変更処理
    /// </summary>
    void ChangeShadow()
    {
        // 全ての影を非表示に
        for (int i = 0; i < shadows.Length; i++)
        {
            shadows[i].SetActive(false);
        }

        // 乱数設定
        System.Random rand = new System.Random();
        int rndNum = rand.Next(0, shadows.Length);

        // 抽選された影を表示する
        shadows[rndNum].SetActive(true);
    }

    /// <summary>
    ///ロビー遷移処理
    /// </summary>
    public void GoLobby()
    {
        // ロビー画面へ遷移
        Initiate.DoneFading();
        Initiate.Fade("Lobby", Color.black, 0.7f);
    }
}
