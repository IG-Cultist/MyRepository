﻿using MagicOnion.Server.Hubs;
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
            var joinedUser = new JoinedUser() { ConnectionID = this.ConnectionId, UserData = user };
            var roomData = new RoomData() { JoinedUser = joinedUser };
            roomStrage.Set(this.ConnectionId, roomData);
            roomData.Health = 3;
            // 参加順を設定
            joinedUser.JoinOrder = roomStrage.AllValues.Count;

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

                // 全員準備できたか判定
                bool isReady = true;
                var roomDataList = roomDataStorage.AllValues.ToArray<RoomData>();

                // 参加人数が2人に満たない場合処理しない
                if (roomDataList.Length >= 2)
                {
                    foreach (var data in roomDataList)
                    {
                        // 1人でも準備中の場合開始しない
                        if (data.Ready == false)
                        {
                            isReady = false;
                        }
                    }
                }
                else isReady = false;

                if (isReady == true)
                {
                    // ルーム参加者全員に、ゲーム開始を送信
                    this.Broadcast(room).OnReady();
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
        public async Task<JoinedUser[]> JoinLobbyAsync(int userID)
        {
            string roomName = randomName();

            // 参加中のユーザ情報を返す
            JoinedUser[] joinedUserList = await JoinAsync("Lobby",userID);

            // 参加人数が2人になったらマッチング
            if(joinedUserList.Length == 2)
            {
                // ルーム参加者全員に、マッチング通知を送信
                this.Broadcast(room).OnMatching(roomName);
            }
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

            string[] resultStr = {"",""};
            resultStr[0] = firstName[num] + secondtName[num2];
            resultStr[1] = firstName[num] + secondtName[num2];

            return firstName[num] + secondtName[num2];
        }
    }
}
