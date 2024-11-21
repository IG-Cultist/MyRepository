using MagicOnion.Server.Hubs;
using Server.Model.Context;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;

namespace Server.StreamingHubs
{
    public class RoomHub : StreamingHubBase<IRoomHub,IRoomHubReceiver>,IRoomHub
    {
        private IGroup room;

        /// <summary>
        /// 参加処理
        /// </summary>
        /// <param name="roomHub">部屋名</param>
        /// <param name="userID">ユーザID</param>
        /// <returns></returns>
        public async Task<JoinedUser[]> JoinAsync(string roomHub, int userID)
        {
            // ルームに参加＆ルームを保持
            this.room = await this.Group.AddAsync(roomHub);

            // DBから指定ユーザ情報を取得
            GameDBContext context = new GameDBContext();
            var user = context.Users.Where(user => user.Id == userID).First();

            // グループストレージにユーザデータを格納
            var roomStrage = this.room.GetInMemoryStorage<RoomData>();
            var joinedUser = new JoinedUser() { ConnectionID = this.ConnectionId ,UserData = user};
            var roomData = new RoomData() { JoinedUser = joinedUser};
            roomStrage.Set(this.ConnectionId, roomData);

            // ルーム参加者全員に、ユーザの通知を送信 (Broadcast(room)で自身も含む)
            this.BroadcastExceptSelf(room).OnJoin(joinedUser);

            RoomData[] roomDataList = roomStrage.AllValues.ToArray<RoomData>();

            // 参加中のユーザ情報を返す
            JoinedUser[] joinedUserList = new JoinedUser[roomDataList.Length];

            for(int i = 0; i < roomDataList.Length; i++)
            {
                joinedUserList[i] = roomDataList[i].JoinedUser;
            }

            return joinedUserList;
        }

        /// <summary>
        /// 離脱処理
        /// </summary>
        /// <param name="roomHub">部屋名</param>
        /// <param name="userID">ユーザID</param>
        /// <returns></returns>
        public async Task LeaveAsync(string roomHub, int userID)
        {
            // グループデータから削除
            this.room.GetInMemoryStorage<RoomData>().Remove(this.ConnectionId);

            // ルーム内のデータから自身を削除
            await room.RemoveAsync(this.Context);

            // ルーム参加者全員に、ユーザの通知を送信 (Broadcast(room)で自身も含む)
            this.BroadcastExceptSelf(room).OnLeave(this.ConnectionId);
        }
    }
}
