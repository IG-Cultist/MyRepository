using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) Stomp();
    }

    /// <summary>
    /// 被弾処理
    /// </summary>
    /// <param name="obj"></param>
    public void Damage(GameObject obj)
    {
        // 踏んだ相手の接続IDを取得
        otherConnectionID = obj.transform.parent.parent.GetComponent<Player>().connectionID;

        gameDirector.Attack(otherConnectionID);

        DamageEffect(obj.transform);
    }

    /// <summary>
    /// 踏みつけ処理
    /// </summary>
    async void Stomp()
    {
        Camera camera = this.transform.GetChild(0).GetComponent<Camera>();

        // Shot Ray from Touch Point
        Vector3 worldPosition = camera.ScreenToWorldPoint(Input.mousePosition);

        // 第二引数 レイはどの方向に進むか(zero=指定点)
        bool isHit = Physics.Raycast(camera.transform.position, worldPosition - camera.transform.position, out RaycastHit hitInfo);
        Ray rayray = camera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(rayray.origin, rayray.direction * 100, Color.red, 0.5f, false);

        RaycastHit hitted = new RaycastHit();
        if (Physics.Raycast(rayray, out hitted, 100))
        {
            GameObject hitObject = hitted.collider.gameObject;
            if (hitObject.name == "StompZone")
            {
                audioSource.PlayOneShot(stompSE);
                // カメラを揺らす
                this.transform.GetChild(0).DOShakePosition(0.2f, 0.1f, 20, 15, false, true);

                gameDirector.moveSpeed = 0.4f;
                Debug.Log("name" + hitObject.name);
                // すでに足跡がある場合それを破壊する
                if (footPrintObj != null) Destroy(footPrintObj);

                // インスタンス生成
                footPrintObj = Instantiate(footPrintPrefabs);

                // カメラからレイを飛ばす
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray, out hit))
                {
                    // 生成位置を設定
                    footPrintObj.transform.position = new Vector3(hit.point.x, 0.54f, hit.point.z);
                }

                await Task.Delay(300);
                // 数秒後、足跡を破壊
                if (footPrintObj != null) Destroy(footPrintObj);
                gameDirector.moveSpeed = 1f;
            }
            if (hitObject != null) hitObject = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // トラップを踏んだ際
        if (collision.gameObject.name == "Trap(active)")
        { 
            // ダメージ処理をする
            DamageEffect(this.transform);
            gameDirector.Attack(this.GetComponent<Player>().connectionID);
            // トラップを破壊
            Destroy(collision.gameObject);
        }
    }

    /// <summary>
    /// 被弾時処理
    /// </summary>
    /// <param name="renderer"></param>
    async void DamageEffect(Transform transform)
    {
        // カメラを揺らす
        transform.GetChild(0).DOShakePosition(0.6f, 1.5f, 45, 15, false, true);
     
        //// 対象のレンダラーを取得
        //Renderer renderer = transform.GetChild(2).GetComponent<Renderer>();

        //// 色を赤くし、少ししたら戻す
        //renderer.material.color = new Color(127, 0, 0);
        //await Task.Delay(1200);
        //renderer.material.color = Color.black;
        //isHit = false;
    }
}