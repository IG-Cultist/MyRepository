// ===================
// User Manage RPC Service
// ===================
using MagicOnion;
using MessagePack;
using Shared.Model.Entity;

namespace Shared.Interfaces.Services
{
    /// <summary>
    /// ユーザ登録API
    /// </summary>
    public interface IUserService : IService<IUserService>
    {
        /// <summary>
        /// ユーザ登録処理
        /// </summary>
        /// <param name="name">ユーザネーム</param>
        /// <returns></returns>
        UnaryResult<int> RegistUserAsync(string name);

        /// <summary>
        /// ユーザ一覧取得処理
        /// </summary>
        /// <returns></returns>
        UnaryResult<User[]> ShowUserAsync();

        /// <summary>
        /// ユーザ取得処理
        /// </summary>
        /// <param name="user_id">ユーザID</param>
        /// <returns></returns>
        UnaryResult<User> GetUserAsync(int user_id);

        /// <summary>
        /// ユーザ名更新処理
        /// </summary>
        /// <param name="infoArray">ユーザ情報</param>
        /// <returns></returns>
        UnaryResult<bool> UpdateUserAsync(UserInfo infoArray);
    }

    /// <summary>
    /// 配列関数
    /// </summary>
    [MessagePackObject]
    public class UserInfo
    {
        /// <summary>
        /// ユーザID
        /// </summary>
        [Key(0)]
        public int user_id;

        /// <summary>
        /// ユーザ名
        /// </summary>
        [Key(1)]
        public string name;
    }
}
