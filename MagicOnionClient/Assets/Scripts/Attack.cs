using UnityEngine;

public class Attack : MonoBehaviour
{
    // ゲームディレクタースクリプト
    GameDirector gameDirector;
    // プレイヤースクリプト
    Player player;

    void Start()
    {
        // 各スクリプトをフィールド内オブジェクトから取得
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        player = GameObject.Find(SendData.userID.ToString()).GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 踏んだのがアイテムの場合
        if (other.gameObject.tag == "Item")
        {
            Debug.Log(other.gameObject.name);
            // それを破壊する
            Destroy(other.gameObject);
            // 名前に応じた効果を発動
            gameDirector.StompItem(other.gameObject.name);
        }

        // 踏んだのが影の場合 かつクールダウン中でない場合
        if (other.gameObject.tag == "Shadow_Rival" && player.isHit == false)
        {
            player.Damage(other.gameObject);
            player.isHit = true;
        }
    }
}
