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
    /// ���[�U�����M����
    /// </summary>
    public async void SendName()
    {
        bool result =  await userModel.RegistAsync(inputField.text);

        if (result == true) resultText.text = "���M����";
        else resultText.text = "���M���s";
    }
}
