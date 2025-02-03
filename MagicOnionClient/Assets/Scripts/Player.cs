/// ==============================
/// �v���C���[�X�N���v�g
/// Name:���Y�W�� Update:02/03
/// ==============================
using System;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // ���Ղ̃v���n�u
    [SerializeField] GameObject footPrintPrefabs;

    // �ڑ�ID
    public Guid connectionID;
    // ���g�̂��̂ł͂Ȃ��ڑ�ID
    Guid otherConnectionID;
    // ��e����
    public bool isHit = false;
    // �������ՃI�u�W�F�N�g
    GameObject footPrintObj;
    // �Q�[���f�B���N�^�[�X�N���v�g
    GameDirector gameDirector;

    // ���݂���SE
    [SerializeField] AudioClip stompSE;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // �Q�[���f�B���N�^�[�X�N���v�g���t�B�[���h���I�u�W�F�N�g����擾
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }

    /// <summary>
    /// ��e����
    /// </summary>
    /// <param name="obj"></param>
    public void Damage(GameObject obj)
    {
        // ���񂾑���̐ڑ�ID���擾
        otherConnectionID = obj.transform.parent.parent.GetComponent<Player>().connectionID;

        // �U������
        gameDirector.Attack(otherConnectionID);

        // ��e�G�t�F�N�g����
        DamageEffect(obj.transform);
    }

    /// <summary>
    /// ���݂�����
    /// </summary>
    public async void Stomp()
    {
        // ���g�̃J�������擾
        Camera camera = this.transform.GetChild(0).GetComponent<Camera>();

        // �^�b�`�����ꏊ�ɃJ�������烌�C�𔭎�
        Vector3 worldPosition = camera.ScreenToWorldPoint(Input.mousePosition);

        Ray rayray = camera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(rayray.origin, rayray.direction * 100, Color.red, 4, false);
        RaycastHit hitted = new RaycastHit();

        if (Physics.Raycast(rayray, out hitted, 100, 1 << 6))
        {
            // ���C�̓��������I�u�W�F�N�g���擾
            GameObject hitObject = hitted.collider.gameObject;
   
            // �I�u�W�F�N�g�����݂��\�G���A�̏ꍇ
            if (hitObject.name == "StompZone")
            {
                // ���݂�SE��炷
                audioSource.PlayOneShot(stompSE);
                // �J������h�炷
                this.transform.GetChild(0).DOShakePosition(0.2f, 0.1f, 20, 15, false, true);
                // �ړ����x��ቺ
                gameDirector.moveSpeed = 0.4f;

                // ���łɑ��Ղ�����ꍇ�����j�󂷂�
                if (footPrintObj != null) Destroy(footPrintObj);

                // �C���X�^���X����
                footPrintObj = Instantiate(footPrintPrefabs);

                // �J�������烌�C���΂�
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray, out hit,10, 1 << 6))
                {
                    // �����ʒu��ݒ�
                    footPrintObj.transform.position = new Vector3(hit.point.x, 0.64f, hit.point.z);
                }

                // 0.5�b��A���Ղ�j��
                await Task.Delay(300);
                if (footPrintObj != null) Destroy(footPrintObj);
                // �ړ����x��߂�
                gameDirector.moveSpeed = 1.5f;
            }

            // ���C�̓��������I�u�W�F�N�g��������
            if (hitObject != null) hitObject = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �g���b�v�𓥂񂾍ۂ����߂Ń_���[�W���󂯂Ă��Ȃ��ꍇ
        if (collision.gameObject.name == "Trap(active)" && isHit == false)
        { 
            // �_���[�W����������
            DamageEffect(this.transform);
            gameDirector.Attack(this.GetComponent<Player>().connectionID);
            // �g���b�v��j��
            Destroy(collision.gameObject);

            // �A�C�e���g�p��������
            gameDirector.OnUseItemUser(this.GetComponent<Player>().connectionID, collision.gameObject.name);
        }
    }

    /// <summary>
    /// ��e������
    /// </summary>
    /// <param name="renderer"></param>
    async void DamageEffect(Transform transform)
    {
        // ��e�����̂������ł���ꍇ
        if (transform.tag != "Shadow_Rival")
        {
            // �J������h�炷
            transform.GetChild(0).DOShakePosition(0.9f, 1.5f, 45, 15, false, true);
            // 2�b�x��
            await Task.Delay(1200);
           �@// ��e�����false��
            isHit = false;
        }
        else
        {
            // ��e���킩��₷������悤�����x��������
            transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 100);

            // 2�b�x��
            await Task.Delay(1200);
           �@// ��e�����false��
            isHit = false;
            // �����x�����ɖ߂�
            transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 220);
        }
    }
}