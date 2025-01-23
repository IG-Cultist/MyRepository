using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
    // �N���b�Nor�^�b�vSE
    [SerializeField] AudioClip clickSE;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// �^�C�g���֖߂鏈��
    /// </summary>
    public void GoTitle()
    {
        audioSource.PlayOneShot(clickSE);
        Initiate.DoneFading();
        Initiate.Fade("Title", Color.black, 0.7f);
    }

    /// <summary>
    /// ���r�[�ɖ߂鏈��
    /// </summary>
    public void PlayAgain()
    {
        audioSource.PlayOneShot(clickSE);
        Initiate.DoneFading();
        Initiate.Fade("Lobby", Color.black, 0.7f);
    }
}
