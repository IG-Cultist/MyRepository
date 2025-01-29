using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using MagicOnion;

namespace Shared.Interfaces.StreamingHubs
{

    /// <summary>
    /// ユーザ入退室通知処理
    /// </summary>
    public interface IRoomHubReceiver
    {
        /// <summary>
        /// プレイヤーアニメーション状態
        /// </summary>
        public enum PlayerState
        {
            /// <summary>
            /// 待機状態
            /// </summary>
            Idle = 0,
            /// <summary>
            /// 移動状態
            /// </summary>
            Move =1,
        }

        /// <summary>
        /// ユーザ入室通知
        /// </summary>
        /// <param name="user">ユーザ</param>
        void OnJoin(JoinedUser user);

        /// <summary>
        /// ユーザ退室通知
        /// </summary>
        /// <param name="connectionID">接続ID</param>
        void OnLeave(Guid connectionID);

        /// <summary>
        /// ユーザ移動通知
        /// </summary>
        /// <param name="connectionID">接続ID</param>
        /// <param name="pos">位置</param>
        /// <param name="rot">向き</param>
        /// <param name="state">アニメーション状態</param>
        void OnMove(Guid connectionID, Vector3 pos, Vector3 rot, PlayerState state);

        /// <summary>
        /// ゲーム開始処理
        /// </summary>
        /// <param name="connectionID">接続ID</param>
        void OnReady(Guid connectionID);

        /// <summary>
        /// ゲーム終了通知
        /// </summary>
        void OnFinish();

        /// <summary>
        /// 攻撃通知
        /// </summary>
        void OnAttack(Guid connectionID, int health);

        /// <summary>
        /// マッチング通知
        /// </summary>
        /// <param name="roomName">部屋名</param>
        /// <param name="userList">参加者</param>
        void OnMatching(string roomName, String[] userList);

        /// <summary>
        /// カウントダウン通知
        /// </summary>
        void OnCount(int time);
        /// <summary>
        /// スキン変更通知
        /// </summary>
        /// <param name="userID">ユーザID</param>
        /// <param name="skinName">スキン名</param>
        void OnChangeSkin(int userID, string skinName);

        /// <summary>
        /// アイテム生成通知
        /// </summary>
        /// <param name="spawnPoint">生成位置番号</param>
        /// <param name="itemNumber">アイテム値</param>
        void OnSpawnItem(int spawnPoint, int itemNumber);

        /// <summary>
        /// アイテム踏みつけ通知
        /// </summary>
        /// <param name="itemName">アイテム名</param>
        void OnStompItem(string itemName);

        /// <summary>
        /// アイテム使用通知
        /// </summary>
        /// <param name="connectionID">ユーザID</param>
        /// <param name="itemName">アイテム名</param>
        void OnUseItem(Guid connectionID, string itemName);
    }
}
