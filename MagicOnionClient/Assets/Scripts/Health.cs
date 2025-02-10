/// ==============================
/// �w���X�X�N���v�g
/// Author: Nishiura Kouta
/// ==============================
using UnityEngine;

/// <summary>
/// �w���X�I�u�W�F�N�g�����N���X
/// </summary>
public class Health : MonoBehaviour
{
    // ��������w���X�̃v���n�u
    [SerializeField] GameObject heartPrefabs;

    /// <summary>
    /// �ő�����
    /// </summary>
    void Awake()
    {
        // �w���X�̐���
        SetHP();        
    }

    /// <summary>
    /// �w���X��������
    /// </summary>
    public void SetHP()
    {
        // 3�񃋁[�v
        for (int i = 0; i < 3; i++)
        {
            // �v���n�u����Q�[���I�u�W�F�N�g�𐶐�
            GameObject itemObj = Instantiate(heartPrefabs);
            // ���O��������A���ʔԍ�������
            itemObj.name = "Heart_" + (i +1);
            // �ʒu��ݒ�
            itemObj.transform.position = new Vector3(5f + (1.5f * i), -16.5f, 10f);

            // ��L�̏��������C�o�������l�ɂ���
            itemObj = Instantiate(heartPrefabs);
            itemObj.name = "Rival_Heart_" + (i + 1);
            itemObj.transform.position = new Vector3(-8f + (1.5f * i), -16.5f, 10f);
        }
    }
}
