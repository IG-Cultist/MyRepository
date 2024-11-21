using System;
using System.Collections.Generic;
using System.Text;
using MagicOnion;

namespace Shared.Interfaces.StreamingHubs
{
    /// <summary>
    /// ユーザ入退室通知処理
    /// </summary>
    public interface IRoomHubReceiver
    {
        // サーバからクライアントを呼ぶ関数を定義

        /// <summary>
        /// ユーザ入室通知
        /// </summary>
        /// <param name="user"></param>
        void OnJoin(JoinedUser user);

        /// <summary>
        /// ユーザ退室通知
        /// </summary>
        /// <param name="connectionID"></param>
        void OnLeave(Guid connectionID);
    }
}
