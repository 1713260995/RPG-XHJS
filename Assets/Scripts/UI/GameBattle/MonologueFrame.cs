using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonologueFrame : MonoBehaviour
{
    [Header("�ֶ���ֵ")]
    public Text monologue;

    [SerializeField]
    [Header("�Զ���ֵ")]
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
    /// ִ�ж���1
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
    /// ����1������
    /// </summary>
    List<string> MonologueContent()
    {
        return new List<string> {
       "����ˡ��˺��Ӳ��ۡ���������к󣬷����Ȿ���к�������༲�������Ʒ�����Ԥ����ʩ�����У���Ҫ���ݰ������¼������棺\r\n1.�������ࣺ����Լ���������ϵͳ���࣬���ݲ��򡢲�λ��֢״�ȷ����������ϸ�Ĺ���ͷ�����\r\n\r\n" +
       "2.���򲡻�������ǿ������Ķ����Ժ͸����ԣ�������Ѫ����Һ���อ������ȷ���Բ������������̽�֣�������ˡ��纮�����¡�ʪ�ȡ����ȡ��������͡����顱�������ǵ��¼�����������Ҫԭ��",
       "3.������ƣ���������ˡ���֤���Ρ��͡���ʩ�Ρ���ԭ�򣬼����ݲ���ľ�����ֺ͸�����������Ϻ����ƣ�ǿ�����Ի����ƺ����֢״�������ơ�ͬʱ������Ҳ����ġ���ҩ����ʳ�ȷ�������Ʒ�����������ϸ������\r\n\r\n" +
       "4.Ԥ���������������������й�Ԥ�������ͱ��������ķ�����������ʳ��ӡ���־����ȷ���Ľ��顣",
       "����֮�������������ʦ���̸�����ɷ����໥�ںϡ����ϰ�ü��ܣ�\r\n1.������Ӱ����������1����" +
       "\r\n������֮����ǰ���������ⵯ�����25�˺���\r\n\r\n2.��������������������2����\r\n��ǰ����һ�����磬���50�˺���",
       "3.��ˮԪ������������3��\r\n����ˮ�������Ͱ�����������ǰ����һ���Ⲩ�������޴����75���˺���\r\n\r\n" +
       "4.����⺮��������������4��\r\n�ܹ�������֮�����ɷ��ں����������ͨ�������ܱ�����4����ǽ��ÿ�����25�˺�������3�Ρ�\r\n",
       "ϰ�ü��ܺ��㵽������һ����ׯ�Ĵ��й䡣����ûʲô�ˣ�������ʵ�һ��������ԭ�������Ѿ�������ռ�졣������ʱ��������ļ��ܣ�սʤ�����ˡ�",
       "����\r\n�������֣���սʤboss������������\r\n\r\n������\r\nǰ������W��\r\n���ˡ���S��\r\n������ת����A��\r\n������ת����D��\r\n˲����ת��ָ�����򡪡�������",
        };
    }
}
