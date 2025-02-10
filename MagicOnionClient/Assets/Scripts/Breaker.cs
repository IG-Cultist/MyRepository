/// ==============================
/// オブジェクト破壊エリアスクリプト
/// Author: Nishiura Kouta
/// ==============================
using UnityEngine;

/// <summary>
/// オブジェクト破壊管理クラス
/// </summary>
public class Breaker : MonoBehaviour
{
    /// <summary>
    /// トリガー内オブジェクト処理
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // 触れたオブジェクトがヘルスの場合、それを破壊
        if (other.gameObject.tag == "Health")
        {
            Destroy(other.gameObject);
        }
    }
}
