using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.StreamingHubs;
using System;
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
    public Action<Guid, Vector3, Vector3> OnMovedUser { get; set; }

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
    /// <param name="userID">���[�UID</param>
    /// <returns></returns>
    public async UniTask JoinAsync(string roomName, int userID)
    {
        JoinedUser[] users = await roomHub.JoinAsync(roomName, userID);
        foreach (var user in users)
        {
            if (user.UserData.Id == userID)
            {
                // �����̐ڑ�ID��ۑ�
                this.ConnectionID = user.ConnectionID;
            }
            //  ���f�����g���N���X�ɒʒm
            OnJoinedUser(user);
        }
    }

    /// <summary>
    /// �����ʒm
    /// </summary>
    /// <param name="user"></param>
    public void OnJoin(JoinedUser user)
    {
        OnJoinedUser(user);
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
        OnLeavedUser(connectionID);
    }

    /// <summary>
    /// �ړ�����
    /// </summary>
    /// <returns></returns>
    /// <param name="pos">���[�U�ʒu</param>
    public async UniTask MoveAsync(Vector3 pos, Vector3 rot)
    {
        await roomHub.MoveAsync(pos, rot);
        //OnMovedUser(this.ConnectionID, pos, rot);
    }

    /// <summary>
    /// �ړ��ʒm
    /// </summary>
    public void OnMove(Guid connectionID, Vector3 pos, Vector3 rot)
    {
        OnMovedUser(connectionID, pos, rot);
    }
}
