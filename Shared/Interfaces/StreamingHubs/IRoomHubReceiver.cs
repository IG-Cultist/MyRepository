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
        void OnReady();

        /// <summary>
        /// ゲーム終了通知
        /// </summary>
        void OnFinish();

        /// <summary>
        /// 攻撃通知
        /// </summary>
        void OnAttack(Guid connectionID, int health);
    }
}
