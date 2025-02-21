using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 对话框
/// </summary>
public class DialogFrame : MonoBehaviour
{

    [Header("手动赋值")]
    public Text dialogTxt;
    public MonologueFrame monologueFrame;//独白框

    [SerializeField]
    [Header("自动赋值")]
    private int mouseDownCount = 1;
    private List<string> dialogList;


    // Start is called before the first frame update
    void Start()
    {
        dialogList = Dialog1Content();
        GameManager.instance.player.GetComponent<PlayerCtrl>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Dialog1();
    }

    private void Dialog1()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (mouseDownCount == dialogList.Count)
            {
                gameObject.SetActive(false);
                monologueFrame.gameObject.SetActive(true);
                return;
            }
            dialogTxt.text = dialogList[mouseDownCount];
            mouseDownCount++;
        }
    }

    /// <summary>
    /// 对话1的内容
    /// </summary>
    List<string> Dialog1Content()
    {
        return new List<string> {
        "你终于醒了。",
        "你是谁，我在哪？",
        "我叫张仲景，是一位医师。这里是洛阳。",
        "啊，我为什么会在这里？",
        "刚才我的朋友跟我说，这附近有一位昏迷的女子，请我救助。想必此人就是你吧。他让我告诉你，他是你师傅，是他把你带到的这里的。" +
        "并且他委托我，让我教你中医，希望你能潜心学习，日后运用中医知识救助百姓。",
        "嗯，我是记得师傅之前和我说过要我学习中医。",
        "我虽才识浅薄，但对于中医的知识我还是略知一二的。这是我所著的医书《伤寒杂病论》。今赠与你，希望你能刻苦专研，日后定能学有所成。"
        };
    }
}
