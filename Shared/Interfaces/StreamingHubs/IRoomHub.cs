using System.Threading.Tasks;
using MagicOnion;

namespace Shared.Interfaces.StreamingHubs
{
    /// <summary>
    /// 入退室処理
    /// </summary>
    public interface IRoomHub : IStreamingHub<IRoomHub,IRoomHubReceiver>
    {
        // サーバからクライアントを呼ぶ関数を定義


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
    }
}
