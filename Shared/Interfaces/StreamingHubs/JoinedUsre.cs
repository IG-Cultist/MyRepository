using System;
using MessagePack;
using Shared.Model.Entity;

namespace Shared.Interfaces.StreamingHubs
{ 
    /// <summary>
    /// 参加済みユーザ情報
    /// </summary>
    [MessagePackObject]
    public class JoinedUser
    {
        /// <summary>
        /// 接続ID
        /// </summary>
        [Key(0)]
        public Guid ConnectionID { get; set; }
        /// <summary>
        /// ユーザ情報
        /// </summary>
        [Key(1)]
        public User UserData { get; set; }
        /// <summary>
        /// 参加順
        /// </summary>
        [Key(2)]
        public int JoinOrder { get; set; }
        /// <summary>
        /// スキン名
        /// </summary>
        [Key(3)]
        public string SkinStr { get; set; }
    }
}
