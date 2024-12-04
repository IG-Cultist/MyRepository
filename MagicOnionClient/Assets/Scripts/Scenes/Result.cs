using UnityEngine;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            SceneManager.LoadScene("Title");
        }
    }
}
