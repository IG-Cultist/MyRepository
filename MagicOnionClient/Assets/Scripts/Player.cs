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
    /// �U������
    /// </summary>
    /// <param name="obj"></param>
    async void GetID(GameObject obj)
    {
        GameDirector gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();

        // ���񂾑���̐ڑ�ID���擾
        otherConnectionID = obj.transform.parent.GetComponent<Player>().connectionID;

        // ���񂾑���̉e�̃����_���[���擾
        Material material = obj.GetComponent<Renderer>().material;


        gameDirector.Attack(otherConnectionID);
        // �F��Ԃ����A����������߂�
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
