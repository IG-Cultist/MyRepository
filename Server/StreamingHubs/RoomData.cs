using Shared.Interfaces.StreamingHubs;
using UnityEngine;

namespace Server.StreamingHubs
{
    /// <summary>
    /// ルーム内に保存するデータクラス
    /// </summary>
    public class RoomData
    {
        /// <summary>
        /// 参加ユーザ
        /// </summary>
        public JoinedUser JoinedUser { get; set; }

        /// <summary>
        /// ユーザの位置
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// ユーザの向き
        /// </summary>
        public Vector3 Rotation { get; set;}

        /// <summary>
        /// ユーザの準備判定
        /// </summary>
        public bool Ready { get; set; }

        /// <summary>
        /// ユーザのゲーム終了判定
        /// </summary>
        public bool Finish {  get; set; }

        /// <summary>
        /// アニメーション状態
        /// </summary>
        public IRoomHubReceiver.PlayerState State { get; set; }

        /// <summary>
        /// 体力
        /// </summary>
        public int Health { get; set; }
    }
}
