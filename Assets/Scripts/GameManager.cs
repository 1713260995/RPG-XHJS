using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [Header("手动赋值")]
    public Transform SkillParent;
    public Transform enemyListParent;
    public Transform worldCanvas;
    public RectTransform winBg;
    public RectTransform failBg;

    [Header("自动赋值")]
    public Transform player;
    public List<EnemyPos> enemyPosList;
    public bool isFail = false;//为true游戏失败
    public bool isWin = false;
    public bool wolfManIsDie = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

    }

    void Update()
    {
        GameFail();
        GameWin();


        if (isFail || isWin)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("GameLobby");
            }
        }
    }

    void GameFail()
    {
        if (player.GetComponent<PlayerCtrl>().currentHP <= 0 && !isFail)
        {
            failBg.gameObject.SetActive(true);
            Time.timeScale = 0;
            isFail = true;
        }
    }

    void GameWin()
    {
        if (wolfManIsDie == true)
        {
            winBg.gameObject.SetActive(true);
            Time.timeScale = 0;
            isWin = true;
        }
    }

}
