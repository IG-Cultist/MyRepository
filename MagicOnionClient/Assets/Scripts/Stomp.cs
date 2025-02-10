/// ==============================
/// 画面タッチスクリプト
/// Author: Nishiura Kouta
/// ==============================
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 画面タッチでの処理を呼ぶクラス
/// </summary>
public class Stomp : MonoBehaviour, IPointerDownHandler
{
    /// <summary>
    /// 対応範囲タッチ処理
    /// </summary>
    /// <param name="pointerEventData"></param>
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        // フィールド内オブジェクトからプレイヤースクリプトを取得し、踏み処理を呼ぶ
        GameObject.FindWithTag("Player").GetComponent<Player>().Stomp();
    }
}
