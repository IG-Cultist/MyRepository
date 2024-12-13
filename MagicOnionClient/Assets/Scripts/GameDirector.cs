/// ==============================
/// �Q�[���f�B���N�^�[�X�N���v�g
/// Name:���Y�W�� Update:12/11
/// ==============================
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;

public class GameDirector : MonoBehaviour
{
    // �������郆�[�U�v���n�u
    [SerializeField] GameObject characterPrefabs;
    // �ގ��{�^��
    [SerializeField] GameObject exitButton;
    // ���͂��ꂽ���[�UID
    [SerializeField] Text userID;
    // �������f��
    [SerializeField] RoomHubModel roomModel;
    // �������[�U�̃f�B�N�V���i���[
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();
    // �J�E���g�_�E���\���p�l��
    [SerializeField] GameObject countPanel;
    // �J�E���g�_�E���e�L�X�g
    [SerializeField] Text countText;

    List<GameObject> skinList = new List<GameObject>();
    int skinNum = 0;

    GameObject myCamera;

    Player playerScript;
    // �Q�[���J�n����ϐ�
    bool isStart = false;

    bool isFinish = false;

    float moveSpeed = 1f;

    // Start is called before the first frame update
    async void Start()
    {
#if UNITY_EDITOR
        //�G�f�B�^�[���s��
        SendData.roomName = "RoomRoom";
#endif

        if (Input.GetKey(KeyCode.Escape))
        {//ESC�������ۂ̏���
#if UNITY_EDITOR
            //�G�f�B�^�[���s��
            UnityEditor.EditorApplication.isPlaying = false;
#else
            //�r���h��
            Application.Quit();
#endif
        }

        // ��\���ɂ���
        exitButton.SetActive(false);
        countPanel.SetActive(false);

        // ���[�U�����������Ƃ��Ƀ��\�b�h�����s����悤���f���ɓo�^
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMovedUser += this.OnMovedUser;
        roomModel.OnReadyUser += this.OnReadyUser;
        roomModel.OnFinishUser += this.OnFinishUser;
        roomModel.OnAttackUser += this.OnAttackUser;
        // �ڑ�
        await roomModel.ConnectAsync();

        await Task.Delay(600);

        JoinRoom();
    }

    void Update()
    {
        MovePlayer();

        // �Q�[���I����Ԃ���ʃ^�b�v�������ꍇ
        if (isFinish == true && Input.GetMouseButtonDown(0))
        {
            LeaveRoom();
            SceneManager.LoadScene("Result");
        }

        // �Q�[���I����Ԃ���ʃ^�b�v�������ꍇ
        if (Input.GetKeyDown(KeyCode.L))
        {
            isStart = true;
        }

        // �Q�[���I����Ԃ���ʃ^�b�v�������ꍇ
        if (Input.GetKeyDown(KeyCode.C))
        {
            SpawnItem();
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            ChangeSkin();
        }

        if (myCamera != null)
        {
            LookUp();
        }
    }

    /// <summary>
    /// ���[�U��������
    /// </summary>
    /// <param name="user"></param>
    void OnJoinedUser(JoinedUser user)
    {  
        // �C���X�^���X����
        GameObject characterGameObject = Instantiate(characterPrefabs); 

        // �v���C���[�X�N���v�g�擾
        playerScript =  characterGameObject.GetComponent<Player>();

        int count;
        if (characterList.Count <= 0)
        {
            count =1;
        }
        else count = 3;

        // �����ʒu��ݒ�
        characterGameObject.transform.position = new Vector3(-7.5f + (2 * count), 2f, -10f);

        // �����ȊO�̃J�������A�N�e�B�u�ɂ���
        if(roomModel.ConnectionID != user.ConnectionID){
            // �Ώۂ̂̃J�������擾
            GameObject obj = characterGameObject.transform.GetChild(0).gameObject;
            // �J�������A�N�e�B�u��
            obj.SetActive(false);
        }
        else if (roomModel.ConnectionID == user.ConnectionID)
        {
            // ���g�̃J�������擾
            GameObject obj = characterGameObject.transform.GetChild(0).gameObject;
            myCamera = obj;
            // �J�����̃R���|�l���g���擾
            Camera camera = obj.GetComponent<Camera>();
            // �J�����̃R���|�l���g����f�[�^���擾
            var cameraData = camera.GetUniversalAdditionalCameraData();
            // �V�[�����ɂ���w���X�J�������擾
            GameObject stackObj = GameObject.Find("HealthCamera");
            // �n�[�g�J�����̃R���|�l���g���擾
            Camera stackCamera = stackObj.GetComponent<Camera>();
            // ���g�̃J�����Ƀn�[�g�J�������X�^�b�N������
            cameraData.cameraStack.Add(stackCamera);
        }

        Transform children = characterGameObject.GetComponentInChildren<Transform>();

        foreach (Transform ob in children)
        {
            GameObject obj;
            switch (ob.name)
            {
                case "shadow":
                    obj = GameObject.Find (ob.name);
                    obj.SetActive(true);
                    skinList.Add(obj);
                    break;
                case "shadow_face":
                    obj = GameObject.Find(ob.name);
                    obj.SetActive(false);
                    skinList.Add(obj);
                    break;
                case "shadow_eye":
                    obj = GameObject.Find(ob.name);
                    obj.SetActive(false);
                    skinList.Add (obj);
                    break;
                default:
                    break;
            }
        }

        // ���ʔԍ����e�q�I�u�W�F�N�g�̖��O�ɕt����
        foreach (Transform obj in characterGameObject.transform)
        {
            obj.name += "_" + user.UserData.Id;
        }

        characterGameObject.name = user.UserData.Id.ToString();

        characterList[user.ConnectionID] = characterGameObject; // �t�B�[���h�ŕێ�
        playerScript.connectionID = user.ConnectionID;
    }

    /// <summary>
    /// ���[�U�ގ�����
    /// </summary>
    /// <param name="connectionID"></param>
    void OnLeavedUser(Guid connectionID)
    {
        // �󂯎�����ڑ�ID�Ǝ��g�̐ڑ�ID����v���Ă���ꍇ
        if (connectionID == roomModel.ConnectionID)
        {
            // �\������Ă��邷�ׂẴI�u�W�F�N�g��j��
            foreach (var character in characterList.Values)
            {
                Destroy(character);
            }
        }
        // �ގ����[�U�̃I�u�W�F�N�g�̂ݔj��
        else Destroy(characterList[connectionID]);
    }

    /// <summary>
    /// ���[�U�ړ�����
    /// </summary>
    /// <param name="pos">�ʒu</param>
    void OnMovedUser(Guid connectionID, Vector3 pos, Vector3 rot, IRoomHubReceiver.PlayerState state)
    {
        // �e�g�����X�t�H�[�����A�j���[�V����
        characterList[connectionID].transform.DOLocalMove(pos,0.1f).SetEase(Ease.Linear);
        characterList[connectionID].transform.DOLocalRotate(rot,0.1f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// ���[�U��������
    /// </summary>
    void OnReadyUser()
    {
        StartGame();
    }

    /// <summary>
    /// ���[�U�Q�[���I������
    /// </summary>
    void OnFinishUser()
    {
        FinishGame();
    }

    /// <summary>
    /// �Q�[���J�n����
    /// </summary>
    void StartGame()
    {
        countPanel.SetActive(true);
        float limitTime = 4;

        while (limitTime > 0)
        {
            limitTime -= Time.deltaTime;

            if (limitTime < 0)
            {
                limitTime = 0;
            }

            countText.text = limitTime.ToString("F0"); // �c�莞�Ԃ𐮐��ŕ\��
        }
        countText.text = "Start";
        isStart = true;
    }

    /// <summary>
    /// �Q�[���I������
    /// </summary>
    void FinishGame()
    {
        exitButton.SetActive(false);
        countText.text = "�N���b�N�őގ�";
        isFinish = true;
    }

    public async void JoinRoom()
    {
        System.Random rand = new System.Random();
        int id = rand.Next(1, 4);

        //ConnectPanel.SetActive(false);
        //int.TryParse(userID.text, out int id);

        await roomModel.JoinAsync(SendData.roomName, id);
        InvokeRepeating("Move", 0.1f, 0.1f);
        Ready();
    }

    /// <summary>
    /// �ގ��{�^������
    /// </summary>
    public async void LeaveRoom()
    {
        CancelInvoke("Move");
        int.TryParse(userID.text, out int id);
        // �ގ�
        await roomModel.LeaveAsync(SendData.roomName, id);
        SceneManager.LoadScene("Title");
    }

    /// <summary>
    /// �ړ��L�[����
    /// </summary>
    public async void Move()
    {
        // �ړ�
        await roomModel.MoveAsync(characterList[roomModel.ConnectionID].transform.position,
            characterList[roomModel.ConnectionID].transform.eulerAngles,
            IRoomHubReceiver.PlayerState.Move);
    }

    /// <summary>
    /// ���������{�^������
    /// </summary>
    public async void Ready()
    {
        await roomModel.ReadyAsync();
    }

    public async void Finish()
    {
        await roomModel.FinishAsync();
    }

    public async void Attack(Guid connectionID)
    {
        await roomModel.AttackAsync(connectionID);
    }

    /// <summary>
    /// ���[�U�U���ʒm
    /// </summary>
    void OnAttackUser(Guid connectionID, int health)
    {
        Debug.Log(characterList[roomModel.ConnectionID] + "��HP�F" + health);
        if (health <= 0)
        {
            Finish();
        }
    }

    /// <summary>
    /// �L�[���͈ړ�����
    /// </summary>
    void MovePlayer()
    {
        // �J�n�O�ƃQ�[���I����͈ړ������Ȃ�
        if (isStart != true || isFinish == true) return;
        Rigidbody rb = characterList[roomModel.ConnectionID].GetComponent<Rigidbody>();  // rigidbody���擾

        // RightArrow�L�[�܂���D�L�[���������ꍇ
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            // ���g�̃I�u�W�F�N�g�̓����ʂɗ͂�������
            rb.AddForce(new Vector3(moveSpeed, 0f, 0f), ForceMode.Impulse);
        }

        // LeftArrow�L�[�܂���A�L�[���������ꍇ
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            // ���g�̃I�u�W�F�N�g�̐����ʂɗ͂�������
            rb.AddForce(new Vector3(-moveSpeed, 0f, 0f), ForceMode.Impulse);
        }

        // UpArrow�L�[�܂���W�L�[���������ꍇ
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            // ���g�̃I�u�W�F�N�g�̓���ʂɗ͂�������
            rb.AddForce(new Vector3(0f, 0f, moveSpeed), ForceMode.Impulse);
        }

        // DownArrow�L�[�܂���S�L�[���������ꍇ
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            // ���g�̃I�u�W�F�N�g�̖k���ʂɗ͂�������
            rb.AddForce(new Vector3(0f, 0f, -moveSpeed), ForceMode.Impulse);
        }
    }

    /// <summary>
    /// �X�L��(�e)�ύX����
    /// </summary>
    void ChangeSkin()
    {
        switch (skinNum)
        {
            case 0:
                skinList[0].SetActive(false);
                skinList[1].SetActive(true);
                skinList[2].SetActive(false);
                skinNum = 1;
                break;
            case 1:
                skinList[0].SetActive(false);
                skinList[1].SetActive(false);
                skinList[2].SetActive(true);
                skinNum = 2;
                break;
            case 2:
                skinList[0].SetActive(true);
                skinList[1].SetActive(false);
                skinList[2].SetActive(false);
                skinNum = 0;
                break;
        }
    }

    /// <summary>
    /// ���_�ύX����
    /// </summary>
    void LookUp()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            myCamera.transform.DOLocalRotate(new Vector3(5f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
            moveSpeed = 0.2f;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            myCamera.transform.DOLocalRotate(new Vector3(30f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
            moveSpeed = 1f;
        }
    }

    /// <summary>
    /// �A�C�e���������̏���
    /// </summary>
    void SpawnItem()
    {
        LookAtCamera camScript = FindAnyObjectByType<LookAtCamera>();
        Camera camera = myCamera.GetComponent<Camera>();
        camScript.GetCamera(camera);
    }
}
