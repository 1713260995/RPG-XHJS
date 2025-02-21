using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLogin : MonoBehaviour
{
    [Header("手动赋值")]
    public InputField account;
    public InputField password;
    public Button loginBtn;
    public Button registerBtn;
    public Button findPwd;
    public TipWindow tipWindow;
    public RolesNameWindows nameWindow;


    [Header("自动赋值")]
    public static string userInfoPath = "userInfo.txt";

    // Start is called before the first frame update
    void Start()
    {
        loginBtn.onClick.AddListener(LoginEvent);
        registerBtn.onClick.AddListener(RegisterEvent);
        findPwd.onClick.AddListener(FindEvent);
    }

    private void FindEvent()
    {
        string txt = MyTools.ReadTxt(userInfoPath);
        if (string.IsNullOrEmpty(txt))
        {
            tipWindow.OpenTips("请先注册账号！");
            return;
        }
        UserInfo user = JsonConvert.DeserializeObject<UserInfo>(txt);
        tipWindow.OpenTips($"您的账号是：{user.account}\n您的密码是:{user.password}");
    }

    private void RegisterEvent()
    {
        if (string.IsNullOrEmpty(account.text))
        {
            tipWindow.OpenTips("账号不能为空");
            return;
        }
        if (string.IsNullOrEmpty(password.text))
        {
            tipWindow.OpenTips("密码不能为空");
            return;
        }
        UserInfo userInfo = new UserInfo() { id = MyTools.GenerateIntID(6), account = account.text, password = password.text };
        string text = JsonConvert.SerializeObject(userInfo);
        MyTools.WriteTxt(userInfoPath, text);
        tipWindow.OpenTips("注册成功");
    }

    private void LoginEvent()
    {
        string txt = MyTools.ReadTxt(userInfoPath);
        if (string.IsNullOrEmpty(txt))
        {
            tipWindow.OpenTips("请先注册账号！");
            return;
        }
        UserInfo user = JsonConvert.DeserializeObject<UserInfo>(txt);
        if (user.account != account.text || user.password != password.text)
        {
            tipWindow.OpenTips("账号或者密码有误！");
            return;
        }
        GameRunning.instance.userInfo = user;
        if (string.IsNullOrEmpty(user.name))
        {
            gameObject.SetActive(false);
            nameWindow.gameObject.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("GameLobby");
        }
    }

}

public class UserInfo
{
    public int id;
    public string account;
    public string password;
    public string name;
    public int lv;
}