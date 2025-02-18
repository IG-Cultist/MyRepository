/// ==============================
/// リザルトシーンスクリプト
/// Author: Nishiura Kouta
/// ==============================
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム終了画面の処理を管理するクラス
/// </summary>
public class Result : MonoBehaviour
{
    // クリックorタップSE
    [SerializeField] AudioClip clickSE;

    // オーディオソース
    AudioSource audioSource;

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// タイトルへ戻る処理
    /// </summary>
    public void GoTitle()
    {
        // クリックSEを鳴らす
        audioSource.PlayOneShot(clickSE);
        // タイトルシーンへ遷移
        Initiate.DoneFading();
        Initiate.Fade("Title", Color.black, 0.7f);
    }

    /// <summary>
    /// ロビーに戻る処理
    /// </summary>
    public void PlayAgain()
    {
        // クリックSEを鳴らす
        audioSource.PlayOneShot(clickSE);
        // ロビーシーンへ遷移
        Initiate.DoneFading();
        Initiate.Fade("Lobby", Color.black, 0.7f);
    }
}
