/// ==============================
/// �J�����������X�N���v�g
/// Author: Nishiura Kouta
/// ==============================
using UnityEngine;

/// <summary>
/// �A�^�b�`�����I�u�W�F�N�g���J�����Ɍ������鏈��
/// </summary>
public class LookAtCamera : MonoBehaviour
{
    // ����Ώۂ̃J����
    Camera camera;

    /// <summary>
    /// �X�V����
    /// </summary>
    void Update()
    {
        // �J������null�łȂ��ꍇ�A�I�u�W�F�N�g���J�����Ɍ�������
        if(camera != null) transform.LookAt(camera.transform);
    }

    /// <summary>
    /// �^�[�Q�b�g�J�����擾����
    /// </summary>
    /// <param name="playerCam"></param>
    public void GetCamera(Camera playerCam)
    {
        // ���łɃJ�������ݒ肳��Ă����ꍇ�A�������Ȃ�
        if (camera != null) return;
        // �擾�����J��������
        camera = playerCam;
    }
}
