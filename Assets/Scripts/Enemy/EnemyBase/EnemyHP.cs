using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    [Header("�ֶ���ֵ")]


    [Header("�Զ���ֵ")]
    public Transform healthPos;
    public EnemyCtrl selfCtrl;
    public Image hpImg;
    private float tempHP;
    private float tempSpeed = 0.03f;//Ѫ����ֵ�仯����

    // Start is called before the first frame update
    void Start()
    {
        tempHP = selfCtrl.currentHeath;
        transform.SetParent(GameManager.instance.worldCanvas);
        hpImg = GetComponentInChildren<Image>();
        selfCtrl = healthPos.parent.GetComponent<EnemyCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (healthPos == null || selfCtrl.currentHeath <= 0)
        {
            Destroy(gameObject);
        }
        transform.position = healthPos.position;


        //if (tempHP != selfCtrl.currentHeath)
        //{
        //    tempHP = Mathf.Lerp(tempHP, selfCtrl.currentHeath, tempSpeed);
        //}
        //if (Mathf.Abs(tempHP - selfCtrl.currentHeath) <= 1)
        //{
        //    tempHP = selfCtrl.currentHeath;
        //}
        //hpImg.fillAmount = tempHP / selfCtrl.maxHeath;
        hpImg.fillAmount = LerpFillAmount(selfCtrl.currentHeath, selfCtrl.maxHeath, tempSpeed);
    }

    float LerpFillAmount(float currentVal, float maxVal, float tempSpeed = 0.03f)
    {
        if (tempHP != currentVal)
        {
            tempHP = Mathf.Lerp(tempHP, currentVal, tempSpeed);
        }
        if (Mathf.Abs(tempHP - currentVal) <= 1)
        {
            tempHP = currentVal;
        }
        return tempHP / maxVal;
    }


    void LateUpdate()
    {
        var m_Camera = Camera.main.transform;
        if (m_Camera == null)
        {
            return;
        }
        // �����ҵĽ�ɫ�����UI�������෴�ģ����ֱ����LookAt()����Ҫ��ÿ��UIԪ����ת������
        // Ϊ�˼򵥣��������������������ʵ������һ��������ת�����Լ����Ϊ��������������
        transform.rotation = Quaternion.LookRotation(transform.position - m_Camera.position);
    }

}
