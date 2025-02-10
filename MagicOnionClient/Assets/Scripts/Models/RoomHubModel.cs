/// ==============================
/// ルームハブモデルスクリプト
/// Name:西浦晃太 Update:02/03
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

    // 接続ID
    public Guid ConnectionID { get; set; }

    // ユーザ接続通知
    public Action<JoinedUser> OnJoinedUser { get; set; }

    // ユーザ退室通知
    public Action<Guid> OnLeavedUser { get; set; }

    // ユーザ移動通知
    public Action<Guid, Vector3, Vector3, IRoomHubReceiver.PlayerState> OnMovedUser { get; set; }

    // ユーザ準備通知
    public Action<Guid> OnReadyUser { get; set; }

    //ユーザ終了通知
    public Action OnFinishUser { get; set; }

    //ユーザ攻撃通知
    public Action<Guid, int> OnAttackUser { get; set; }

    //マッチング通知
    public Action<string, string[]> OnMatchingUser { get; set; }

    // カウントダウン通知
    public Action<int> OnCountUser { get; set; }

    // スキン変更通知
    public Action<int, string> OnChangeSkinUser { get; set; }

    // アイテム生成通知
    public Action<int, int> OnSpawnItemUser { get; set; }

    // アイテム踏みつけ通知
    public Action<string> OnStompItemUser { get; set; }

    // アイテム使用通知
    public Action<Guid, string> OnUseItemUser { get; set; }
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
    /// <param name="skinName">スキン名</param>
    /// <returns></returns>
    public async UniTask JoinAsync(string roomName, string skinName)
    {
        JoinedUser[] users = await roomHub.JoinAsync(roomName, skinName);

        // 自分の接続IDを保存
        this.ConnectionID =  await roomHub.GetConID();

        foreach (var user in users)
        {
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
        OnJoinedUser?.Invoke(user);
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
        OnLeavedUser?.Invoke(connectionID);
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
        OnMovedUser?.Invoke(connectionID, pos, rot, state);
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
    public void OnReady(Guid connectionID)
    {
        //OnReadyUser?.Invoke(connectionID);
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
        OnFinishUser?.Invoke();
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
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
        OnAttackUser?.Invoke(connectionID, health);
    }

    /// <summary>
    /// ロビー参加処理
    /// </summary>
    /// <param name="userID">ユーザID</param>
    /// <returns></returns>
    public async UniTask JoinLobbyAsync()
    {
        await roomHub.JoinLobbyAsync();
        await JoinAsync("Lobby", "shadow_noraml");
    }

    /// <summary>
    /// マッチング通知
    /// </summary>
    /// <param name="roomName"></param>
    public void OnMatching(string roomName, string[] userList)
    {
        OnMatchingUser?.Invoke(roomName, userList);
    }

    /// <summary>
    /// カウントダウン処理
    /// </summary>
    /// <param name="userID">ユーザID</param>
    /// <returns></returns>
    public async UniTask CountTimer(int time)
    {
        await roomHub.CountTimer(time);
    }

    /// <summary>
    /// カウントダウン通知
    /// </summary>
    /// <param name="roomName"></param>
    public void OnCount(int time)
    {
        OnCountUser?.Invoke(time);
    }

    /// <summary>
    /// スキン変更処理
    /// </summary>
    /// <param name="skinName"></param>
    /// <returns></returns>
    public async UniTask ChangeSkinAsync(int userID, string skinName)
    {
        await roomHub.ChangeSkinAsync(userID, skinName);
    }

    /// <summary>
    /// スキン適用通知
    /// </summary>
    /// <param name="skinName"></param>
    /// <returns></returns>
    public void OnChangeSkin(int userID, string skinName)
    {
        OnChangeSkinUser?.Invoke(userID, skinName);
    }

    /// <summary>
    /// アイテム生成処理
    /// </summary>
    /// <param name="spawnPoint">生成位置</param>
    /// <param name="itemNumber">アイテム値</param>
    /// <returns></returns>
    public async UniTask SpawnItemAsync(int spawnPoint, int itemNumber)
    {
        await roomHub.SpawnItemAsync(spawnPoint, itemNumber);
    }

    /// <summary>
    /// アイテム生成通知
    /// </summary>
    /// <param name="spawnPoint">生成位置</param>
    /// <param name="itemNumber">アイテム値</param>
    public void OnSpawnItem(int spawnPoint, int itemNumber)
    {
        OnSpawnItemUser?.Invoke(spawnPoint, itemNumber);
    }

    /// <summary>
    /// アイテム踏みつけ処理
    /// </summary>
    /// <param name="itemName">アイテム名</param>
    /// <returns></returns>
    public async UniTask StompItemAsync(string itemName)
    {
        await roomHub.StompItemAsync(itemName);
    }

    /// <summary>
    /// アイテム踏みつけ通知
    /// </summary>
    /// <param name="itemName">アイテム名</param>
    /// <returns></returns>
    public void OnStompItem(string itemName)
    {
        OnStompItemUser?.Invoke(itemName);
    }

    /// <summary>
    /// アイテム使用処理
    /// </summary>
    /// <param name="connectionID">接続ID</param>
    /// <param name="itemName">アイテム名</param>
    /// <returns></returns>
    public async UniTask UseItemAsync(Guid connectionID, string itemName)
    {
        await roomHub.UseItemAsync(connectionID, itemName);
    }

    /// <summary>
    /// アイテム使用通知
    /// </summary>
    /// <param name="connectionID"></param>
    /// <param name="itemName"></param>
    public void OnUseItem(Guid connectionID, string itemName)
    {
        OnUseItemUser?.Invoke(connectionID, itemName);
    }
}