/// ==============================
/// �I�u�W�F�N�g�j��G���A�X�N���v�g
/// Name:���Y�W�� Update:02/03
/// ==============================
using UnityEngine;

public class Breaker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // �G�ꂽ�I�u�W�F�N�g���w���X�̏ꍇ�A�����j��
        if (other.gameObject.tag == "Health")
        {
            Destroy(other.gameObject);
        }
    }
}
