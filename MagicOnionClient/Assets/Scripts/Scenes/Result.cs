/// ==============================
/// ���U���g�V�[���X�N���v�g
/// Author: Nishiura Kouta
/// ==============================
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// �Q�[���I����ʂ̏������Ǘ�����N���X
/// </summary>
public class Result : MonoBehaviour
{
    // �N���b�Nor�^�b�vSE
    [SerializeField] AudioClip clickSE;

    // �I�[�f�B�I�\�[�X
    AudioSource audioSource;

    /// <summary>
    /// �J�n����
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// �^�C�g���֖߂鏈��
    /// </summary>
    public void GoTitle()
    {
        // �N���b�NSE��炷
        audioSource.PlayOneShot(clickSE);
        // �^�C�g���V�[���֑J��
        Initiate.DoneFading();
        Initiate.Fade("Title", Color.black, 0.7f);
    }

    /// <summary>
    /// ���r�[�ɖ߂鏈��
    /// </summary>
    public void PlayAgain()
    {
        // �N���b�NSE��炷
        audioSource.PlayOneShot(clickSE);
        // ���r�[�V�[���֑J��
        Initiate.DoneFading();
        Initiate.Fade("Lobby", Color.black, 0.7f);
    }
}
