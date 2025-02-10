/// ==============================
/// 送信データスクリプト
/// Author: Nishiura Kouta
/// ==============================
using System;
using System.Collections.Generic;

/// <summary>
/// 静的変数管理クラス
/// </summary>
public class SendData
{
    /// <summary>
    /// 部屋名
    /// </summary>
    public static string roomName { get; set; }

    /// <summary>
    /// 接続IDリスト
    /// </summary>
    public static List<Guid> idList { get; set; }

    /// <summary>
    /// スキン名
    /// </summary>
    public static string skinName { get; set; }

    /// <summary>
    /// ユーザID
    /// </summary>
    public static int userID { get; set; }
}
