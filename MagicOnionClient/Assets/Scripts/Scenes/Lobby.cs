/// ==============================
/// �Q�[���f�B���N�^�[�X�N���v�g
/// Name:���Y�W�� Update:12/11
/// ==============================
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    // �Q���ҕ\���e�L�X�g
    [SerializeField] Text[] joinedUserName;
    // �ގ��{�^��
    [SerializeField] GameObject exitButton;
    // �������f��
    [SerializeField] RoomHubModel roomModel;
    // ���[�h�p�l��
    [SerializeField] GameObject loadingPanel;
    // ���[�h�A�C�R��
    [SerializeField] GameObject loadingIcon;
    // �w�b�_�[�e�L�X�g
    [SerializeField] Text headerText;
    // �Q�����[�U�̐ڑ�ID�ۑ����X�g
    List<Guid> idList = new List<Guid>();

    // Start is called before the first frame update
    async void Start()
    {
        // ��\���ɂ���
        loadingPanel.SetActive(false);

        // ���[�U�����������Ƃ��Ƀ��\�b�h�����s����悤���f���ɓo�^
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMatchingUser += this.OnMatchingUser;

        // �ڑ�
        await roomModel.ConnectAsync();

        await Task.Delay(300);

        JoinRoom();
    }

    /// <summary>
    /// ���[�U��������
    /// </summary>
    /// <param name="user"></param>
    void OnJoinedUser(JoinedUser user)
    {
        idList.Add(user.ConnectionID);

        int cnt = 0;
        foreach (var id in idList)
        {
            joinedUserName[cnt].text = id.ToString();
            cnt++;
        }
    }

    /// <summary>
    /// ���[�U�ގ�����
    /// </summary>
    /// <param name="connectionID"></param>
    void OnLeavedUser(Guid connectionID)
    {
        for (int i = 0; i < joinedUserName.Length; i++)
        {
            if (joinedUserName[i].text == connectionID.ToString())
            {
                // Player1���������ꍇ
                if (i == 0)
                {
                    // Player2�̖��O��Player1�̏ꏊ�Ɉڂ�
                    joinedUserName[0].text = joinedUserName[1].text;
                    joinedUserName[1].text = "";
                }
                else // ���E�҂̖��O���폜
                {
                    joinedUserName[i].text = "";
                }
                
                idList.Remove(connectionID);
            }
        }
    }

    /// <summary>
    /// �����{�^������
    /// </summary>
    public async void JoinRoom()
    {
        System.Random rand = new System.Random();
        // 1�`10�܂ł̗�������
        int id = rand.Next(1, 4);

        // ����
        await roomModel.JoinLobbyAsync(id);

        exitButton.SetActive(true);
    }

    /// <summary>
    /// �ގ��{�^������
    /// </summary>
    public async void LeaveRoom()
    {
        // �ގ�
        await roomModel.LeaveAsync("Lobby", 1);
        //SceneManager.LoadScene("Title");

        Initiate.DoneFading();
        Initiate.Fade("Title", Color.black, 0.7f);
    }

    async void OnMatchingUser(string roomName)
    {
        // ����f�[�^����
        SendData.roomName = roomName;
        SendData.idList = idList;
        Loading();
        await Task.Delay(800);

        Initiate.DoneFading();
        Initiate.Fade("Game", Color.black, 0.7f);
    }

    /// <summary>
    /// ���[�f�B���O
    /// </summary>
    async void Loading()
    {
        loadingPanel.SetActive(true);
        headerText.text = "���΂炭���҂���������";

        float angle = 8;
        bool rot = true;

        for (int i = 0; i < 80; i++)
        {
            if (loadingIcon != null)
            {
                if (rot)
                {
                    loadingIcon.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.back);
                }
                else
                {
                    loadingIcon.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.forward);
                }
                await Task.Delay(10);
            }
        }
        loadingPanel.SetActive(false);
    }
}
