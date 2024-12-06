/// ==============================
/// �Q�[���f�B���N�^�[�X�N���v�g
/// Name:���Y�W�� Update:11/26
/// ==============================
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    // �������郆�[�U�v���n�u
    [SerializeField] GameObject characterPrefabs;

    // �ڑ��p�l��
    [SerializeField] GameObject ConnectPanel;

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
    // ���������{�^��
    [SerializeField] GameObject readyButton;
    Player playerScript;
    // �Q�[���J�n����ϐ�
    bool isStart = false;

    bool isFinish = false;

    bool isJoin = false;

    float moveSpeed = 1f;

    // Start is called before the first frame update
    async void Start()
    {
        // ��\���ɂ���
        countPanel.SetActive(false);
        readyButton.SetActive(false);

        // ���[�U�����������Ƃ��Ƀ��\�b�h�����s����悤���f���ɓo�^
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMovedUser += this.OnMovedUser;
        roomModel.OnReadyUser += this.OnReadyUser;
        roomModel.OnFinishUser += this.OnFinishUser;
        roomModel.OnAttackUser += this.OnAttackUser;
        // �ڑ�
        await roomModel.ConnectAsync();
    }

    void Update()
    {
        MovePlayer();

        if (Input.GetKeyDown(KeyCode.X) && isStart == true)
        {
            Finish();
        }

        // �Q�[���I����Ԃ���ʃ^�b�v�������ꍇ
        if (isFinish == true && Input.GetMouseButtonDown(0))
        {
            LeaveRoom();
            SceneManager.LoadScene("Result");
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
        characterGameObject.transform.position = new Vector3(-7.5f + (user.UserData.Id *2), 2f, -10f);

        // �����ȊO�̃J�������A�N�e�B�u�ɂ���
        if(roomModel.ConnectionID != user.ConnectionID){
            characterGameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        // ���ʔԍ����e�q�I�u�W�F�N�g�̖��O�ɕt����
        foreach (Transform obj in characterGameObject.transform)
        {
            obj.name += "_" + user.UserData.Id;
        }
        characterGameObject.name = characterPrefabs.name + "_" + user.UserData.Id;

        characterList[user.ConnectionID] = characterGameObject; // �t�B�[���h�ŕێ�
        playerScript.connectionID = user.ConnectionID;
        readyButton.SetActive(true);
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
            // ���������{�^�����\��
            readyButton.SetActive(false);
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
        readyButton.SetActive(false);
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
        isStart = true;
    }

    /// <summary>
    /// �Q�[���I������
    /// </summary>
    void FinishGame()
    {
        countText.text = "�N���b�N�őގ�";
        isFinish = true;
    }

    /// <summary>
    /// �����{�^������
    /// </summary>
    public async void JoinRoom()
    {
        //ConnectPanel.SetActive(false);
        int.TryParse(userID.text, out int id);
        // ����
        await roomModel.JoinAsync("sampleRoom", id);
        InvokeRepeating("Move", 0.1f, 0.1f);
        await Task.Delay(60);
        isJoin= true;
    }

    /// <summary>
    /// �ގ��{�^������
    /// </summary>
    public async void LeaveRoom()
    {
        ConnectPanel.SetActive(true);
        CancelInvoke("Move");
        int.TryParse(userID.text, out int id);
        // �ގ�
        await roomModel.LeaveAsync("sampleRoom", id);
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
        // �Q�[�����J�n���Ă��Ȃ��A���I�����Ă���ꍇ�������Ȃ�
        if (isStart != true || isFinish == true || isJoin != true) return;

        Rigidbody rb = characterList[roomModel.ConnectionID].GetComponent<Rigidbody>();  // rigidbody���擾

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            rb.AddForce(new Vector3(moveSpeed, 0f, 0f), ForceMode.Impulse);
            //characterList[roomModel.ConnectionID].transform.position += new Vector3(moveSpeed, 0f, 0f);
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            rb.AddForce(new Vector3(-moveSpeed, 0f, 0f), ForceMode.Impulse);
        }
        
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            rb.AddForce(new Vector3(0f, 0f, moveSpeed), ForceMode.Impulse);
        }
        
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            rb.AddForce(new Vector3(0f, 0f, -moveSpeed), ForceMode.Impulse);
        }
    }
}
