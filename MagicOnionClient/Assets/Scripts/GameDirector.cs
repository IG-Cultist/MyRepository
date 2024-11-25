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

    // Start is called before the first frame update
    async void Start()
    {
        // ���[�U�����������Ƃ���OnJoinUser���\�b�h�����s����悤���f���ɓo�^
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMovedUser += this.OnMovedUser;
        // �ڑ�
        await roomModel.ConnectAsync();
    }

    void Update()
    {
        MovePlayer();
    }

    public async void JoinRoom()
    {
        int.TryParse(userID.text, out int id);
        // ����
        await roomModel.JoinAsync("sampleRoom", id);
        InvokeRepeating("Move", 0.1f, 0.1f);
    }

    public async void LeaveRoom()
    {
        int.TryParse(userID.text, out int id);
        // �ގ�
        await roomModel.LeaveAsync("sampleRoom", id);
        CancelInvoke("Move");
    }

    public async void Move()
    {
        // �ړ�
        await roomModel.MoveAsync(characterList[roomModel.ConnectionID].transform.position,
            characterList[roomModel.ConnectionID].transform.eulerAngles);
    }

    /// <summary>
    /// ���[�U��������
    /// </summary>
    /// <param name="user"></param>
    void OnJoinedUser(JoinedUser user)
    {
        GameObject characterGameObject = Instantiate(characterPrefabs); //�C���X�^���X����
        characterGameObject.transform.position = new Vector3(-6 + user.UserData.Id, 0, 0);
        characterList[user.ConnectionID] = characterGameObject; // �t�B�[���h�ŕێ�
    }

    /// <summary>
    /// ���[�U�ގ�����
    /// </summary>
    /// <param name="connectionID"></param>
    void OnLeavedUser(Guid connectionID)
    {
        if (connectionID == roomModel.ConnectionID)
        {
            foreach (var character in characterList.Values)
            {
                Destroy(character);
            }
        }
        else Destroy(characterList[connectionID]);

    }

    /// <summary>
    /// ���[�U�ړ�����
    /// </summary>
    /// <param name="pos">�ʒu</param>
    void OnMovedUser(Guid connectionID, Vector3 pos, Vector3 rot)
    {
        characterList[connectionID].transform.DOMove(pos,0.1f);
        characterList[connectionID].transform.DORotate(rot,0.1f);
    }

    void MovePlayer()
    {
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
