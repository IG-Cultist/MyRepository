/// ==============================
/// プレイヤースクリプト
/// Name:西浦晃太 Update:02/03
/// ==============================
using System;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 足跡のプレハブ
    [SerializeField] GameObject footPrintPrefabs;

    // 接続ID
    public Guid connectionID;
    // 自身のものではない接続ID
    Guid otherConnectionID;
    // 被弾判定
    public bool isHit = false;
    // 生成足跡オブジェクト
    GameObject footPrintObj;
    // ゲームディレクタースクリプト
    GameDirector gameDirector;

    // 踏みつけ時SE
    [SerializeField] AudioClip stompSE;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // ゲームディレクタースクリプトをフィールド内オブジェクトから取得
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }

    /// <summary>
    /// 被弾処理
    /// </summary>
    /// <param name="obj"></param>
    public void Damage(GameObject obj)
    {
        // 踏んだ相手の接続IDを取得
        otherConnectionID = obj.transform.parent.parent.GetComponent<Player>().connectionID;

        // 攻撃処理
        gameDirector.Attack(otherConnectionID);

        // 被弾エフェクト処理
        DamageEffect(obj.transform);
    }

    /// <summary>
    /// 踏みつけ処理
    /// </summary>
    public async void Stomp()
    {
        // 自身のカメラを取得
        Camera camera = this.transform.GetChild(0).GetComponent<Camera>();

        // タッチした場所にカメラからレイを発射
        Vector3 worldPosition = camera.ScreenToWorldPoint(Input.mousePosition);

        Ray rayray = camera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(rayray.origin, rayray.direction * 100, Color.red, 4, false);
        RaycastHit hitted = new RaycastHit();

        if (Physics.Raycast(rayray, out hitted, 100, 1 << 6))
        {
            // レイの当たったオブジェクトを取得
            GameObject hitObject = hitted.collider.gameObject;
   
            // オブジェクトが踏みつけ可能エリアの場合
            if (hitObject.name == "StompZone")
            {
                // 踏みつけSEを鳴らす
                audioSource.PlayOneShot(stompSE);
                // カメラを揺らす
                this.transform.GetChild(0).DOShakePosition(0.2f, 0.1f, 20, 15, false, true);
                // 移動速度を低下
                gameDirector.moveSpeed = 0.4f;

                // すでに足跡がある場合それを破壊する
                if (footPrintObj != null) Destroy(footPrintObj);

                // インスタンス生成
                footPrintObj = Instantiate(footPrintPrefabs);

                // カメラからレイを飛ばす
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray, out hit,10, 1 << 6))
                {
                    // 生成位置を設定
                    footPrintObj.transform.position = new Vector3(hit.point.x, 0.64f, hit.point.z);
                }

                // 0.5秒後、足跡を破壊
                await Task.Delay(300);
                if (footPrintObj != null) Destroy(footPrintObj);
                // 移動速度を戻す
                gameDirector.moveSpeed = 1.5f;
            }

            // レイの当たったオブジェクトを初期化
            if (hitObject != null) hitObject = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // トラップを踏んだ際かつ直近でダメージを受けていない場合
        if (collision.gameObject.name == "Trap(active)" && isHit == false)
        { 
            // ダメージ処理をする
            DamageEffect(this.transform);
            gameDirector.Attack(this.GetComponent<Player>().connectionID);
            // トラップを破壊
            Destroy(collision.gameObject);

            // アイテム使用同期処理
            gameDirector.OnUseItemUser(this.GetComponent<Player>().connectionID, collision.gameObject.name);
        }
    }

    /// <summary>
    /// 被弾時処理
    /// </summary>
    /// <param name="renderer"></param>
    async void DamageEffect(Transform transform)
    {
        // 被弾したのが自分である場合
        if (transform.tag != "Shadow_Rival")
        {
            // カメラを揺らす
            transform.GetChild(0).DOShakePosition(0.9f, 1.5f, 45, 15, false, true);
            // 2秒遅延
            await Task.Delay(1200);
           　// 被弾判定をfalseに
            isHit = false;
        }
        else
        {
            // 被弾をわかりやすくするよう透明度をあげる
            transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 100);

            // 2秒遅延
            await Task.Delay(1200);
           　// 被弾判定をfalseに
            isHit = false;
            // 透明度を元に戻す
            transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 220);
        }
    }
}