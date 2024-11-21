using MagicOnion;
using MagicOnion.Server;
using Server.Model.Context;
using Shared.Model.Entity;
using Shared.Interfaces.Services;

namespace Server.Services
{
    public class UserService : ServiceBase<IUserService>, IUserService
    {
        /// <summary>
        /// ユーザ登録処理
        /// </summary>
        /// <param name="name">ユーザネーム</param>
        /// <returns></returns>
        public async UnaryResult<int> RegistUserAsync(string name)
        {
            using var context = new GameDBContext();

            // バリデーションチェック
            if(context.Users.Where(user => user.Name == name).Count() > 0)
            { // 取得レコードが複数の場合、エラーを返す
                throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument, "That name is already registered");
            }
            
            // テーブルにレコードを追加
            User user =  new User();
            user.Name = name;
            user.Token = "";
            user.Created_at = DateTime.Now;
            user.Updated_at = DateTime.Now;
            context.Users.Add(user);

            // 変更を保存
            await context.SaveChangesAsync();
            return user.Id;
        }

        /// <summary>
        /// ユーザ一覧処理
        /// </summary>
        /// <param name="name">ユーザネーム</param>
        /// <returns></returns>
        public async UnaryResult<User[]> ShowUserAsync()
        {
            using var context = new GameDBContext();
            User[] user = context.Users.ToArray();

            return user;
        }

        /// <summary>
        /// ユーザ取得処理
        /// </summary>
        /// <param name="user_id">ユーザID</param>
        /// <returns></returns>
        public async UnaryResult<User> GetUserAsync(int user_id)
        {
            using var context = new GameDBContext();

            // ユーザ取得
            User user = context.Users.Where(user => user.Id == user_id).First();
            
            return user;
        }

        /// <summary>
        /// ユーザ情報更新処理
        /// </summary>
        /// <param name="infoArray">ユーザ情報</param>
        /// <returns></returns>
        public async UnaryResult<bool> UpdateUserAsync(UserInfo infoArray)
        {
            using var context = new GameDBContext();

            // バリデーションチェック
            if (context.Users.Where(user => user.Id == infoArray.user_id).Count() != 1)
            { // 取得レコードが複数の場合、エラーを返す
                throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument, "That name doesn't exist");
            }

            // テーブルのレコードを更新
            User user = context.Users.Where(user => user.Id == infoArray.user_id).First();
            user.Name = infoArray.name;
            user.Updated_at = DateTime.Now;

            // 変更を保存
            await context.SaveChangesAsync();
            return true;
        }
    }
}
