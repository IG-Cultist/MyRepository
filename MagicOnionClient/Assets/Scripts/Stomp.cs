/// ==============================
/// ��ʃ^�b�`�X�N���v�g
/// Author: Nishiura Kouta
/// ==============================
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ��ʃ^�b�`�ł̏������ĂԃN���X
/// </summary>
public class Stomp : MonoBehaviour, IPointerDownHandler
{
    /// <summary>
    /// �Ή��͈̓^�b�`����
    /// </summary>
    /// <param name="pointerEventData"></param>
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        // �t�B�[���h���I�u�W�F�N�g����v���C���[�X�N���v�g���擾���A���ݏ������Ă�
        GameObject.FindWithTag("Player").GetComponent<Player>().Stomp();
    }
}
