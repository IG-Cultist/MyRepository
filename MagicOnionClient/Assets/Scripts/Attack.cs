/// ==============================
/// �A�^�b�N�X�N���v�g
/// Name:���Y�W�� Update:02/03
/// ==============================
using UnityEngine;

public class Attack : MonoBehaviour
{
    // �Q�[���f�B���N�^�[�X�N���v�g
    GameDirector gameDirector;
    // �v���C���[�X�N���v�g
    Player player;

    void Start()
    {
        // �e�X�N���v�g���t�B�[���h���I�u�W�F�N�g����擾
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        player = GameObject.Find(SendData.userID.ToString()).GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // ���񂾂̂��A�C�e���̏ꍇ
        if (other.gameObject.tag == "Item" || other.gameObject.tag == "Fake")
        {
            // �����j�󂷂�
            Destroy(other.gameObject);
            // ���O�ɉ��������ʂ𔭓�
            gameDirector.StompItem(other.gameObject.name);
        }

        // ���񂾂̂��e�̏ꍇ ���N�[���_�E�����łȂ��ꍇ
        if (other.gameObject.tag == "Shadow_Rival" && player.isHit == false)
        {
            // ��e����
            player.Damage(other.gameObject);
            // ��e�����true��
            player.isHit = true;
        }
    }
}
