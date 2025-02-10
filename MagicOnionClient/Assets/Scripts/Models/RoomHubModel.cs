/// ==============================
/// ���[���n�u���f���X�N���v�g
/// Name:���Y�W�� Update:02/03
/// ==============================
using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomHubModel : BaseModel, IRoomHubReceiver
{
    private GrpcChannel channel;
    private IRoomHub roomHub;

    // �ڑ�ID
    public Guid ConnectionID { get; set; }

    // ���[�U�ڑ��ʒm
    public Action<JoinedUser> OnJoinedUser { get; set; }

    // ���[�U�ގ��ʒm
    public Action<Guid> OnLeavedUser { get; set; }

    // ���[�U�ړ��ʒm
    public Action<Guid, Vector3, Vector3, IRoomHubReceiver.PlayerState> OnMovedUser { get; set; }

    // ���[�U�����ʒm
    public Action<Guid> OnReadyUser { get; set; }

    //���[�U�I���ʒm
    public Action OnFinishUser { get; set; }

    //���[�U�U���ʒm
    public Action<Guid, int> OnAttackUser { get; set; }

    //�}�b�`���O�ʒm
    public Action<string, string[]> OnMatchingUser { get; set; }

    // �J�E���g�_�E���ʒm
    public Action<int> OnCountUser { get; set; }

    // �X�L���ύX�ʒm
    public Action<int, string> OnChangeSkinUser { get; set; }

    // �A�C�e�������ʒm
    public Action<int, int> OnSpawnItemUser { get; set; }

    // �A�C�e�����݂��ʒm
    public Action<string> OnStompItemUser { get; set; }

    // �A�C�e���g�p�ʒm
    public Action<Guid, string> OnUseItemUser { get; set; }
    /// <summary>
    /// MagicOnion�ڑ�����
    /// </summary>
    /// <returns></returns>
    public async UniTask ConnectAsync()
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        channel = GrpcChannel.ForAddress(ServerURL,
            new GrpcChannelOptions() { HttpHandler = handler });
        roomHub = await StreamingHubClient.
            ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this);
    }

    /// <summary>
    /// MagicOnion�ؒf����
    /// </summary>
    /// <returns></returns>
    public async UniTask DisconnectAsync()
    {   
        if (roomHub != null) await roomHub.DisposeAsync();
        if (channel != null) await channel.ShutdownAsync();

        roomHub = null; channel = null;
    }

    /// <summary>
    /// �j������
    /// </summary>
    async void OnDestroy()
    {
        DisconnectAsync();
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="roomName">������</param>
    /// <param name="skinName">�X�L����</param>
    /// <returns></returns>
    public async UniTask JoinAsync(string roomName, string skinName)
    {
        JoinedUser[] users = await roomHub.JoinAsync(roomName, skinName);

        // �����̐ڑ�ID��ۑ�
        this.ConnectionID =  await roomHub.GetConID();

        foreach (var user in users)
        {
            // ���f�����g���N���X�ɒʒm
            OnJoinedUser(user);
        }
    }

    /// <summary>
    /// �����ʒm
    /// </summary>
    /// <param name="user"></param>
    public void OnJoin(JoinedUser user)
    {
        OnJoinedUser?.Invoke(user);
    }

    /// <summary>
    /// �ގ�����
    /// </summary>
    /// <param name="roomName">������</param>
    /// <param name="userID">���[�UID</param>
    /// <returns></returns>
    public async UniTask LeaveAsync(string roomName, int userID)
    {
        await roomHub.LeaveAsync(roomName, userID);

        OnLeavedUser(this.ConnectionID);
    }

    /// <summary>
    /// �ގ��ʒm
    /// </summary>
    /// <param name="user"></param>
    public void OnLeave(Guid connectionID)
    {
        OnLeavedUser?.Invoke(connectionID);
    }

    /// <summary>
    /// �ړ�����
    /// </summary>
    /// <returns></returns>
    /// <param name="pos">���[�U�ʒu</param>
    public async UniTask MoveAsync(Vector3 pos, Vector3 rot, IRoomHubReceiver.PlayerState state)
    {
        await roomHub.MoveAsync(pos, rot, state);
        //OnMovedUser(this.ConnectionID, pos, rot);
    }

    /// <summary>
    /// �ړ��ʒm
    /// </summary>
    public void OnMove(Guid connectionID, Vector3 pos, Vector3 rot, IRoomHubReceiver.PlayerState state)
    {
        OnMovedUser?.Invoke(connectionID, pos, rot, state);
    }

    /// <summary>
    /// �Q�[���J�n����
    /// </summary>
    /// <returns></returns>
    public async UniTask ReadyAsync()
    {
        await roomHub.ReadyAsync();
    }

    /// <summary>
    /// �Q�[���J�n
    /// </summary>
    public void OnReady(Guid connectionID)
    {
        //OnReadyUser?.Invoke(connectionID);
    }

    /// <summary>
    /// �Q�[���I������
    /// </summary>
    /// <returns></returns>
    public async UniTask FinishAsync()
    {
        await roomHub.FinishAsync();
    }

    /// <summary>
    /// �Q�[���I��
    /// </summary>
    public void OnFinish()
    {
        OnFinishUser?.Invoke();
    }

    /// <summary>
    /// �U������
    /// </summary>
    /// <param name="userID">���[�UID</param>
    /// <returns></returns>
    public async UniTask AttackAsync(Guid connectionID)
    {
        int health = await roomHub.AttackAsync(connectionID);

        OnAttack(connectionID, health);
    }

    /// <summary>
    /// �U���ʒm
    /// </summary>
    /// <param name="connectionID"></param>
    public void OnAttack(Guid connectionID, int health)
    {
        OnAttackUser?.Invoke(connectionID, health);
    }

    /// <summary>
    /// ���r�[�Q������
    /// </summary>
    /// <param name="userID">���[�UID</param>
    /// <returns></returns>
    public async UniTask JoinLobbyAsync()
    {
        await roomHub.JoinLobbyAsync();
        await JoinAsync("Lobby", "shadow_noraml");
    }

    /// <summary>
    /// �}�b�`���O�ʒm
    /// </summary>
    /// <param name="roomName"></param>
    public void OnMatching(string roomName, string[] userList)
    {
        OnMatchingUser?.Invoke(roomName, userList);
    }

    /// <summary>
    /// �J�E���g�_�E������
    /// </summary>
    /// <param name="userID">���[�UID</param>
    /// <returns></returns>
    public async UniTask CountTimer(int time)
    {
        await roomHub.CountTimer(time);
    }

    /// <summary>
    /// �J�E���g�_�E���ʒm
    /// </summary>
    /// <param name="roomName"></param>
    public void OnCount(int time)
    {
        OnCountUser?.Invoke(time);
    }

    /// <summary>
    /// �X�L���ύX����
    /// </summary>
    /// <param name="skinName"></param>
    /// <returns></returns>
    public async UniTask ChangeSkinAsync(int userID, string skinName)
    {
        await roomHub.ChangeSkinAsync(userID, skinName);
    }

    /// <summary>
    /// �X�L���K�p�ʒm
    /// </summary>
    /// <param name="skinName"></param>
    /// <returns></returns>
    public void OnChangeSkin(int userID, string skinName)
    {
        OnChangeSkinUser?.Invoke(userID, skinName);
    }

    /// <summary>
    /// �A�C�e����������
    /// </summary>
    /// <param name="spawnPoint">�����ʒu</param>
    /// <param name="itemNumber">�A�C�e���l</param>
    /// <returns></returns>
    public async UniTask SpawnItemAsync(int spawnPoint, int itemNumber)
    {
        await roomHub.SpawnItemAsync(spawnPoint, itemNumber);
    }

    /// <summary>
    /// �A�C�e�������ʒm
    /// </summary>
    /// <param name="spawnPoint">�����ʒu</param>
    /// <param name="itemNumber">�A�C�e���l</param>
    public void OnSpawnItem(int spawnPoint, int itemNumber)
    {
        OnSpawnItemUser?.Invoke(spawnPoint, itemNumber);
    }

    /// <summary>
    /// �A�C�e�����݂�����
    /// </summary>
    /// <param name="itemName">�A�C�e����</param>
    /// <returns></returns>
    public async UniTask StompItemAsync(string itemName)
    {
        await roomHub.StompItemAsync(itemName);
    }

    /// <summary>
    /// �A�C�e�����݂��ʒm
    /// </summary>
    /// <param name="itemName">�A�C�e����</param>
    /// <returns></returns>
    public void OnStompItem(string itemName)
    {
        OnStompItemUser?.Invoke(itemName);
    }

    /// <summary>
    /// �A�C�e���g�p����
    /// </summary>
    /// <param name="connectionID">�ڑ�ID</param>
    /// <param name="itemName">�A�C�e����</param>
    /// <returns></returns>
    public async UniTask UseItemAsync(Guid connectionID, string itemName)
    {
        await roomHub.UseItemAsync(connectionID, itemName);
    }

    /// <summary>
    /// �A�C�e���g�p�ʒm
    /// </summary>
    /// <param name="connectionID"></param>
    /// <param name="itemName"></param>
    public void OnUseItem(Guid connectionID, string itemName)
    {
        OnUseItemUser?.Invoke(connectionID, itemName);
    }
}