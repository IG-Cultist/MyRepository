using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Guid connectionID;
    Guid otherConnectionID;
    bool isHit = false;

    string nowItemName = "";

    /// <summary>
    /// 攻撃処理
    /// </summary>
    /// <param name="obj"></param>
    async void GetID(GameObject obj)
    {
        GameDirector gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();

        // 踏んだ相手の接続IDを取得
        otherConnectionID = obj.transform.parent.GetComponent<Player>().connectionID;

        // 踏んだ相手の影のレンダラーを取得
        Material material = obj.GetComponent<Renderer>().material;


        gameDirector.Attack(otherConnectionID);
        // 色を赤くし、少ししたら戻す
        material.color = new Color(127,0,0);
        await Task.Delay(1200);
        material.color = Color.black;
        isHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shadow" && isHit == false)
        {
            GetID(other.gameObject);
            isHit = true;
        }

        if (other.gameObject.tag == "Item")
        {
            nowItemName = other.gameObject.name;
            Destroy(other.gameObject);
        }
    }
}
