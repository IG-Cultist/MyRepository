//using System.Numerics;
using System;
using System.Threading.Tasks;
using MagicOnion;
using UnityEngine;

namespace Shared.Interfaces.StreamingHubs
{
    /// <summary>
    /// 入退室処理
    /// </summary>
    public interface IRoomHub : IStreamingHub<IRoomHub,IRoomHubReceiver>
    {
        /// <summary>
        /// ユーザ入室
        /// </summary>
        /// <param name="roomHub">入室する部屋名</param>
        /// <param name="userID">ユーザID</param>
        /// <returns></returns>
        Task<JoinedUser[]> JoinAsync(string roomHub,int userID);

        /// <summary>
        /// ユーザ退室
        /// </summary>
        /// <param name="roomHub">退室する部屋名</param>
        /// <param name="userID">ユーザID</param>
        /// <returns></returns>
        Task LeaveAsync(string roomHub, int userID);

        /// <summary>
        /// ユーザ移動
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="rot">向き</param>
        /// <returns></returns>
        Task MoveAsync(Vector3 pos, Vector3 rot);

        /// <summary>
        /// ゲーム開始
        /// </summary>
        /// <returns></returns>
        Task ReadyAsync();

        /// <summary>
        /// ゲーム終了
        /// </summary>
        /// <returns></returns>
        Task FinishAsync();
    }
}
