/// ==============================
/// オブジェクト破壊エリアスクリプト
/// Name:西浦晃太 Update:02/03
/// ==============================
using UnityEngine;

public class Breaker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 触れたオブジェクトがヘルスの場合、それを破壊
        if (other.gameObject.tag == "Health")
        {
            Destroy(other.gameObject);
        }
    }
}
