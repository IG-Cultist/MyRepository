using DG.Tweening;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    // �������郆�[�U�v���n�u
    [SerializeField] GameObject characterPrefabs;
    // ���͂��ꂽ���[�UID
    [SerializeField] Text userID;
    // �������f��
    [SerializeField] RoomHubModel roomModel;
    // �������[�U�̃f�B�N�V���i���[
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();
    // �J�E���g�_�E���\���p�l��
    [SerializeField] GameObject countPanel;
    // �J�E���g�_�E���e�L�X�g
    [SerializeField] Text countText;
    // ���������{�^��
    [SerializeField] GameObject readyButton;
    // �Q�[���J�n����ϐ�
    bool isStart = false;

    // Start is called before the first frame update
    async void Start()
    {
        // ��\���ɂ���
        countPanel.SetActive(false);
        readyButton.SetActive(false);

        // ���[�U�����������Ƃ��Ƀ��\�b�h�����s����悤���f���ɓo�^
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMovedUser += this.OnMovedUser;
        roomModel.OnReadyUser += this.OnReadyUser;
        // �ڑ�
        await roomModel.ConnectAsync();
    }

    void Update()
    {
        MovePlayer();
    }

    /// <summary>
    /// ���[�U��������
    /// </summary>
    /// <param name="user"></param>
    void OnJoinedUser(JoinedUser user)
    {
        GameObject characterGameObject = Instantiate(characterPrefabs); //�C���X�^���X����
        // �����ʒu��ݒ�
        characterGameObject.transform.position = new Vector3(-6 + user.UserData.Id, 0, 0);
        characterList[user.ConnectionID] = characterGameObject; // �t�B�[���h�ŕێ�
        readyButton.SetActive(true);
    }

    /// <summary>
    /// ���[�U�ގ�����
    /// </summary>
    /// <param name="connectionID"></param>
    void OnLeavedUser(Guid connectionID)
    {
        // �󂯎�����ڑ�ID�Ǝ��M�̐ڑ�ID����v���Ă���ꍇ
        if (connectionID == roomModel.ConnectionID)
        {  
            // ���������{�^�����\��
            readyButton.SetActive(false);
            // �\������Ă��邷�ׂẴI�u�W�F�N�g��j��
            foreach (var character in characterList.Values)
            {
                Destroy(character);
            }
        }
        // �ގ����[�U�̃I�u�W�F�N�g�̂ݔj��
        else Destroy(characterList[connectionID]);

    }

    /// <summary>
    /// ���[�U�ړ�����
    /// </summary>
    /// <param name="pos">�ʒu</param>
    void OnMovedUser(Guid connectionID, Vector3 pos, Vector3 rot)
    {
        // �e�g�����X�t�H�[�����A�j���[�V����
        characterList[connectionID].transform.DOLocalMove(pos,0.1f).SetEase(Ease.Linear);
        characterList[connectionID].transform.DOLocalRotate(rot,0.1f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// ���[�U��������
    /// </summary>
    void OnReadyUser()
    {
        StartGame();
    }

    /// <summary>
    /// �Q�[���J�n����
    /// </summary>
    void StartGame()
    {
        readyButton.SetActive(false);
        countPanel.SetActive(true);
        float countdownSeconds = 4;

        while (countdownSeconds > 0)
        {
            countdownSeconds -= Time.deltaTime;
            var span = new TimeSpan(0, 0, (int)countdownSeconds);
            countText.text = span.ToString(@"mm\:ss");
        }
        isStart = true;
    }

    /// <summary>
    /// �����{�^������
    /// </summary>
    public async void JoinRoom()
    {
        int.TryParse(userID.text, out int id);
        // ����
        await roomModel.JoinAsync("sampleRoom", id);
        InvokeRepeating("Move", 0.1f, 0.1f);
    }

    /// <summary>
    /// �ގ��{�^������
    /// </summary>
    public async void LeaveRoom()
    { 
        CancelInvoke("Move");
        int.TryParse(userID.text, out int id);
        // �ގ�
        await roomModel.LeaveAsync("sampleRoom", id);
    }

    /// <summary>
    /// �ړ��L�[����
    /// </summary>
    public async void Move()
    {
        // �ړ�
        await roomModel.MoveAsync(characterList[roomModel.ConnectionID].transform.position,
            characterList[roomModel.ConnectionID].transform.eulerAngles);
    }

    /// <summary>
    /// ���������{�^������
    /// </summary>
    public async void Ready()
    {
        await roomModel.ReadyAsync();
    }

    /// <summary>
    /// �L�[���͈ړ�����
    /// </summary>
    void MovePlayer()
    {
        if (isStart != true) return;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            characterList[roomModel.ConnectionID].transform.position += new Vector3(0.1f, 0f, 0f);
            characterList[roomModel.ConnectionID].transform.eulerAngles += new Vector3(0f, 5f, 0f);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            characterList[roomModel.ConnectionID].transform.position -= new Vector3(0.1f, 0f, 0f);
            characterList[roomModel.ConnectionID].transform.eulerAngles -= new Vector3(0f, 5f, 0f);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            characterList[roomModel.ConnectionID].transform.position += new Vector3(0f, 0f, 0.1f);
            characterList[roomModel.ConnectionID].transform.eulerAngles += new Vector3(0f, 5f, 0f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            characterList[roomModel.ConnectionID].transform.position -= new Vector3(0f, 0f, 0.1f);
            characterList[roomModel.ConnectionID].transform.eulerAngles -= new Vector3(0f, 5f, 0f);
        }
    }
}
