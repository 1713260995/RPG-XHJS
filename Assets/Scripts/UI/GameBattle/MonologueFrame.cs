using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonologueFrame : MonoBehaviour
{
    [Header("手动赋值")]
    public Text monologue;

    [SerializeField]
    [Header("自动赋值")]
    private int mouseDownCount = 1;
    private List<string> contentList;
    public PlayerCtrl playerCtrl;


    // Start is called before the first frame update
    void Start()
    {
        contentList = MonologueContent();
        playerCtrl = GameManager.instance.player.GetComponent<PlayerCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        Monologue1();
    }

    /// <summary>
    /// 执行独白1
    /// </summary>
    private void Monologue1()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (mouseDownCount == contentList.Count)
            {
                gameObject.SetActive(false);
                playerCtrl.enabled = true;
                return;
            }
            monologue.text = contentList[mouseDownCount];
            mouseDownCount++;
        }
    }

    /// <summary>
    /// 独白1的内容
    /// </summary>
    List<string> MonologueContent()
    {
        return new List<string> {
       "你打开了《伤寒杂病论》。深刻钻研后，发现这本书中涵盖了诸多疾病的治疗方法和预防措施。其中，主要内容包括以下几个方面：\r\n1.疾病分类：该书对疾病进行了系统分类，根据病因、病位、症状等方面进行了详细的归类和分析。\r\n\r\n" +
       "2.病因病机：该书强调病因的多样性和复杂性，从气、血、津液、脏腑、经络等方面对病因进行了深入探讨，并提出了“风寒、夏温、湿热、燥热”等六淫和“七情”等内因是导致疾病发生的主要原因。",
       "3.诊断治疗：该书提出了“辨证论治”和“因病施治”的原则，即根据病情的具体表现和个体差异进行诊断和治疗，强调个性化治疗和针对症状进行治疗。同时，该书也对针灸、草药、饮食等方面的治疗方法进行了详细阐述。\r\n\r\n" +
       "4.预防保健：该书提出了许多有关预防疾病和保护健康的方法，包括饮食起居、情志调摄等方面的建议。",
       "看完之后你感悟颇深，结合师傅教给你的仙法，相互融合。因此习得技能：\r\n1.“琼花飞影”——数字1键。" +
       "\r\n运用琼花之力向前发出三发光弹，造成25伤害。\r\n\r\n2.“电闪雷鸣”——数字2键。\r\n向前发射一束闪电，造成50伤害。",
       "3.“水元劲”——数字3键\r\n运用水的力量和阿尔法射线向前发出一道光波，能量巨大，造成75点伤害。\r\n\r\n" +
       "4.“五光寒阴绝”——数字4键\r\n能够将极寒之力，仙法融合于自身的神通。向四周爆发出4道冰墙，每次造成25伤害，持续3次。\r\n",
       "习得技能后，你到附近的一座村庄四处闲逛。发现没什么人，随后你问到一阵妖气。原来这里已经被妖怪占领。看来是时候运用你的技能，战胜妖怪了。",
       "任务：\r\n清理妖怪，并战胜boss——恶狼首领\r\n\r\n操作：\r\n前进——W键\r\n后退——S键\r\n向左旋转——A键\r\n向右旋转——D键\r\n瞬间旋转至指定方向——鼠标左键",
        };
    }
}
