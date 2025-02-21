using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{

    [Header("�ֶ���ֵ")]
    public float moveSpeed;//�ƶ��ٶ�
    public List<Image> skillImg = new List<Image>();//����ͼ��
    public List<Text> skillCoolingTxt = new List<Text>();//������ȴʱ����ʾ
    public List<float> skillDamage = new List<float>() { 25, 50, 75, 25 };//�����˺�
    public List<float> skillCoolingTime = new List<float>() { 0.9f, 2, 3, 10, 2, 2 };//������ȴ����
    public float rotateSpeed;//������ת�ٶ�
    public float maxHP;
    public float maxMP;
    public Image _hpImg;
    public Image _mpImg;

    [Header("�Զ���ֵ")]
    public bool isStartSkill = false;//�Ƿ��ڼ����ͷ�״̬
    private NavMeshAgent agent;
    private List<float> skillCoolingTimeRecord = new List<float> { 0, 0, 0, 0, 0, 0 };
    public float currentHP;
    public float currentMP;
    public float tempHP;
    public float tempMP;

    // Start is called before the first frame update
    private void Awake()
    {
        currentHP = maxHP;
        currentMP = maxMP;
        tempHP = currentHP;
        tempMP = currentMP;
        GameManager.instance.player = transform;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        SetPlayerState();
        if (isStartSkill == false)
        {
            Move();
            ReleaseSkills();
        }
        SetSkillUI();
        LookAtMousePoint();

    }

    /// <summary>
    /// ������ҵ�״ֵ̬
    /// </summary>
    void SetPlayerState()
    {
        tempHP = LerpFillAmount(tempHP, currentHP, maxHP);
        _hpImg.fillAmount = (maxHP - tempHP) / maxHP;
        tempMP = LerpFillAmount(tempMP, currentMP, maxMP);
        _mpImg.fillAmount = (maxMP - tempMP) / maxMP;

        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        currentMP = Mathf.Clamp(currentMP, 0, maxMP);
    }

    float LerpFillAmount(float tempVal, float currentVal, float maxVal, float tempSpeed = 0.03f)
    {
        if (tempVal != currentVal)
        {
            tempVal = Mathf.Lerp(tempVal, currentVal, tempSpeed);
        }
        if (Mathf.Abs(tempVal - currentVal) <= 1)
        {
            tempVal = currentVal;
        }
        return tempVal;
    }

    /// <summary>
    /// ʹ��ҳ������������λ��
    /// </summary>
    void LookAtMousePoint()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(myRay, out hit))
            {
                transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
            }
        }
    }

    /// <summary>
    /// ���ü���UI�������ʾ
    /// </summary>
    private void SetSkillUI()
    {
        for (int i = 0; i < skillCoolingTimeRecord.Count; i++)
        {
            Image img = skillImg[i];
            Text text = skillCoolingTxt[i];
            if (skillCoolingTimeRecord[i] > 0)
            {
                skillCoolingTimeRecord[i] -= Time.deltaTime;
                img.fillAmount = skillCoolingTimeRecord[i] / skillCoolingTime[i];
                if (text.gameObject.activeInHierarchy != true)
                {
                    text.gameObject.SetActive(true);
                }
                text.color = Color.white;
                float val = skillCoolingTimeRecord[i];
                text.text = Math.Round(val, 1).ToString();
            }
            else
            {
                if (text.gameObject.activeInHierarchy == true)
                {
                    text.gameObject.SetActive(false);
                }
                img.fillAmount = 1;
            }
        }
    }

    /// <summary>
    /// �ͷż���
    /// </summary>
    void ReleaseSkills()
    {
        if ((Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1)) && skillCoolingTimeRecord[0] <= 0)
        {
            StartSkill("skill1");
        }
        if ((Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2)) && skillCoolingTimeRecord[1] <= 0)
        {
            StartSkill("skill2");
        }
        if ((Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Keypad3)) && skillCoolingTimeRecord[2] <= 0)
        {
            StartSkill("skill3");
        }
        if ((Input.GetKey(KeyCode.Alpha4) || Input.GetKey(KeyCode.Keypad4)) && skillCoolingTimeRecord[3] <= 0)
        {
            StartSkill("skill4");
        }
    }

    /// <summary>
    /// �����ͷſ�ʼ
    /// </summary>
    /// <param name="skillName"></param>
    private void StartSkill(string skillName)
    {
        isStartSkill = true;
        GetComponentInChildren<Animator>().SetBool("run", false);
        GetComponentInChildren<Animator>().SetBool("skilling", true);
        GetComponentInChildren<Animator>().SetTrigger(skillName);
    }

    /// <summary>
    /// ���������ƶ�
    /// </summary>
    void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * 3);
            GetComponentInChildren<Animator>().SetBool("run", true);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * -3);
            GetComponentInChildren<Animator>().SetBool("run", true);
        }
        if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            GetComponentInChildren<Animator>().SetBool("run", false);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up * -1 * Time.deltaTime * rotateSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up * 1 * Time.deltaTime * rotateSpeed);
        }
    }


    /// <summary>
    /// ���ܽ�����ȴʱ��
    /// </summary>
    /// <param name="index"></param>
    public void EnterCoolingTime(int index)
    {
        skillCoolingTimeRecord[index] = skillCoolingTime[index];
    }


    /// <summary>
    /// ʹ�����Զ�Ѱ·
    /// </summary>
    void MoveByAgent()
    {
        var tarPoint = MyTools.GetPonitByMouse(1);
        if (tarPoint != Vector3.zero)
        {
            agent.SetDestination(tarPoint);

            // transform.position = point;
        }
        if (agent.remainingDistance < 0.5f)
        {
            GetComponentInChildren<Animator>().SetBool("run", false);
        }
        else
        {
            GetComponentInChildren<Animator>().SetBool("run", true);
        }
    }


}
