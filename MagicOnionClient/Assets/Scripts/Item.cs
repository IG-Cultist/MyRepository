/// ==============================
/// �A�C�e���X�N���v�g
/// Author: Nishiura Kouta
/// ==============================
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �A�C�e���̏������Ǘ�����N���X
/// </summary>
public class Item : MonoBehaviour
{
    // ��������A�C�e���̃v���n�u���X�g
    [SerializeField] List<GameObject> itemPrefabs;

    // �����|�C���g�I�u�W�F�N�g�̔z��
    [SerializeField] GameObject[] spawnPointObject;

    // ���ݑ��݂��Ă���A�C�e���z��
    GameObject[] nowItems = new GameObject[3];

    // ���ݑ��݂��Ă���A�C�e���z��(�����p)
    GameObject[] nowItems_rival = new GameObject[3];

    // ���삷��v���C���[�̃J����
    public Camera playerCam;

    // �A�C�e����������p�J�E���g
    int itemCount = 0;

    RoomHubModel roomModel;

    /// <summary>
    /// �J�n����
    /// </summary>
    void Start()
    {
        // ���[�����f���X�N���v�g���t�B�[���h���I�u�W�F�N�g����擾
        roomModel = GameObject.Find("RoomHubModel").GetComponent<RoomHubModel>();
        // �ʒm��ݒ�
        roomModel.OnSpawnItemUser += this.OnSpawnItemUser;
    }

    /// <summary>
    /// ���̃I�u�W�F�N�g���j�󂳂ꂽ�ۂ̏���
    /// </summary>
    void OnDestroy()
    {
        // �o�^�����e�ʒm������
        roomModel.OnSpawnItemUser -= this.OnSpawnItemUser;
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    void Update()
    {
        // �����񐔂�9�ȏ�ɂȂ����ꍇ�������~
        if(itemCount >=18) CancelInvoke("SpawnItem");
    }

    /// <summary>
    /// �A�C�e����������
    /// </summary>
    void SpawnItem()
    {
        System.Random rand = new System.Random();

        // �����ʒu�ԍ��̃����_���ݒ�
        int num = rand.Next(0, 3);
        // ��������A�C�e���̔ԍ��̃����_���ݒ�
        int itemNum = rand.Next(0, itemPrefabs.Count);
        // �C���X�^���X����
        GameObject itemObj = Instantiate(itemPrefabs[itemNum]);

        // ���łɃX�|�[���|�C���g�ɃA�C�e�������݂��Ă���ꍇ
        if (nowItems[num] != null)
        {
            // ���̃A�C�e����j��
            Destroy(nowItems[num]);
        }

        // �I�΂ꂽ�X�|�[���|�C���g�̎��A�C�e���ɁA�����������̂�����
        nowItems[num] = itemObj;

        // ���O�����
        itemObj.name = itemPrefabs[itemNum].name + "_" + itemCount;

        // �����ʒu�̃|�W�V��������
        Vector3 pos = spawnPointObject[num].transform.position;
        // �����ʒu��菭����ɃA�C�e���𐶐�
        itemObj.transform.position = new Vector3(pos.x, pos.y +1.0f, pos.z);

        // �e�A�C�e���Ƀv���C���[�̃J������n��
        itemObj.GetComponent<LookAtCamera>().GetCamera(playerCam);

        SpawnItem(num, itemNum);
        
        // ������������Z
        itemCount++; 
    }

    /// <summary>
    ///  �A�C�e�������J�n
    /// </summary>
    public void StartSpawn()
    {
        // 3�b��A1�b��1��A�C�e���𐶐�
        InvokeRepeating("SpawnItem", 3f, 3f);
    }

    /// <summary>
    /// �A�C�e��������������
    /// </summary>
    /// <param name="spawnPoint">�����ʒu</param>
    /// <param name="itemNumber">�A�C�e���l</param>
    public async void SpawnItem(int spawnPoint, int itemNumber)
    {
        await roomModel.SpawnItemAsync(spawnPoint, itemNumber);
    }

    /// <summary>
    /// �A�C�e�������ʒm
    /// </summary>
    /// <param name="spawnPoint">�����ʒu</param>
    /// <param name="itemNumber">�����A�C�e�����ʔԍ�</param>
    void OnSpawnItemUser(int spawnPoint, int itemNumber)
    {
        // �C���X�^���X����
        GameObject itemObj = Instantiate(itemPrefabs[itemNumber]);

        // ���łɃX�|�[���|�C���g�ɃA�C�e�������݂��Ă���ꍇ
        if (nowItems[spawnPoint] != null)
        {
            // ���̃A�C�e����j��
            Destroy(nowItems[spawnPoint]);
        }

        // �I�΂ꂽ�X�|�[���|�C���g�̎��A�C�e���ɁA�����������̂�����
        nowItems[spawnPoint] = itemObj;

        // ���O�����
        itemObj.name = itemPrefabs[itemNumber].name + "_" + itemCount;

        // �����ʒu�̃|�W�V��������
        Vector3 pos = spawnPointObject[spawnPoint].transform.position;
        // �����ʒu��菭����ɃA�C�e���𐶐�
        itemObj.transform.position = new Vector3(pos.x, pos.y + 1.0f, pos.z);

        // �e�A�C�e���Ƀv���C���[�̃J������n��
        itemObj.GetComponent<LookAtCamera>().GetCamera(playerCam);

        // ������������Z
        itemCount++;
    }
}
