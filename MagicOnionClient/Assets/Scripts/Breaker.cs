/// ==============================
/// �I�u�W�F�N�g�j��G���A�X�N���v�g
/// Author: Nishiura Kouta
/// ==============================
using UnityEngine;

/// <summary>
/// �I�u�W�F�N�g�j��Ǘ��N���X
/// </summary>
public class Breaker : MonoBehaviour
{
    /// <summary>
    /// �g���K�[���I�u�W�F�N�g����
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // �G�ꂽ�I�u�W�F�N�g���w���X�̏ꍇ�A�����j��
        if (other.gameObject.tag == "Health")
        {
            Destroy(other.gameObject);
        }
    }
}
