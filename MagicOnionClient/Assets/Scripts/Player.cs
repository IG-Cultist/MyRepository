using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) Stomp();
    }

    /// <summary>
    /// ��e����
    /// </summary>
    /// <param name="obj"></param>
    public void Damage(GameObject obj)
    {
        // ���񂾑���̐ڑ�ID���擾
        otherConnectionID = obj.transform.parent.parent.GetComponent<Player>().connectionID;

        gameDirector.Attack(otherConnectionID);

        DamageEffect(obj.transform);
    }

    /// <summary>
    /// ���݂�����
    /// </summary>
    async void Stomp()
    {
        Camera camera = this.transform.GetChild(0).GetComponent<Camera>();

        // Shot Ray from Touch Point
        Vector3 worldPosition = camera.ScreenToWorldPoint(Input.mousePosition);

        // ������ ���C�͂ǂ̕����ɐi�ނ�(zero=�w��_)
        bool isHit = Physics.Raycast(camera.transform.position, worldPosition - camera.transform.position, out RaycastHit hitInfo);
        Ray rayray = camera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(rayray.origin, rayray.direction * 100, Color.red, 0.5f, false);

        RaycastHit hitted = new RaycastHit();
        if (Physics.Raycast(rayray, out hitted, 100))
        {
            GameObject hitObject = hitted.collider.gameObject;
            if (hitObject.name == "StompZone")
            {
                audioSource.PlayOneShot(stompSE);
                // �J������h�炷
                this.transform.GetChild(0).DOShakePosition(0.2f, 0.1f, 20, 15, false, true);

                gameDirector.moveSpeed = 0.4f;
                Debug.Log("name" + hitObject.name);
                // ���łɑ��Ղ�����ꍇ�����j�󂷂�
                if (footPrintObj != null) Destroy(footPrintObj);

                // �C���X�^���X����
                footPrintObj = Instantiate(footPrintPrefabs);

                // �J�������烌�C���΂�
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray, out hit))
                {
                    // �����ʒu��ݒ�
                    footPrintObj.transform.position = new Vector3(hit.point.x, 0.64f, hit.point.z);
                }

                await Task.Delay(300);
                // ���b��A���Ղ�j��
                if (footPrintObj != null) Destroy(footPrintObj);
                gameDirector.moveSpeed = 1f;
            }
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

            await Task.Delay(1200);
            isHit = false;
        }
        else
        {
            // ��e���킩��₷������悤�����x��������
            transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 100);

            await Task.Delay(1200);
            isHit = false;
            // �����x�����ɖ߂�
            transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 220);
        }
    }
}