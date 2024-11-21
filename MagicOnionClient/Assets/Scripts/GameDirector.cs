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
    [SerializeField] GameObject characterPrefabs;
    [SerializeField] Text userID;
    [SerializeField] RoomHubModel roomModel;
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    // Start is called before the first frame update
    async void Start()
    {
        // ���[�U�����������Ƃ���OnJoinUser���\�b�h�����s����悤���f���ɓo�^
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        // �ڑ�
        await roomModel.ConnectAsync();
    }

    public async void JoinRoom()
    { 
        int.TryParse(userID.text, out int id);
        // ����
        await roomModel.JoinAsync("sampleRoom",id);
    }

    public async void LeaveRoom()
    {
        int.TryParse(userID.text, out int id);
        // �ގ�
        await roomModel.LeaveAsync("sampleRoom", id);
    }

    private void OnJoinedUser(JoinedUser user)
    {
        GameObject characterGameObject = Instantiate(characterPrefabs); //�C���X�^���X����
        characterGameObject.transform.position = new Vector3(-6 + user.UserData.Id, 0, 0);
        characterList[user.ConnectionID] = characterGameObject; // �t�B�[���h�ŕێ�
    }

    private void OnLeavedUser(Guid connectionID)
    {
        if (connectionID == roomModel.ConnectionID)
        {
            foreach (var character in characterList.Values)
            {
                Destroy(character);
            }
        }else Destroy(characterList[connectionID]);

    }
}
