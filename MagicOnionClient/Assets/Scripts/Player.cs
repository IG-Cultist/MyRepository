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

    /// <summary>
    /// 攻撃処理
    /// </summary>
    /// <param name="obj"></param>
    async void GetID(GameObject obj)
    {
        // 踏んだ相手の接続IDを取得
        otherConnectionID = obj.transform.parent.GetComponent<Player>().connectionID;

        // 踏んだ相手のレンダラーを取得
        Material material = obj.GetComponent<Renderer>().material;

        // 色を赤くし、少ししたら戻す
        material.color = new Color(127,0,0);
        await Task.Delay(1000);
        material.color = Color.black;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shadow")
        {
            GetID(other.gameObject);
        }
    }
}
