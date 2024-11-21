using MessagePack;
using System;

namespace Shared.Model.Entity
{
    /// <summary>
    /// ユーザ情報
    /// </summary>
    [MessagePackObject]
    public class User
    {
        /// <summary>
        /// ユーザID
        /// </summary>
        [Key(0)]
        public int Id { get; set; }
        /// <summary>
        /// ユーザ名
        /// </summary>
        [Key(1)] 
        public string Name { get; set; }
       /// <summary>
       /// 認証トークン
       /// </summary>
        [Key(2)] 
        public string Token { get; set; }
        /// <summary>
        /// 生成日時
        /// </summary>
        [Key(3)] 
        public DateTime Created_at { get; set; }
        /// <summary>
        /// 更新日時
        /// </summary>
        [Key(4)] 
        public DateTime Updated_at { get; set; }
    }
}
