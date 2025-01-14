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
        if (other.gameObject.tag == "Item")
        {
            Debug.Log(other.gameObject.name);
            // �����j�󂷂�
            Destroy(other.gameObject);
            // ���O�ɉ��������ʂ𔭓�
            gameDirector.StompItem(other.gameObject.name);
        }

        // ���񂾂̂��e�̏ꍇ ���N�[���_�E�����łȂ��ꍇ
        if (other.gameObject.tag == "Shadow_Rival" && player.isHit == false)
        {
            player.Damage(other.gameObject);
            player.isHit = true;
        }
    }
}
