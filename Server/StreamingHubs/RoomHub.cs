using MagicOnion.Server.Hubs;
using Server.Model.Context;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using UnityEngine;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Server.StreamingHubs
{
    public class RoomHub : StreamingHubBase<IRoomHub, IRoomHubReceiver>, IRoomHub
    {
        private IGroup room;

        protected override ValueTask OnDisconnected()
        {
            if (room != null && this.room.GetInMemoryStorage<RoomData>() != null)
            {
                // ルームデータを削除
                this.room.GetInMemoryStorage<RoomData>().Remove(this.ConnectionId);
                // 退室を全メンバーに通知
                this.Broadcast(room).OnLeave(this.ConnectionId);
                // ルーム内のメンバから削除
                room.RemoveAsync(this.Context);

            }
            return CompletedTask;
        }

        /// <summary>
        /// 参加処理
        /// </summary>
        /// <param name="roomHub">部屋名</param>
        /// <param name="userID">ユーザID</param>
        /// <param name="skinName">スキン名</param> 
        /// <returns></returns>
        public async Task<JoinedUser[]> JoinAsync(string roomHub, string skinName)
        {
            // ルームに参加＆ルームを保持
            this.room = await this.Group.AddAsync(roomHub);

            // DBから指定ユーザ情報を取得
            GameDBContext context = new GameDBContext();
            //var user = context.Users.Where(user => user.Id == userID).First();

            // グループストレージにユーザデータを格納
            var roomStrage = this.room.GetInMemoryStorage<RoomData>();
            var joinedUser = new JoinedUser() { ConnectionID = this.ConnectionId/*, UserData = user */};

            var roomData = new RoomData() { JoinedUser = joinedUser };
            roomStrage.Set(this.ConnectionId, roomData);
            roomData.Health = 3;
            roomData.ConnectionID = this.ConnectionId;
            // 参加順を設定
            joinedUser.JoinOrder = roomStrage.AllValues.Count;
            // 参加者のスキン名を設定
            joinedUser.SkinName = skinName;

            // ルーム参加者全員に、ユーザの通知を送信 (Broadcast(room)で自身も含む)
            this.BroadcastExceptSelf(room).OnJoin(joinedUser);

            RoomData[] roomDataList = roomStrage.AllValues.ToArray<RoomData>();

            // 参加中のユーザ情報を返す
            JoinedUser[] joinedUserList = new JoinedUser[roomDataList.Length];

            for (int i = 0; i < roomDataList.Length; i++)
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

            // ルーム参加者全員に、ユーザの通知を送信
            this.BroadcastExceptSelf(room).OnLeave(this.ConnectionId);

            // ルーム内のデータから自身を削除
            await room.RemoveAsync(this.Context);
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="rot">向き</param>
        /// <returns></returns>
        public async Task MoveAsync(Vector3 pos, Vector3 rot, IRoomHubReceiver.PlayerState state)
        {
            var roomDataStorage = this.room.GetInMemoryStorage<RoomData>();
            var roomData = roomDataStorage.Get(this.ConnectionId);
            roomData.Position = pos;
            roomData.Rotation = rot;
            roomData.State = state;
            // ルーム参加者全員に、ユーザの通知を送信
            this.BroadcastExceptSelf(room).OnMove(this.ConnectionId, pos, rot, state);
        }

        /// <summary>
        /// ゲーム開始処理
        /// </summary>
        /// <param name="skinName">スキン名</param> 
        /// <returns></returns>
        public async Task ReadyAsync()
        {
            // 準備完了状態を自身のroomDataに保存
            var roomDataStorage = this.room.GetInMemoryStorage<RoomData>();

            // 排他制御
            lock (roomDataStorage) // 整合性を保つため1人にのみ処理させる (これなしで処理をすると通知が重複しておかしくなる)
            {
                var roomData = roomDataStorage.Get(this.ConnectionId);
                roomData.Ready = true;
                // ルーム参加者全員に、準備完了通知を送信
                this.Broadcast(room).OnReady(this.ConnectionId);

                string[] readyUser =  new string[2];

                readyUser[0] = this.ConnectionId.ToString();
                // 全員準備できたか判定
                var roomDataList = roomDataStorage.AllValues.ToArray<RoomData>();

                if(roomDataList.Length < 2 ) return;

                // 参加者から準備の出来ているユーザを取得
                foreach (var data in roomDataList)
                {
                    if (data.Ready == true && data.ConnectionID != this.ConnectionId)
                    {
                        // 該当ユーザと自身でマッチング
                        readyUser[1] = data.ConnectionID.ToString();
                        // ルーム参加者全員に、マッチング通知を送信
                        this.Broadcast(room).OnMatching(randomName(), readyUser);
                    }
                }
            }
        }

        /// <summary>
        /// ゲーム終了処理
        /// </summary>
        /// <returns></returns>
        public async Task FinishAsync()
        {
            // 準備完了状態を自身のroomDataに保存
            var roomDataStorage = this.room.GetInMemoryStorage<RoomData>();

            // 排他制御
            lock (roomDataStorage)
            {
                var roomData = roomDataStorage.Get(this.ConnectionId);
                roomData.Finish = true;

                // 全員準備できたか判定
                bool isFinish = true;
                var roomDataList = roomDataStorage.AllValues.ToArray<RoomData>();

                foreach (var data in roomDataList)
                {
                    // 1人でも未クリアの場合開始しない
                    if (data.Finish == false)
                    {
                        isFinish = false;
                    }
                }

                if (isFinish == true)
                {
                    // ルーム参加者全員に、ゲーム終了を送信
                    this.Broadcast(room).OnFinish();
                }
            }
        }

        /// <summary>
        /// 攻撃通知処理
        /// </summary>
        /// <param name="userID">ユーザID</param>
        /// <returns></returns>
        public async Task<int> AttackAsync(Guid connectionID)
        {
            // 受け取ったIDのHPを-1
            var roomDataStorage = this.room.GetInMemoryStorage<RoomData>();
            var roomData = roomDataStorage.Get(this.ConnectionId);

            if (roomData.Health > 0)
            {
                roomData.Health--;
            }
            else
            {
                // 終了処理
                var roomDataList = roomDataStorage.AllValues.ToArray<RoomData>();
                foreach (var data in roomDataList)
                {
                    data.Finish = true;
                }
            } 
            
            // ルーム参加者全員に、ユーザの通知を送信
            this.Broadcast(room).OnAttack(connectionID, roomData.Health);

            return roomData.Health;
        }

        /// <summary>
        /// ロビー入室処理
        /// </summary>
        /// <returns></returns>
        public async Task<JoinedUser[]> JoinLobbyAsync()
        {
            // 参加中のユーザ情報を返す
            JoinedUser[] joinedUserList = await JoinAsync("Lobby", "shadow_noraml");

            return joinedUserList;
        }

        /// <summary>
        /// カウントダウン処理
        /// </summary>
        /// <returns></returns>
        public async Task CountTimer(int time)
        {
            // ルーム参加者全員に、残り時間を送信
            this.Broadcast(room).OnCount(time);
        }

        /// <summary>
        /// スキン変更処理
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="skinName"></param>
        /// <returns></returns>
        public async Task ChangeSkinAsync(int userID, string skinName)
        {
            // ルーム参加者全員に、ユーザのスキンを送信
            this.Broadcast(room).OnChangeSkin(userID, skinName);
        }


        /// <summary>
        /// アイテム生成処理
        /// </summary>
        /// <param name="spawnPoint">生成位置番号</param>
        /// <param name="itemNumber">アイテム値</param>
        /// <returns></returns>
        public async Task SpawnItemAsync(int spawnPoint, int itemNumber)
        {
            // ルーム参加者全員に、アイテムが生成されたことを通知
            this.BroadcastExceptSelf(room).OnSpawnItem(spawnPoint, itemNumber);
        }

        /// <summary>
        /// アイテム踏みつけ処理
        /// </summary>
        /// <param name="itemName">アイテム名</param>
        /// <returns></returns>
        public async Task StompItemAsync(string itemName)
        {
            // ルーム参加者全員に、対象アイテムが踏まれたことを通知
            this.Broadcast(room).OnStompItem(itemName);
        }


        /// <summary>
        /// アイテム使用処理
        /// </summary>
        /// <param name="connectionID">ユーザID</param>
        /// <param name="itemName">アイテム名</param>
        /// <returns></returns>
        public async Task UseItemAsync(Guid connectionID, string itemName)
        {
            // ルーム参加者全員に、対象がアイテムを使用したことを通知
            this.BroadcastExceptSelf(room).OnUseItem(connectionID, itemName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Guid> GetConID()
        {
            return this.ConnectionId;
        }

        /// <summary>
        /// ランダムネーム出力処理
        /// </summary>
        /// <returns></returns>
        public string randomName()
        {
            System.Random rand = new System.Random();
            // ファストネーム定義
            string[] firstName = new string[]{
            "Nice","Great","Delicious","Crazy","Hottest",
            "Mrs","Master","Huge","Tiny","Clever",
            "Wetty","Pretty","Golden","Giga","Godly",
            "Cool","Burning","Creepy","Fishy","Metallic",
            "Oriental","Rock&","Mudly","More","Strong",
            "Shiny","Sparkle","Legal","Crazy","Dancing"
        };
            // セカンドネーム定義
            string[] secondtName = new string[]{
            "Cake","Stone","Slime","Clover","Animal",
            "Fish","Earth","Throat","City","Chad",
            "Ghost","Hotel","Knight","Candy","Worm",
            "Tree","Dice","Baby","Machine","Dog",
            "Room","Bird","Cat","Water","Pizza",
            "Skelton","Boots","Game","Gun","Data"
        };
            // 1～30までの乱数を代入
            int num = rand.Next(1, 30);
            int num2 = rand.Next(1, 30);
            Guid num3 = Guid.NewGuid();

            string resultStr;
            resultStr = firstName[num] + secondtName[num2] + "_" + num3;

            return resultStr;
        }
    }
}
