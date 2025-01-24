using UnityEngine;

public class FakeShadow : MonoBehaviour
{
    Rigidbody rb;
    System.Random rand;
    [SerializeField] GameObject[] shadows;

    void Start()
    {
        rand = new System.Random();

        // 初期移動値のランダム設定
        int num = rand.Next(0, 2);

        rb = this.transform.GetComponent<Rigidbody>();  // rigidbodyを取得

        // 発射角度分岐
        switch (num)
        {
            case 0:
                rb.AddForce(new Vector3(8f, 0f, 8f), ForceMode.Impulse);
                break;
            case 1:
                rb.AddForce(new Vector3(-8f, 0f, 8f), ForceMode.Impulse);
                break;
        }

        // 全スキンを非表示
        foreach (var obj in shadows)
        {
            obj.SetActive(false);
        }

        SetSkin();
    }

    /// <summary>
    /// スキンを設定
    /// </summary>
    void SetSkin()
    {
        // スキン値のランダム設定
        int num = rand.Next(0, this.transform.childCount);

        // 抽選されたものを表示
        shadows[num].gameObject.SetActive(true);
    }
}
