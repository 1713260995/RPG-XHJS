using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �Ի���
/// </summary>
public class DialogFrame : MonoBehaviour
{

    [Header("�ֶ���ֵ")]
    public Text dialogTxt;
    public MonologueFrame monologueFrame;//���׿�

    [SerializeField]
    [Header("�Զ���ֵ")]
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
    /// �Ի�1������
    /// </summary>
    List<string> Dialog1Content()
    {
        return new List<string> {
        "���������ˡ�",
        "����˭�������ģ�",
        "�ҽ����پ�����һλҽʦ��������������",
        "������Ϊʲô�������",
        "�ղ��ҵ����Ѹ���˵���⸽����һλ���Ե�Ů�ӣ����Ҿ�������ش��˾�����ɡ������Ҹ����㣬������ʦ���������������������ġ�" +
        "������ί���ң����ҽ�����ҽ��ϣ������Ǳ��ѧϰ���պ�������ҽ֪ʶ�������ա�",
        "�ţ����Ǽǵ�ʦ��֮ǰ����˵��Ҫ��ѧϰ��ҽ��",
        "�����ʶǳ������������ҽ��֪ʶ�һ�����֪һ���ġ�������������ҽ�顶�˺��Ӳ��ۡ����������㣬ϣ�����ܿ̿�ר�У��պ���ѧ�����ɡ�"
        };
    }
}
