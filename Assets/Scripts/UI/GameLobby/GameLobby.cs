using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLobby : MonoBehaviour
{
    [Header("手动赋值")]
    public Text userName;
    public Button enterGameBtn;
    public Button quitGameBtn;

    [Header("自动赋值")]
    public UserInfo userInfo;

    // Start is called before the first frame update
    void Start()
    {
        userInfo = GameRunning.instance.userInfo;
        userName.text = userInfo.name;

        enterGameBtn.onClick.AddListener(() => { SceneManager.LoadScene("GameBattle"); });
        quitGameBtn.onClick.AddListener(() => { MyTools.QuitGame(); });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
