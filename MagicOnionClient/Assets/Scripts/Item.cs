using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // ��������A�C�e���̃v���n�u���X�g
    [SerializeField] List<GameObject> itemPrefabs;

    // ���삷��v���C���[�̃J����
    public Camera playerCam;

    // �A�C�e����������p�J�E���g
    int itemCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        // 3�b��A1�b��1��A�C�e���𐶐�
        InvokeRepeating("SpawnItem", 3f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        // �����񐔂�5�ȏ�ɂȂ����ꍇ�������~
        if(itemCount >= 5) CancelInvoke("SpawnItem");
    }

    void SpawnItem()
    {
        System.Random rand = new System.Random();

        // ��������A�C�e���̔ԍ��̃����_���ݒ�
        int itemNum = rand.Next(0, itemPrefabs.Count);

        // �����ʒu�̃����_���ݒ�
        float x = rand.Next(-10, 10);
        float z = rand.Next(-10, 10);

        // �C���X�^���X����
        GameObject itemObj = Instantiate(itemPrefabs[itemNum]);

        // ���O�����
        itemObj.name = itemPrefabs[itemNum].name;

        // �����ʒu��ݒ�
        itemObj.transform.position = new Vector3(x, 10f, z);

        // �e�A�C�e���Ƀv���C���[�̃J������n��
        itemObj.GetComponent<LookAtCamera>().GetCamera(playerCam);

        // ������������Z
        itemCount++;
    }
}
