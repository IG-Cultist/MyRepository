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
    /// �U������
    /// </summary>
    /// <param name="obj"></param>
    async void GetID(GameObject obj)
    {
        // ���񂾑���̐ڑ�ID���擾
        otherConnectionID = obj.transform.parent.GetComponent<Player>().connectionID;

        // ���񂾑���̃����_���[���擾
        Material material = obj.GetComponent<Renderer>().material;

        // �F��Ԃ����A����������߂�
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
