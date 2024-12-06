/// ==============================
/// ルームハブモデルスクリプト
/// Name:西浦晃太 Update:11/26
/// ==============================
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

    // 接続ID
    public Guid ConnectionID { get; set; }

    // ユーザ接続通知
    public Action<JoinedUser> OnJoinedUser { get; set; }

    // ユーザ退室通知
    public Action<Guid> OnLeavedUser { get; set; }

    // ユーザ移動通知
    public Action<Guid, Vector3, Vector3, IRoomHubReceiver.PlayerState> OnMovedUser { get; set; }

    // ユーザ準備通知
    public Action OnReadyUser {  get; set; }

    //ユーザ終了通知
    public Action OnFinishUser { get; set; }

    //ユーザ攻撃通知
    public Action<Guid, int> OnAttackUser { get; set; }

    /// <summary>
    /// MagicOnion接続処理
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
    /// MagicOnion切断処理
    /// </summary>
    /// <returns></returns>
    public async UniTask DisconnectAsync()
    {
        if (roomHub != null) await roomHub.DisposeAsync();
        if (channel != null) await channel.ShutdownAsync();
        roomHub = null; channel = null;
    }

    /// <summary>
    /// 破棄処理
    /// </summary>
    async void OnDestroy()
    {
        DisconnectAsync();
    }

    /// <summary>
    /// 入室処理
    /// </summary>
    /// <param name="roomName">部屋名</param>
    /// <param name="userID">ユーザID</param>
    /// <returns></returns>
    public async UniTask JoinAsync(string roomName, int userID)
    {
        JoinedUser[] users = await roomHub.JoinAsync(roomName, userID);
        foreach (var user in users)
        {
            if (user.UserData.Id == userID)
            {
                // 自分の接続IDを保存
                this.ConnectionID = user.ConnectionID;
            }
            // モデルを使うクラスに通知
            OnJoinedUser(user);
        }
    }

    /// <summary>
    /// 入室通知
    /// </summary>
    /// <param name="user"></param>
    public void OnJoin(JoinedUser user)
    {
        OnJoinedUser(user);
    }

    /// <summary>
    /// 退室処理
    /// </summary>
    /// <param name="roomName">部屋名</param>
    /// <param name="userID">ユーザID</param>
    /// <returns></returns>
    public async UniTask LeaveAsync(string roomName, int userID)
    {
        await roomHub.LeaveAsync(roomName, userID);

        OnLeavedUser(this.ConnectionID);
    }

    /// <summary>
    /// 退室通知
    /// </summary>
    /// <param name="user"></param>
    public void OnLeave(Guid connectionID)
    {
        OnLeavedUser(connectionID);
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    /// <returns></returns>
    /// <param name="pos">ユーザ位置</param>
    public async UniTask MoveAsync(Vector3 pos, Vector3 rot, IRoomHubReceiver.PlayerState state)
    {
        await roomHub.MoveAsync(pos, rot, state);
        //OnMovedUser(this.ConnectionID, pos, rot);
    }

    /// <summary>
    /// 移動通知
    /// </summary>
    public void OnMove(Guid connectionID, Vector3 pos, Vector3 rot, IRoomHubReceiver.PlayerState state)
    {
        OnMovedUser(connectionID, pos, rot, state);
    }

    /// <summary>
    /// ゲーム開始処理
    /// </summary>
    /// <returns></returns>
    public async UniTask ReadyAsync()
    {
        await roomHub.ReadyAsync();
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void OnReady()
    {
        OnReadyUser();
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    /// <returns></returns>
    public async UniTask FinishAsync()
    {
        await roomHub.FinishAsync();
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void OnFinish()
    {
        OnFinishUser();
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    /// <param name="roomName">部屋名</param>
    /// <param name="userID">ユーザID</param>
    /// <returns></returns>
    public async UniTask AttackAsync(Guid connectionID)
    {
        int health = await roomHub.AttackAsync(connectionID);

        OnAttack(connectionID, health);
    }

    /// <summary>
    /// 攻撃通知
    /// </summary>
    /// <param name="connectionID"></param>
    public void OnAttack(Guid connectionID, int health)
    {
        OnAttackUser(connectionID, health);
    }
}
