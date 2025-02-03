/// ==============================
/// �A�C�e���X�N���v�g
/// Name:���Y�W�� Update:02/03
/// ==============================
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        roomModel = GameObject.Find("RoomHubModel").GetComponent<RoomHubModel>();
        roomModel.OnSpawnItemUser += this.OnSpawnItemUser;
    }

    // Update is called once per frame
    void Update()
    {
        // �����񐔂�9�ȏ�ɂȂ����ꍇ�������~
        if(itemCount >=9) CancelInvoke("SpawnItem");
    }

    void SpawnItem()
    {
        // ������������Z
        itemCount++; 

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
    /// �A�C�e������
    /// </summary>
    /// <param name="spawnPoint">�����ʒu</param>
    /// <param name="itemNumber">�A�C�e���l</param>
    public async void SpawnItem(int spawnPoint, int itemNumber)
    {
        await roomModel.SpawnItemAsync(spawnPoint, itemNumber);
    }

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

        nowItems[spawnPoint] = itemObj;

        // ���O�����
        itemObj.name = itemPrefabs[itemNumber].name;

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
