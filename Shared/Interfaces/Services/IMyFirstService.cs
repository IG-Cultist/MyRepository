// ===================
// My Fitst RPC Service
// ===================
using MagicOnion;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces.Services
{
    public interface IMyFirstService : IService<IMyFirstService>
    {
        /// <summary>
        /// 引数の加算処理
        /// </summary>
        /// <param name="x">足す数</param>
        /// <param name="y">足される数</param>
        /// <returns></returns>
        UnaryResult<int> SumAsync(int x, int y);

        /// <summary>
        /// 引数の減産処理
        /// </summary>
        /// <param name="x">引く数</param>
        /// <param name="y">引かれる数</param>
        /// <returns></returns>
        UnaryResult<int> SubAsync(int x, int y);

        /// <summary>
        /// 引数の配列加算処理
        /// </summary>
        /// <param name="num">加算される配列</param>
        /// <returns></returns>
        UnaryResult<int> SumAllAsync(int[] num);

        /// <summary>
        /// 引数の加算、減算、乗算、除算処理
        /// </summary>
        /// <param name="x">処理する数</param>
        /// <param name="y">処理される数</param>
        /// <returns></returns>
        UnaryResult<int[]> CalcForOperationAsync(int x, int y);

        /// <summary>
        /// 引数(小数)の加算処理
        /// </summary>
        /// <param name="numArray">配列を持つ関数</param>
        /// <returns></returns>
        UnaryResult<float> SumAllNumberAsync(Number numArray);
    }

    /// <summary>
    /// 配列関数
    /// </summary>
    [MessagePackObject]
    public class Number
    {
        [Key(0)]
        public float x;
        [Key(1)]
        public float y;
    }
}
