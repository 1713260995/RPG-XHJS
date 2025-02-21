using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    [Header("手动赋值")]


    [Header("自动赋值")]
    public Transform healthPos;
    public EnemyCtrl selfCtrl;
    public Image hpImg;
    private float tempHP;
    private float tempSpeed = 0.03f;//血条插值变化速率

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
        // 这里我的角色朝向和UI朝向是相反的，如果直接用LookAt()还需要把每个UI元素旋转过来。
        // 为了简单，用了下面这个方法。它实际上是一个反向旋转，可以简单理解为“负负得正”吧
        transform.rotation = Quaternion.LookRotation(transform.position - m_Camera.position);
    }

}
