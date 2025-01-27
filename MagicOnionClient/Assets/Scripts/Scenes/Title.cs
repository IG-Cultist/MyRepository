using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] Image img;
    // 影のプレハブ配列
    [SerializeField] GameObject[] shadows;

    // クリックorタップSE
    [SerializeField] AudioClip clickSE;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SendData.roomName = "";
        ChangeShadow();
    }
    void Update()
    {
        img.color = Color.Lerp(new Color32(255,255,255,255), new Color32(255,255,255,0), Mathf.PingPong(Time.time / 1.0f, 1.0f));

        // クリック時にクリックSEを鳴らす
        if (Input.GetMouseButtonUp(0))
        {
            audioSource.PlayOneShot(clickSE); 
            StartGame();
        }

        // SPACEキー押下時、ロビーに遷移
        if (Input.GetKeyDown(KeyCode.Space)) StartGame();

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

    public void StartGame()
    {
        // ロビー画面へ遷移
        Initiate.DoneFading();
        Initiate.Fade("Lobby", Color.black, 0.7f);
    }
}
