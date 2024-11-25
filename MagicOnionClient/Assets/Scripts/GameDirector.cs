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
    // 生成するユーザプレハブ
    [SerializeField] GameObject characterPrefabs;
    // 入力されたユーザID
    [SerializeField] Text userID;
    // 部屋モデル
    [SerializeField] RoomHubModel roomModel;
    // 生成ユーザのディクショナリー
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    // Start is called before the first frame update
    async void Start()
    {
        // ユーザが入室したときにOnJoinUserメソッドを実行するようモデルに登録
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMovedUser += this.OnMovedUser;
        // 接続
        await roomModel.ConnectAsync();
    }

    void Update()
    {
        MovePlayer();
    }

    public async void JoinRoom()
    {
        int.TryParse(userID.text, out int id);
        // 入室
        await roomModel.JoinAsync("sampleRoom", id);
        InvokeRepeating("Move", 0.1f, 0.1f);
    }

    public async void LeaveRoom()
    {
        int.TryParse(userID.text, out int id);
        // 退室
        await roomModel.LeaveAsync("sampleRoom", id);
        CancelInvoke("Move");
    }

    public async void Move()
    {
        // 移動
        await roomModel.MoveAsync(characterList[roomModel.ConnectionID].transform.position,
            characterList[roomModel.ConnectionID].transform.eulerAngles);
    }

    /// <summary>
    /// ユーザ入室処理
    /// </summary>
    /// <param name="user"></param>
    void OnJoinedUser(JoinedUser user)
    {
        GameObject characterGameObject = Instantiate(characterPrefabs); //インスタンス生成
        characterGameObject.transform.position = new Vector3(-6 + user.UserData.Id, 0, 0);
        characterList[user.ConnectionID] = characterGameObject; // フィールドで保持
    }

    /// <summary>
    /// ユーザ退室処理
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
    /// ユーザ移動処理
    /// </summary>
    /// <param name="pos">位置</param>
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
