using Shared.Interfaces.StreamingHubs;

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
    }
}
