using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject footPrintPrefabs;

    public Guid connectionID;
    Guid otherConnectionID;
    public bool isHit = false;

    GameObject footPrintObj;

    GameDirector gameDirector;
    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) Stomp();
    }

    /// <summary>
    /// �U������
    /// </summary>
    /// <param name="obj"></param>
    public async void GetID(GameObject obj)
    {
        // ���񂾑���̐ڑ�ID���擾
        otherConnectionID = obj.transform.parent.GetComponent<Player>().connectionID;

        // ���񂾑���̉e�̃����_���[���擾
        Renderer renderer = obj.GetComponent<Renderer>();

        gameDirector.Attack(otherConnectionID);
        // �F��Ԃ����A����������߂�
        renderer.material.color = new Color(127, 0, 0);
        await Task.Delay(1200);
        renderer.material.color = Color.black;
        isHit = false;
    }

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
                    footPrintObj.transform.position = new Vector3(hit.point.x, 0.54f, hit.point.z);
                }

                await Task.Delay(300);
                // ���b��A���Ղ�j��
                if (footPrintObj != null) Destroy(footPrintObj);
                gameDirector.moveSpeed = 1f;
            }
                if (hitObject != null)hitObject = null;
        }
    }
}
