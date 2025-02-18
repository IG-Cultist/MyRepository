/// ==============================
/// �^�C�g���X�N���v�g
/// Author: Nishiura Kouta
/// ==============================
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �^�C�g���V�[���̏������Ǘ�����N���X
/// </summary>
public class Title : MonoBehaviour
{
    // �Q�[���J�n�����摜
    [SerializeField] Image img;
    // �e�̃v���n�u�z��
    [SerializeField] GameObject[] shadows;

    // �N���b�Nor�^�b�vSE
    [SerializeField] AudioClip clickSE;
    // �I�[�f�B�I�\�[�X
    AudioSource audioSource;

    /// <summary>
    /// �ő�����
    /// </summary>
    void Awake()
    {
        Application.targetFrameRate = 60; // ������Ԃ�-1�ɂȂ��Ă���
    }

    /// <summary>
    /// �J�n����
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // ���[�J���̕�������������
        SendData.roomName = "";
        // �����\���̉e��ݒ�
        ChangeShadow();
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    void Update()
    {
        // �摜���������_�ł�����
        img.color = Color.Lerp(new Color32(255,255,255,255), new Color32(255,255,255,0), Mathf.PingPong(Time.time / 1.0f, 1.0f));

        // �N���b�N���ɃN���b�NSE��炵�A���r�[�֑J��
        if (Input.GetMouseButtonUp(0))
        {
            audioSource.PlayOneShot(clickSE);
            GoLobby();
        }

        // SPACE�L�[�������A���r�[�ɑJ��
        if (Input.GetKeyDown(KeyCode.Space)) GoLobby();

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

    /// <summary>
    ///���r�[�J�ڏ���
    /// </summary>
    public void GoLobby()
    {
        // ���r�[��ʂ֑J��
        Initiate.DoneFading();
        Initiate.Fade("Lobby", Color.black, 0.7f);
    }
}
