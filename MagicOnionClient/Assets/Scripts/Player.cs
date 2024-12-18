using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject footPrintPrefabs;

    public Guid connectionID;
    Guid otherConnectionID;
    public bool isHit = false;

    GameObject footPrintObj;

    GameDirector gameDirector;
    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) Stomp();
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    /// <param name="obj"></param>
    public async void GetID(GameObject obj)
    {
        // 踏んだ相手の接続IDを取得
        otherConnectionID = obj.transform.parent.GetComponent<Player>().connectionID;

        // 踏んだ相手の影のレンダラーを取得
        Renderer renderer = obj.GetComponent<Renderer>();

        gameDirector.Attack(otherConnectionID);
        // 色を赤くし、少ししたら戻す
        renderer.material.color = new Color(127, 0, 0);
        await Task.Delay(1200);
        renderer.material.color = Color.black;
        isHit = false;
    }

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
                if (hitObject != null)hitObject = null;
        }
    }
}
