/// ==============================
/// �Q�[���f�B���N�^�[�X�N���v�g
/// Name:���Y�W�� Update:12/24
/// ==============================
using DG.Tweening;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TedLab;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    // �������郆�[�U�v���n�u
    [SerializeField] GameObject characterPrefabs; 
    // ��������g���b�v�v���n�u
    [SerializeField] GameObject trapPrefabs;
    // �ގ��{�^��
    [SerializeField] GameObject exitButton;
    // �������f��
    [SerializeField] RoomHubModel roomModel;
    // �������[�U�̃f�B�N�V���i���[
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();
    // �J�E���g�_�E���\���p�l��
    [SerializeField] GameObject countPanel;
    // �J�E���g�_�E���e�L�X�g
    [SerializeField] Text countText;
    // ������
    [SerializeField] Text roomName;
    // ���L�A�C�e���p�l��
    [SerializeField] Image itemPanel;

    GameObject myCamera;

    List<GameObject> heartList;

    Player playerScript;
    // �Q�[���J�n����ϐ�
    bool isStart = false;

    bool isFinish = false;

    public float moveSpeed = 1f;

    public int userID;

    string nowItemName = "";

    bool isBoost = false;

    bool isMaster;

    int time = 30;

    // Start is called before the first frame update
    async void Start()
    {
#if UNITY_EDITOR
        //�G�f�B�^�[���s��
        if(SendData.roomName == null) SendData.roomName = "RoomRoom";
        SendData.userID = 1;
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

        // ���[�U�����������Ƃ��Ƀ��\�b�h�����s����悤���f���ɓo�^
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMovedUser += this.OnMovedUser;
        //roomModel.OnReadyUser += this.OnReadyUser;
        roomModel.OnFinishUser += this.OnFinishUser;
        roomModel.OnAttackUser += this.OnAttackUser;
        roomModel.OnCountUser += this.OnCountUser;
        roomModel.OnChangeSkinUser += this.OnChangeSkinUser;
        // �ڑ�
        await roomModel.ConnectAsync();

        await Task.Delay(600);

        JoinRoom();
    
        roomName.text = "RoomName:" +  SendData.roomName;

        // �n�[�g�̃Q�[���I�u�W�F�N�g���擾
        heartList = new List<GameObject>();
        // �e�n�[�g�����X�g�ɓ����
        for (int i = 0; i < 3; i++)
        {
            heartList.Add(GameObject.Find("Heart_" + (i + 1)));
        }
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

        // �����ȊO�̊e�I�u�W�F�N�g���A�N�e�B�u�ɂ���
        if (roomModel.ConnectionID != user.ConnectionID)
        {
            // �Ώۂ̂̃J�������擾
            GameObject obj = characterGameObject.transform.GetChild(0).gameObject;
            // �J�������A�N�e�B�u��
            obj.SetActive(false);
            // �A�^�b�N�]�[�����A�N�e�B�u��
            GameObject zone = characterGameObject.transform.GetChild(1).gameObject;
            zone.SetActive(false);

            //Change(user.UserData.Id);
        }
        else if (roomModel.ConnectionID == user.ConnectionID)
        {
            if (user.JoinOrder == 1)
            {
                isMaster = true;
            }

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

            // �𑜓x�ݒ�𐶐������J�����ɐݒ�
            RectScalerWithViewport rectScalerWithViewport = GameObject.Find("RectScalerPanel").GetComponent<RectScalerWithViewport>();
            rectScalerWithViewport.refCamera = camera;

            // �X�L���ύX����
            ChangeSkin(characterGameObject.transform.GetChild(2));
        }

        // ���ʔԍ����e�q�I�u�W�F�N�g�̖��O�ɕt����
        foreach (Transform obj in characterGameObject.transform.GetChild(2))
        {
            if (obj.tag == "Shadow")
            {
                obj.name += "_" + user.UserData.Id;
            }
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
        if(characterList.Count == 0) return;
        // �e�g�����X�t�H�[�����A�j���[�V����
        characterList[connectionID].transform.DOLocalMove(pos,0.1f).SetEase(Ease.Linear);
        characterList[connectionID].transform.DOLocalRotate(rot,0.1f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// ���[�U��������
    /// </summary>
    void OnReadyUser()
    {
        //isStart = true;
        //if (isMaster ==true) StartGame();
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
        InvokeRepeating("CountDown", 0.1f, 1f);
        isStart = true;
    }

    /// <summary>
    /// �Q�[���I������
    /// </summary>
    void FinishGame()
    {
        exitButton.SetActive(false);
        countText.text = "�N���b�N�őގ�";
        CancelInvoke("CountDown");
        isFinish = true;
    }

    public async void JoinRoom()
    {
        //System.Random rand = new System.Random();
        //int id = rand.Next(1, 4);
  
        //ConnectPanel.SetActive(false);
        //int.TryParse(userID.text, out int id);
        //userID = id;

        await roomModel.JoinAsync(SendData.roomName, SendData.userID);

        InvokeRepeating("Move", 0.1f, 0.1f);

        isStart = true;
        if (isMaster == true) StartGame();
    }

    /// <summary>
    /// �ގ��{�^������
    /// </summary>
    public async void LeaveRoom()
    {
        CancelInvoke("Move");
        // �ގ�
        await roomModel.LeaveAsync(SendData.roomName, SendData.userID);
        SceneManager.LoadScene("Title");
    }

    /// <summary>
    /// �ړ��L�[����
    /// </summary>
    public async void Move()
    {
        if (characterList[roomModel.ConnectionID] == null) return;
        // �ړ�
        await roomModel.MoveAsync(characterList[roomModel.ConnectionID].transform.position,
            characterList[roomModel.ConnectionID].transform.eulerAngles,
            IRoomHubReceiver.PlayerState.Move);
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

        // �󂯎�����ڑ�ID�Ǝ��g�̐ڑ�ID����v���Ă���ꍇ
        if (connectionID == roomModel.ConnectionID)
        {
            // �J������h�炷
            characterList[roomModel.ConnectionID].transform.GetChild(0).DOShakePosition(0.6f, 1.5f, 45, 15, false, true);

            Destroy(heartList[health]);
            heartList.Remove(heartList[health]);
        }

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
    void ChangeSkin(Transform transform)
    {
        // �擾�������̐������[�v
        foreach (Transform ob in transform)
        {
            // ���r�[�Őݒ肵���X�L�����ƈ�v���Ă����ꍇ
            //if (SendData.skinName == ob.name)
            //{ //�X�L����\��
            //    GameObject.Find(ob.name).SetActive(true);
            //}
            //else
            //{ //�X�L�����\����
            //    GameObject.Find(ob.name).SetActive(false);
            //}

            // ���r�[�Őݒ肵���X�L�����ƈ�v���Ȃ��ꍇ
            if (SendData.skinName != ob.name)
            {
                // �R���C�_�[������
                ob.GetComponent<BoxCollider>().enabled = false;
                // �����x��0�ɂ���
                ob.GetComponent<Renderer>().material.color = new Color(255, 255, 255, 0);
            }
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
    /// �A�C�e���𓥂񂾍ۂ̏���
    /// </summary>
    public void StompItem(string name)
    {
        // ���\�[�X����A�A�C�e���e�N�X�`�����擾
        Texture2D texture = Resources.Load("Items/" + name) as Texture2D;

        itemPanel.sprite = 
            Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        nowItemName = name;
    }

    /// <summary>
    /// �A�C�e���ʌ��ʏ���
    /// </summary>
    public async void UseItem()
    {
        // �A�C�e���ʏ���
        switch (nowItemName)
        {
            case "Compass": // �R���p�X�̏ꍇ
                // ����̈ʒu���}�b�v��3�b�ԕ\������
                break;

            case "RollerBlade": // ���[���[�u���[�h�̏ꍇ
                // �ړ����x��3�b��2�{�ɂ���
                moveSpeed = 2.0f;
                isBoost = true;
                break;

            case "StopWatch": // �X�g�b�v�E�H�b�`�̏ꍇ
                // �Q�[�����Ԃ�3�b��������
                time += 3;
                await roomModel.CountTimer(time);
                break;

            case "Trap": // �g���b�v�̏ꍇ
                // �t�B�[���h�Ƀg���b�v��ݒu
                Vector3 playerPos = characterList[roomModel.ConnectionID].transform.position;
                // �C���X�^���X����
                GameObject trapObj = Instantiate(trapPrefabs);
                trapObj.name = "Trap(active)";
                // �����ʒu��ݒ�
                trapObj.transform.position = new Vector3(playerPos.x, playerPos.y + 0.3f, playerPos.x);
                break;

            case "Projector": // ���e�@�̏ꍇ
                // 5�b�Ԏ��R�ɓ����U�̉e����������
                break;

            default:
                break;
        }

        // ���ݏ��L���Ă���A�C�e���̖��O��������
        nowItemName = "";

        // ���\�[�X����A�A�C�e���e�N�X�`�����擾
        Texture2D texture = Resources.Load("Items/None") as Texture2D;

        // �A�C�e���e�N�X�`���𖢏����̃e�N�X�`���ɕύX
        itemPanel.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                               Vector2.zero);

        // ���[���[�u���[�h���g�p���Ă����ꍇ
        if (isBoost == true)
        {
            //3 �b��ɑ��x��߂�
            await Task.Delay(1800);
            moveSpeed = 1.0f;
            isBoost = false;
        }
    }

    /// <summary>
    /// �J�E���g�_�E������
    /// </summary>
    async void CountDown()
    {
        // �������Ԃ�-1
        time -= 1;
        // �c�莞�Ԃ�S���[�U�ɒʒm
        await roomModel.CountTimer(time);
        // ���Z���ʂ�UI�ɓK��
        countText.text = time.ToString();
        // �^�C���A�b�v��
        if (time <= 0)
        {
            // UI�Ƀ^�C���A�b�v�ƕ\�L����
            countText.text = "Time UP";
            // �J�E���g�_�E���������~
            CancelInvoke("CountDown");
            // �Q�[���G���h�������Ă�
            Finish();
        }
    }
    void OnCountUser(int time)
    {
        this.time = time;

        // ���Z���ʂ�UI�ɓK��
        countText.text = this.time.ToString();
        // �^�C���A�b�v��
        if (this.time <= 0)
        {
            // UI�Ƀ^�C���A�b�v�ƕ\�L����
            countText.text = "Time UP";
            // �J�E���g�_�E���������~
            CancelInvoke("CountDown");
            // �Q�[���G���h�������Ă�
            Finish();
        }
    }

    void OnChangeSkinUser(int userID, string skinName)
    {
        // �擾�������̐������[�v
        foreach (Transform ob in GameObject.Find(userID.ToString()).transform.GetChild(2))
        {
            //// ���r�[�Őݒ肵���X�L�����ƈ�v���Ă����ꍇ
            //if (skinName == ob.name)
            //{ //�X�L����\��
            //    GameObject.Find(ob.name).SetActive(true);
            //}
            //else
            //{ //�X�L�����\����
            //    GameObject.Find(ob.name).SetActive(false);
            //}

            // ���r�[�Őݒ肵���X�L�����ƈ�v���Ȃ��ꍇ
            if (skinName != ob.name)
            {
                // �R���C�_�[������
                ob.GetComponent<BoxCollider>().enabled = false;
                // �����x��0�ɂ���
                ob.GetComponent<Renderer>().material.color = new Color(255, 255, 255, 0);
            }
        }
    }

    async void Change(int id)
    {
        await roomModel.ChangeSkinAsync(id, SendData.skinName);
    }
}