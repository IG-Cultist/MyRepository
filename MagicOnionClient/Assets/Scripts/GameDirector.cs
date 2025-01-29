/// ==============================
/// �Q�[���f�B���N�^�[�X�N���v�g
/// Name:���Y�W�� Update:1/24
/// ==============================
using DG.Tweening;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TedLab;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class GameDirector : MonoBehaviour
{
    // �������郆�[�U�v���n�u
    [SerializeField] GameObject characterPrefabs; 
    // ��������g���b�v�v���n�u
    [SerializeField] GameObject trapPrefabs;
    // ��������U�e�v���n�u
    [SerializeField] GameObject fakeShadowPrefabs;
    // �ގ��{�^��
    [SerializeField] GameObject exitButton;
    // �J�E���g�_�E���\���p�l��
    [SerializeField] GameObject countPanel;
    // �J�E���g�_�E���Q�[���I�u�W�F�N�g
    [SerializeField] GameObject[] coundDownObjects;
    // �J�n���}�Q�[���I�u�W�F�N�g
    [SerializeField] GameObject[] readyTextObjects;
    // �Q�[�����ʃI�u�W�F�N�g
    [SerializeField] GameObject[] resultObjects;

    // �ړ����x�㏸�A�C�R��
    [SerializeField] GameObject speedUpEffect;
    // �ʒu�\���A�C�R��
    [SerializeField] GameObject localizationEffect;
    // �e�����A�C�R��
    [SerializeField] GameObject spawnShadowEffect;

    // ���_�ύX�{�^��
    [SerializeField] GameObject viewButton;
    // �J�E���g�_�E���e�L�X�g
    [SerializeField] Text countText;
    // ���L�A�C�e���p�l��
    [SerializeField] Image itemPanel;
    // ����p�W���C�X�e�B�b�N
    [SerializeField] FixedJoystick joystick;

    // �������[�U�̃f�B�N�V���i���[
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();
    // �������f��
    [SerializeField] RoomHubModel roomModel;

    // �X�g�b�v�E�H�b�`�g�pSE
    [SerializeField] AudioClip stopWatchSE;
    // ���݂���SE
    [SerializeField] AudioClip stompSE;
    // �v���W�F�N�^�[�g�pSE
    [SerializeField] AudioClip projectorSE;
    // �g���b�v�g�pSE
    [SerializeField] AudioClip trapSE;
    // �^�C���A�b�vSE
    [SerializeField] AudioClip timeUpSE;
    // ���v�̐jSE
    [SerializeField] AudioClip clockSE;
    // �g���b�v����SE
    [SerializeField] AudioClip trapBiteSE;
    // SE
    [SerializeField] AudioClip compassSE;
    // ���[���u���[�h�g�pSE
    [SerializeField] AudioClip rollerBladeSE;

    // �I�[�f�B�I�\�[�X
    AudioSource audioSource;
    // �����̃J����
    GameObject myCamera;

    // �v���C���[��HP���X�g
    List<GameObject> heartList;
    List<GameObject> rivalHeartList;

    // �v���C���[��HP
    int playerHP = 3;
    int rivalHP = 3;
    
    // �v���C���[�X�N���v�g
    Player playerScript;
    // �A�C�e���X�N���v�g
    Item itemScript;

    // �Q�[���J�n����ϐ�
    bool isStart = false;
    // �Q�[���I������ϐ�
    bool isFinish = false;

    float defaultSpeed = 1.5f;

    // �ړ����x
    public float moveSpeed;

    public int userID;

    // ���e�@�g�p�񐔃J�E���g
    int useProjector = 0;

    // ���ݏ��L���Ă���A�C�e����
    string nowItemName = "";

    // �ړ����x�u�[�X�g����
    bool isBoost = false;

    // �ʒu���蔻��
    bool isLocate = false;

    // �}�X�^�[�N���C�A���g����
    bool isMaster;

    // ��������
    public int time = 31;
    // �^�C���A�b�v��SE�Đ��񐔗p�ϐ�
    int timeUpCnt = 0;

    // ���_�ύX�p�ϐ�
    int viewCount = 2;
    bool isCooldown = false;

    // �{�^�����������Ƃ�true�A�������Ƃ�false�ɂȂ�t���O
    bool buttonDownFlag = false;

    // Start is called before the first frame update
    async void Start()
    {
#if UNITY_EDITOR
        //�G�f�B�^�[���s��
        if (SendData.roomName == null) 
        {
            SendData.roomName = "RoomRoom"; 
            SendData.userID = 1; 
        }

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

        // ���[�U�����������Ƃ��Ƀ��\�b�h�����s����悤���f���ɓo�^
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMovedUser += this.OnMovedUser;
        roomModel.OnFinishUser += this.OnFinishUser;
        roomModel.OnAttackUser += this.OnAttackUser;
        roomModel.OnCountUser += this.OnCountUser;
        roomModel.OnChangeSkinUser += this.OnChangeSkinUser;
        roomModel.OnUseItemUser += this.OnUseItemUser;
        roomModel.OnStompItemUser += this.OnStompItemUser;
        // �I�[�f�B�I�\�[�X�̃R���|�l���g�擾
        audioSource = GetComponent<AudioSource>();

        itemScript = GameObject.Find("ItemManager").GetComponent<Item>();

        moveSpeed = defaultSpeed;

        // ��\���ɂ���
        exitButton.SetActive(false);
        spawnShadowEffect.SetActive(false);
        localizationEffect.SetActive(false);
        speedUpEffect.SetActive(false);

        // �J�E���g�_�E���p�e�L�X�g���\��
        for (int i = 0; i < coundDownObjects.Length; i++)
        {
            coundDownObjects[i].SetActive(false);
        }
        // �J�n�ʒm�p�e�L�X�g���\��
        for (int i = 0; i < readyTextObjects.Length; i++)
        {
            readyTextObjects[i].SetActive(false);
        }
        // ���s����p�e�L�X�g���\��
        for (int i = 0; i < resultObjects.Length; i++)
        {
            resultObjects[i].SetActive(false);
        }
        // �n�[�g�̃Q�[���I�u�W�F�N�g���擾
        heartList = new List<GameObject>();
        rivalHeartList = new List<GameObject>();
        // �e�n�[�g�����X�g�ɓ����
        for (int i = 0; i < 3; i++)
        {
            rivalHeartList.Add(GameObject.Find("Rival_Heart_" + (i + 1)));
            heartList.Add(GameObject.Find("Heart_" + (i + 1)));
        }

        // �ڑ�
        await roomModel.ConnectAsync();

        await Task.Delay(600);

        JoinRoom();
    }

    void Update()
    {
        MovePlayer();

        // ����������
        if (buttonDownFlag)
        {
            // ���Ԑ؂�ɂȂ����狭���I�Ɏ��_��߂�
            if (viewCount <= 0) OnButtonUp();
        }

        // �Q�[���I����Ԃ���ʃ^�b�v�������ꍇ
        if (isFinish == true && Input.GetMouseButtonDown(0))
        {
            LeaveRoom();
        }

        if (timeUpCnt >= 4) CancelInvoke("TimeUp");

        if(this.time > 3)
        {
            // �J�E���g�_�E���p�e�L�X�g���\��
            for (int i = 0; i < coundDownObjects.Length; i++)
            {
                coundDownObjects[i].SetActive(false);
            }
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

        // �����ʒu��ݒ�
        characterGameObject.transform.position = new Vector3(-7.5f + (2 * user.UserData.Id), 2f, -10f);

        characterGameObject.name = user.UserData.Id.ToString();

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

            // ���g�̉e�Ƌ�ʂ��邽�߃^�O��ύX
            foreach (Transform item in characterGameObject.transform.GetChild(2))
            {
                item.tag = "Shadow_Rival";
            }
            // �}�b�v�����\����
            characterGameObject.transform.GetChild(3).GetComponent<MeshRenderer>().enabled = false;
            obj.tag = "Camera_Rival";
            characterGameObject.tag = "Rival";
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

            // �A�C�e���X�N���v�g�֎��g�̃J������n��
            itemScript.playerCam = camera;

            // �𑜓x�ݒ�𐶐������J�����ɐݒ�
            RectScalerWithViewport rectScalerWithViewport = GameObject.Find("RectScalerPanel").GetComponent<RectScalerWithViewport>();
            rectScalerWithViewport.refCamera = camera;
        }

        ChangeSkin(user.UserData.Id, user.SkinName);

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
        itemScript.StartSpawn();
        countPanel.SetActive(true);
        InvokeRepeating("CountDown", 0.1f, 1f);
    }

    async void ReadyGo()
    {
        readyTextObjects[0].SetActive(true);
        await Task.Delay(1200);

        readyTextObjects[0].SetActive(false);
        readyTextObjects[1].SetActive(true);

        await Task.Delay(800);
        readyTextObjects[1].SetActive(false);
    }

    /// <summary>
    /// �Q�[���I������
    /// </summary>
    void FinishGame()
    {
        exitButton.SetActive(false);
        CancelInvoke("CountDown");
        isFinish = true;
        InvokeRepeating("TimeUp", 0.1f, 2f);   
    }

    public async void JoinRoom()
    {
        await roomModel.JoinAsync(SendData.roomName, SendData.userID, SendData.skinName);

        InvokeRepeating("Move", 0.1f, 0.1f);

        ReadyGo();
        //isStart = true;
        if (isMaster == true)
        {
            StartGame();
        }
    }

    /// <summary>
    /// �ގ��{�^������
    /// </summary>
    public async void LeaveRoom()
    {
        CancelInvoke("Move");

        countText.text = "";
     
        // �ގ�
        await roomModel.LeaveAsync(SendData.roomName, SendData.userID);
        Initiate.DoneFading();
        Initiate.Fade("Result", Color.black, 0.7f);
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
            playerHP = health;

            if (health <= 0)
            {
                characterList[connectionID].SetActive(false);
                resultObjects[1].SetActive(true);
                Finish();
            }
        }
        else
        {
            Destroy(rivalHeartList[health]);
            rivalHP = health;

            if (health <= 0)
            {
                characterList[connectionID].SetActive(false);
                resultObjects[0].SetActive(true);
                Finish();
            }
        }
    }

    /// <summary>
    /// �L�[���͈ړ�����
    /// </summary>
    void MovePlayer()
    {
        // �J�n�O�ƃQ�[���I����͈ړ������Ȃ�
        if (isStart != true) return;
        if (isFinish == true) return;
        Rigidbody rb = characterList[roomModel.ConnectionID].GetComponent<Rigidbody>();  // rigidbody���擾

        float axisZ = joystick.Vertical;
        float axisX = joystick.Horizontal;

        rb.AddRelativeForce(new Vector3(90 * axisX, 0, 90 * axisZ));

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
    /// �A�C�e���𓥂񂾍ۂ̏���
    /// </summary>
    public async void StompItem(string name)
    {
        string[] words = name.Split("_");

        // ���񂾂̂��U�e�łȂ��ꍇ
        if (words[0] != "Fake")
        {
            // ���\�[�X����A�A�C�e���e�N�X�`�����擾
            Texture2D texture = Resources.Load("Items/" + words[0]) as Texture2D;

            itemPanel.sprite =
                Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            nowItemName = name;
        }   
        await roomModel.StompItemAsync(name);
    }

    /// <summary>
    /// ���񂾃A�C�e���j��ʒm
    /// </summary>
    /// <param name="itemName"></param>
    async void OnStompItemUser(string itemName)
    {
        GameObject obj = GameObject.Find(itemName);
        if (obj != null) Destroy(obj);
    }

    /// <summary>
    /// �A�C�e���ʌ��ʏ���
    /// </summary>
    public async void UseItem()
    {
        if (isStart != true) return;
        if (isFinish == true) return;

        string[] words = nowItemName.Split("_");

        GameObject rivalObj = new GameObject();
        Destroy(rivalObj);

        // �����łȂ��v���C���[�̃Q�[���I�u�W�F�N�g���擾
        foreach (var id in characterList.Keys)
        {
            if (id != roomModel.ConnectionID)
            {
                rivalObj = characterList[id];
                break;
            }
        }
        // ���g�̈ʒu���擾
        Vector3 playerPos = characterList[roomModel.ConnectionID].transform.position;

        // �A�C�e���ʏ���
        switch (words[0])
        {
            case "Compass": // �R���p�X�̏ꍇ
                // ����̈ʒu���}�b�v��3�b�ԕ\������
                audioSource.PlayOneShot(compassSE);
                
                //���C�o���̈ʒu��\��
                rivalObj.transform.GetChild(3).GetComponent<MeshRenderer>().enabled = true;
                localizationEffect.SetActive(true);
                isLocate = true;
                break;

            case "RollerBlade": // ���[���[�u���[�h�̏ꍇ
                audioSource.PlayOneShot(rollerBladeSE);
                speedUpEffect.SetActive(true);
                // �ړ����x��3�b��2�{�ɂ���
                moveSpeed = 2.0f;
                isBoost = true;
                break;

            case "StopWatch": // �X�g�b�v�E�H�b�`�̏ꍇ
                // �Q�[�����Ԃ�3�b��������
                audioSource.PlayOneShot(stopWatchSE);
                time += 3;
                await roomModel.CountTimer(time);
                break;

            case "Trap": // �g���b�v�̏ꍇ
                audioSource.PlayOneShot(trapSE);

                // �C���X�^���X����
                GameObject trapObj = Instantiate(trapPrefabs);
                trapObj.name = "Trap(active)";
                // �����ʒu��ݒ�
                trapObj.transform.position = new Vector3(playerPos.x, playerPos.y + 1.0f, playerPos.z + 3.0f); 

                await roomModel.UseItemAsync(roomModel.ConnectionID, nowItemName);
                break;

            case "Projector": // ���e�@�̏ꍇ

                useProjector++;
                // 5�b�Ԏ��R�ɓ����U�̉e����������
                audioSource.PlayOneShot(projectorSE);

                // �C���X�^���X����
                GameObject fakeObj = Instantiate(fakeShadowPrefabs);

                fakeObj.name = "Fake_Shadow" + "_" + useProjector;

                // �����ʒu��ݒ�
                fakeObj.transform.position = new Vector3(playerPos.x, 1.7f, playerPos.z + 3.0f);

                spawnShadowEffect.SetActive(true);
                KillFake(fakeObj);
                await roomModel.UseItemAsync(roomModel.ConnectionID, nowItemName);
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
            // 3�b��ɑ��x��߂�
            await Task.Delay(1800);
            speedUpEffect.SetActive(false);
            moveSpeed = defaultSpeed;
            isBoost = false;
        }

        // �R���p�X���g�p���Ă����ꍇ
        if (isLocate == true)
        {
            // 3�b��Ɉʒu���������
            await Task.Delay(1800);
            rivalObj.transform.GetChild(3).GetComponent<MeshRenderer>().enabled = false;
            localizationEffect.SetActive(false);
            isLocate = false;
        }
    }

    public async void OnUseItemUser(Guid connectionID, string itemName)
    {
        Vector3 playerPos = characterList[connectionID].transform.position;
        
        switch (itemName)
        {
            case "Trap": // �g���b�v�̏ꍇ
                audioSource.PlayOneShot(trapSE);
                // �C���X�^���X����
                GameObject trapObj = Instantiate(trapPrefabs);
                trapObj.name = "Trap(active)";
                // �����ʒu��ݒ�
                trapObj.transform.position = new Vector3(playerPos.x, playerPos.y + 1.0f, playerPos.z + 3.0f);

                break;

            case "Projector": // ���e�@�̏ꍇ
                useProjector++;
                // 5�b�Ԏ��R�ɓ����U�̉e����������
                audioSource.PlayOneShot(projectorSE);

                // �C���X�^���X����
                GameObject fakeObj = Instantiate(fakeShadowPrefabs);

                fakeObj.name = "Fake_Shadow" + "_" + useProjector;

                // �����ʒu��ݒ�
                fakeObj.transform.position = new Vector3(playerPos.x, 1.7f, playerPos.z + 3.0f);
                break;

            case "Trap(active)": //�g���b�v(�A�N�e�B�u)�̏ꍇ
                // �j��
                audioSource.PlayOneShot(trapBiteSE);
                GameObject obj = GameObject.Find(itemName);
                if (obj != null) Destroy(obj);
                break;

            default:
                break;
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
    }

    /// <summary>
    /// �J�E���g�_�E���ʒm
    /// </summary>
    /// <param name="time"></param>
    void OnCountUser(int time)
    {
        isStart = true;

        // �������Ԃ𑗂��Ă������ԂƓ�������
        this.time = time;

        switch (this.time)
        {
            case 3: // 3������ �J�E���g�_�E���p�I�u�W�F�N�g����ɂ���
                countText.text = "";
                coundDownObjects[0].SetActive(true);
                break;

            case 2: // 2������
                coundDownObjects[0].SetActive(false);
                coundDownObjects[1].SetActive(true);
                break;

            case 1: // 1������
                coundDownObjects[1].SetActive(false);
                coundDownObjects[2].SetActive(true);
                break;

            case <= 0: // �^�C���A�b�v
                coundDownObjects[2].SetActive(false);
                // UI�Ƀ^�C���A�b�v�ƕ\�L����
                countText.text = "Time UP";
                // �J�E���g�_�E���������~
                CancelInvoke("CountDown");

                if(playerHP < rivalHP) resultObjects[1].SetActive(true);
                else if (playerHP > rivalHP) resultObjects[0].SetActive(true);
                else resultObjects[2].SetActive(true);
                // �Q�[���G���h�������Ă�
                Finish();
                break;

            default: // ����ȊO�̓J�E���g����ʂɔ��f
                // ���Z���ʂ�UI�ɓK��
                countText.text = this.time.ToString();
                break;
        }
    }

    void OnChangeSkinUser(int userID, string skinName)
    {
        // �擾�������̐������[�v
        foreach (Transform ob in GameObject.Find(userID.ToString()).transform.GetChild(2))
        {
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

    async void ChangeSkin(int userID, string skinName)
    {
        await roomModel.ChangeSkinAsync(userID, skinName);
    }

    void TimeUp()
    {
        audioSource.PlayOneShot(timeUpSE);
        timeUpCnt++;
    }

    /// <summary>
    /// ���_�ύX�{�^������������
    /// </summary>
    public void OnButtonDown()
    {
        if (isStart != true) return;
        if (isFinish == true) return;

        if (isCooldown) return; // �N�[���_�E�����̏ꍇ�A�������Ȃ�

        // ���_�ύX���Ԃ�������
        viewCount = 3;
        // �����t���O��true��
        buttonDownFlag = true;

        // �J���������グ�鎋�_�ɉ�]
        myCamera.transform.DOLocalRotate(new Vector3(5f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
        // �ړ����x������������
        moveSpeed = 0.2f;
        // ���_�ύX���Ԃ��J�E���g�_�E��
        InvokeRepeating("ViewTime", 0.1f, 1f);
    }
    /// <summary>
    /// ���_�ύX��������
    /// </summary>
    public void OnButtonUp()
    {
        if (isStart != true) return;
        if (isFinish == true) return;

        if (isCooldown) return; // �N�[���_�E�����̏ꍇ�A�������Ȃ�

        // �N�[���_�E�����ɂ���
        isCooldown = true;
        // �J�E���g�_�E�����~
        CancelInvoke("ViewTime");
        // �����t���O��false��
        buttonDownFlag = false;

        // �J�����̉�]�����ɖ߂�
        myCamera.transform.DOLocalRotate(new Vector3(30f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
        //�ړ����x�����ɖ߂�
        moveSpeed = defaultSpeed;

        // ���\�[�X����A�A�C�R�����擾
        Texture2D texture = Resources.Load("UI/eye_close") as Texture2D;
        
        Image buttonTexture = viewButton.GetComponent<Image>();

        buttonTexture.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                       Vector2.zero);

        InvokeRepeating("ViewCooldown", 3f, 1f);

    }

    void ViewTime()
    {
        viewCount--;
    }

    void ViewCooldown()
    {
        CancelInvoke("ViewCooldown");

        // ���\�[�X����A�A�C�R�����擾
        Texture2D texture = Resources.Load("UI/eye_open") as Texture2D;

        Image buttonTexture = viewButton.GetComponent<Image>();

        buttonTexture.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                       Vector2.zero);
        isCooldown = false;
    }

    /// <summary>
    /// �U�e�폜����
    /// </summary>
    /// <param name="fakeObj"></param>
    async void KillFake(GameObject fakeObj)
    {
        // 10�b��ɉe���폜
        await Task.Delay(6000);

        // �폜�ʒm
        Destroy(fakeObj);
        StompItem(fakeObj.name);

        spawnShadowEffect.SetActive(false);
    }
}