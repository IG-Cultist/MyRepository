/// ==============================
/// ヘルススクリプト
/// Author: Nishiura Kouta
/// ==============================
using UnityEngine;

/// <summary>
/// ヘルスオブジェクト生成クラス
/// </summary>
public class Health : MonoBehaviour
{
    // 生成するヘルスのプレハブ
    [SerializeField] GameObject heartPrefabs;

    /// <summary>
    /// 最速処理
    /// </summary>
    void Awake()
    {
        // ヘルスの生成
        SetHP();        
    }

    /// <summary>
    /// ヘルス生成処理
    /// </summary>
    public void SetHP()
    {
        // 3回ループ
        for (int i = 0; i < 3; i++)
        {
            // プレハブからゲームオブジェクトを生成
            GameObject itemObj = Instantiate(heartPrefabs);
            // 名前を訂正し、識別番号をつける
            itemObj.name = "Heart_" + (i +1);
            // 位置を設定
            itemObj.transform.position = new Vector3(5f + (1.5f * i), -16.5f, 10f);

            // 上記の処理をライバルも同様にする
            itemObj = Instantiate(heartPrefabs);
            itemObj.name = "Rival_Heart_" + (i + 1);
            itemObj.transform.position = new Vector3(-8f + (1.5f * i), -16.5f, 10f);
        }
    }
}
