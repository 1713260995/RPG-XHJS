using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RolesNameWindows : MonoBehaviour
{
    public GameLogin gameLogin;
    public InputField nameInput;
    public TipWindow tipWindow;
    public Button enterBtn;

    // Start is called before the first frame update
    void Start()
    {
        enterBtn.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(nameInput.text))
            {
                tipWindow.OpenTips("«Îœ» ‰»Î√˚≥∆£°");
                return;
            }
            UserInfo user = GameRunning.instance.userInfo;
            user.name = nameInput.text;
            string text = JsonConvert.SerializeObject(user);
            MyTools.WriteTxt(GameLogin.userInfoPath, text);
            SceneManager.LoadScene("GameLobby");
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
