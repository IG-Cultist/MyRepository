using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [SerializeField] Text inputField;
    [SerializeField] Text resultText;
    [SerializeField] GameObject Button;
    UserModel userModel;

    // Start is called before the first frame update
    void Start()
    {
        userModel = FindObjectOfType<UserModel>();
    }

    /// <summary>
    /// ユーザ名送信処理
    /// </summary>
    public async void SendName()
    {
        bool result =  await userModel.RegistAsync(inputField.text);

        if (result == true) resultText.text = "送信成功";
        else resultText.text = "送信失敗";
    }
}
