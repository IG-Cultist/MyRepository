/// ==============================
/// �t�F�C�N�V���h�E�X�N���v�g
/// Name:���Y�W�� Update:1/24
/// ==============================
using UnityEngine;

public class FakeShadow : MonoBehaviour
{
    // ���̃I�u�W�F�N�g�̃��W�b�g�{�f�B
    Rigidbody rb;
    // �����V�X�e��
    System.Random rand;
    // �\���X�L��
    [SerializeField] GameObject[] shadows;

    void Start()
    {
        rand = new System.Random();

        // �����ړ��l�̃����_���ݒ�
        int num = rand.Next(0, 2);

        rb = this.transform.GetComponent<Rigidbody>();  // rigidbody���擾

        // ���ˊp�x����
        switch (num)
        {
            case 0: // �E���֔���
                rb.AddForce(new Vector3(8f, 0f, 8f), ForceMode.Impulse);
                break;
            case 1: // �����֔���
                rb.AddForce(new Vector3(-8f, 0f, 8f), ForceMode.Impulse);
                break;
        }

        // �S�X�L�����\��
        foreach (var obj in shadows)
        {
            obj.SetActive(false);
        }

        // �X�L���ݒ�
        SetSkin();
    }

    /// <summary>
    /// �X�L����ݒ�
    /// </summary>
    void SetSkin()
    {
        // �X�L���l�̃����_���ݒ�
        int num = rand.Next(0, this.transform.childCount);

        // ���I���ꂽ���̂�\��
        shadows[num].gameObject.SetActive(true);
    }
}
