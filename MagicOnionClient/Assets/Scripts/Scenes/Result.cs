using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
    // クリックorタップSE
    [SerializeField] AudioClip clickSE;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// タイトルへ戻る処理
    /// </summary>
    public void GoTitle()
    {
        audioSource.PlayOneShot(clickSE);
        Initiate.DoneFading();
        Initiate.Fade("Title", Color.black, 0.7f);
    }

    /// <summary>
    /// ロビーに戻る処理
    /// </summary>
    public void PlayAgain()
    {
        audioSource.PlayOneShot(clickSE);
        Initiate.DoneFading();
        Initiate.Fade("Lobby", Color.black, 0.7f);
    }
}
