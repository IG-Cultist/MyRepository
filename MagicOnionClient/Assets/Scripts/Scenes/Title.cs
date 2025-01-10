using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] GameObject[] shadows;

    [SerializeField] Text userID;

    // �N���b�Nor�^�b�vSE
    [SerializeField] AudioClip clickSE;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SendData.roomName = "";
        ChangeShadow();
    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) audioSource.PlayOneShot(clickSE);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Initiate.DoneFading();
            Initiate.Fade("Lobby", Color.black, 0.7f);

            int.TryParse(userID.text, out int id);
            SendData.userID = id;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeShadow();
        }
    }

    void ChangeShadow()
    {
        for (int i = 0; i < shadows.Length; i++)
        {
            shadows[i].SetActive(false);
        }

        System.Random rand = new System.Random();
        int rndNum = rand.Next(0, shadows.Length);

        shadows[rndNum].SetActive(true);
    }
}
