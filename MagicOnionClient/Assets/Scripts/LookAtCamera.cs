/// ==============================
/// カメラを向くスクリプト
/// Author: Nishiura Kouta
/// ==============================
using UnityEngine;

/// <summary>
/// アタッチしたオブジェクトをカメラに向かせる処理
/// </summary>
public class LookAtCamera : MonoBehaviour
{
    // 見る対象のカメラ
    Camera camera;

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        // カメラがnullでない場合、オブジェクトをカメラに向かせる
        if(camera != null) transform.LookAt(camera.transform);
    }

    /// <summary>
    /// ターゲットカメラ取得処理
    /// </summary>
    /// <param name="playerCam"></param>
    public void GetCamera(Camera playerCam)
    {
        // すでにカメラが設定されていた場合、処理しない
        if (camera != null) return;
        // 取得したカメラを代入
        camera = playerCam;
    }
}
