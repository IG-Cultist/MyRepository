using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] Image img;
    // �e�̃v���n�u�z��
    [SerializeField] GameObject[] shadows;

    // �N���b�Nor�^�b�vSE
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

        // �N���b�N���ɃN���b�NSE��炷
        if (Input.GetMouseButtonUp(0))
        {
            audioSource.PlayOneShot(clickSE); 
            StartGame();
        }

        // SPACE�L�[�������A���r�[�ɑJ��
        if (Input.GetKeyDown(KeyCode.Space)) StartGame();

        //TAB�L�[�ŉe��ύX(�B���@�\)
        if (Input.GetKeyDown(KeyCode.Tab)) ChangeShadow();
    }

    /// <summary>
    /// �e�ύX����
    /// </summary>
    void ChangeShadow()
    {
        // �S�Ẳe���\����
        for (int i = 0; i < shadows.Length; i++)
        {
            shadows[i].SetActive(false);
        }

        // �����ݒ�
        System.Random rand = new System.Random();
        int rndNum = rand.Next(0, shadows.Length);

        // ���I���ꂽ�e��\������
        shadows[rndNum].SetActive(true);
    }

    public void StartGame()
    {
        // ���r�[��ʂ֑J��
        Initiate.DoneFading();
        Initiate.Fade("Lobby", Color.black, 0.7f);
    }
}
