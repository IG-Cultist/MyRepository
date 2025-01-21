/// ==============================
/// ���r�[�X�N���v�g
/// Name:���Y�W�� Update:01/20
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
    // �����{�^��
    [SerializeField] GameObject readyButton;
    // �������f��
    [SerializeField] RoomHubModel roomModel;
    // ���[�h�p�l��
    [SerializeField] GameObject loadingPanel;
    // �����p�l��
    [SerializeField] GameObject explainPanel;
    // �����摜
    [SerializeField] GameObject[] explainImages;
    // �����摜
    [SerializeField] GameObject[] explainTitles;
    // ���[�h�A�C�R��
    [SerializeField] GameObject loadingIcon;
    // �X�L���ύX�p�l��
    [SerializeField] GameObject skinPanel;
    // �X�L���ύX�{�^��
    [SerializeField] GameObject[] changeButton;
    // �w�b�_�[�e�L�X�g
    [SerializeField] Text headerText;
    // �Q�����[�U�̐ڑ�ID�ۑ����X�g
    List<Guid> idList = new List<Guid>();

    int count = 0;

    // �����摜�p�ϐ�
    int imageCnt = 0;

    // �N���b�Nor�^�b�vSE
    [SerializeField] AudioClip clickSE;

    AudioSource audioSource;

    // Start is called before the first frame update
    async void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // ��\���ɂ���
        loadingPanel.SetActive(false);
        skinPanel.SetActive(false);
        readyButton.SetActive(false);
        explainPanel.SetActive(false);
        // ���[�U�����������Ƃ��Ƀ��\�b�h�����s����悤���f���ɓo�^
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMatchingUser += this.OnMatchingUser;

        // �ڑ�
        await roomModel.ConnectAsync();

        // �����҂�
        await Task.Delay(300);

        // ���r�[����
        JoinRoom();
    }

    void Update()
    {
        // ���N���b�N�������͉�ʃ^�b�v���N���b�NSE���o��
        if (Input.GetMouseButtonUp(0)) audioSource.PlayOneShot(clickSE);
    }

    /// <summary>
    /// ���[�U��������
    /// </summary>
    /// <param name="user"></param>
    void OnJoinedUser(JoinedUser user)
    {
        // �����҂�2���ȏ�̏ꍇ�������Ȃ�
        if (idList.Count >= 2) return;
        // �Q����ID���X�g�ɓ����
        idList.Add(user.ConnectionID);

        int cnt = 0;
        // �\���p�Q���Җ���ID�ɂ���
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
    /// ��������
    /// </summary>
    public async void JoinRoom()
    {
        //System.Random rand = new System.Random();
        // 1�`10�܂ł̗�������
        //int id = rand.Next(1, 4);

        // ����
        await roomModel.JoinLobbyAsync(SendData.userID);

        // �e�{�^����\��
        exitButton.SetActive(true);
        skinPanel.SetActive(true);
        readyButton.SetActive(true);
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

    /// <summary>
    /// ���������{�^������
    /// </summary>
    public async void Ready()
    {
        // �I���X�L������
        string sendStr;
        switch (count)
        {
            case 0:
                sendStr = "shadow_normal";
                break;
            case 1:
                sendStr = "shadow_eye";
                break;
            case 2:
                sendStr = "shadow_face";
                break;
            case 3:
                sendStr = "shadow_mouth";
                break;
            default:
                sendStr = "";
                break;
        }
        // ���M�p�X�L�����ɑ��
        SendData.skinName = sendStr;

        // ������������
        await roomModel.ReadyAsync();
        // ���������{�^�����\����
        readyButton.SetActive(false);

        // �X�L���ύX�{�^�����\����
        changeButton[0].SetActive(false);
        changeButton[1].SetActive(false);
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
        headerText.text = "�܂��Ȃ��J�n...";

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

    /// <summary>
    /// ���̃X�L���\������
    /// </summary>
    public void nextSkin()
    {
        count++;
        if (count >= 4) count = 0;

        // ���\�[�X����A�A�C�R�����擾
        Texture2D texture = Resources.Load("Shadows/shadow_" + count) as Texture2D;

        Image skinPreview =  skinPanel.transform.GetChild(0).GetComponent<Image>();

        skinPreview.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                       Vector2.zero);
    }

    /// <summary>
    /// 1�O�̃X�L���\������
    /// </summary>
    public void backSkin()
    {
        count--;
        if (count < 0) count = 3;

        // ���\�[�X����A�A�C�R�����擾
        Texture2D texture = Resources.Load("Shadows/shadow_" + count) as Texture2D;

        Image skinPreview = skinPanel.transform.GetChild(0).GetComponent<Image>();

        skinPreview.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                       Vector2.zero);
    }

    /// <summary>
    /// �����p�l���\������
    /// </summary>
    public void openExplain()
    {
        explainPanel.SetActive(true);
        explainImages[0].SetActive(true);
        explainImages[1].SetActive(false);
        explainTitles[0].SetActive(true);
        explainTitles[1].SetActive(false);
    }

    /// <summary>
    /// �����p�l����\������
    /// </summary>
    public void closeExplain()
    {
        explainPanel.SetActive(false);
    }

    /// <summary>
    /// ���̐����摜�\������
    /// </summary>
    public void nextImage()
    {
        if (imageCnt >= 1)
        {
            imageCnt = 0;
            explainImages[0].SetActive(true);
            explainImages[1].SetActive(false);
            explainTitles[0].SetActive(true);
            explainTitles[1].SetActive(false);
        }
        else
        {
            imageCnt++;
            explainImages[0].SetActive(false);
            explainImages[1].SetActive(true);
            explainTitles[0].SetActive(false);
            explainTitles[1].SetActive(true);
        }
    }
  
    /// <summary>
    /// ��O�̐����摜�\������
    /// </summary>
    public void backImage()
    {
        if (imageCnt <= 0)
        {
            imageCnt = 1;
            explainImages[0].SetActive(false);
            explainImages[1].SetActive(true);
            explainTitles[0].SetActive(false);
            explainTitles[1].SetActive(true);
        }
        else
        {
            imageCnt--;
            explainImages[0].SetActive(true);
            explainImages[1].SetActive(false);
            explainTitles[0].SetActive(true);
            explainTitles[1].SetActive(false);
        }
    }
}
