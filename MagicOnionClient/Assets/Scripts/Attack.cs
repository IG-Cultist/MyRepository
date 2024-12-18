using UnityEngine;

public class Attack : MonoBehaviour
{
    GameDirector gameDirector;
    Player player;

    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        player = GameObject.Find(gameDirector.userID.ToString()).GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Item")
        {
            Debug.Log(other.gameObject.name);
            Destroy(other.gameObject);
            gameDirector.StompItem(other.gameObject.name);
        }
        
        if (other.gameObject.tag == "Shadow" && player.isHit == false)
        {
            if (other.gameObject.name != "shadow_normal_" + gameDirector.userID)
            {
                player.GetID(other.gameObject);
                player.isHit = true;
            }
        }
    }
}
