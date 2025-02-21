using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    [Header("�ֶ���ֵ")]
    public float observationRange = 20f;//������Ұ��Χ
    public float attackRange = 2.5f;//���﹥����Χ
    public float attackCoolTime = 3;//��ͨ������ȴʱ������
    public float moveSpeed = 5;//�ƶ��ٶ�
    public float minDistanceWithPlayer = 2.5f;//�˾������ڣ���������׷�����
    public float maxHeath = 100;//����ֵ����
    public float destroyTime = 3;//ʬ������ʱ��
    public Transform healthPos;
    public Dictionary<string, float> skillDamage = new Dictionary<string, float>();//���������˺�ֵ

    [Header("�Զ���ֵ")]
    public float attackCoolTimeRecord;//������ͨ������ǰ��ȴʱ��
    public Animator enemyAnim;//���ﶯ��������
    public Transform player;//���
    public EnemyAI ai;
    public bool isGround;//�Ƿ�Ӵ�������
    public float distanceWithPlayer;//����ҵľ���
    public float currentHeath;//��ǰ����ֵ
    public Transform enemyListParent;//���й���ĸ�����
    public int enemyId;//����id

    public virtual void Start()
    {
        enemyListParent = GameManager.instance.enemyListParent;
        transform.SetParent(enemyListParent);
        skillDamage.Add("normalAttack", 100);
        currentHeath = maxHeath;
        GenerateHP();
        isGround = false;
        enemyAnim = transform.GetChild(0).GetComponent<Animator>();
        player = GameManager.instance.player;
        enemyId = MyTools.GenerateIntID(8);
    }


    public virtual void Update()
    {

        currentHeath = Mathf.Clamp(currentHeath, 0, maxHeath);
        distanceWithPlayer = Vector3.Distance(transform.position, player.position);
        if (currentHeath <= 0)
        {
            ai = EnemyAI.die;
        }
        EnemyAIMode();
    }

    private void OnDestroy()
    {
        var m = GameManager.instance.enemyPosList.Where(o => o.id == enemyId).FirstOrDefault();
        if (m!=null&&m.iconPos!=null)
        {
            Destroy(m.iconPos.gameObject);
        }
        GameManager.instance.enemyPosList.Remove(m);
    }

    /// <summary>
    /// ���ù����aiģʽ
    /// </summary>
    public virtual void EnemyAIMode()
    {
        switch (ai)
        {
            case EnemyAI.idle:
                IdleMode();
                break;
            case EnemyAI.attack:
                AttackMode();
                break;
            case EnemyAI.die:
                DieMode();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ����״̬
    /// </summary>
    public virtual void DieMode()
    {
        if (!MyTools.PlayingThisAnim(enemyAnim, "die"))
        {
            enemyAnim.SetTrigger("die");
            Destroy(gameObject, destroyTime);
        }
    }

    /// <summary>
    /// ����״̬
    /// </summary>
    public virtual void AttackMode()
    {
        var isMoveRange = CheckDistanceWithPlayer(observationRange);
        var isAttackRange = CheckDistanceWithPlayer(attackRange);
        if (isAttackRange)
        {
            NormalAttack();
        }
        if (isMoveRange && !MyTools.PlayingThisAnim(enemyAnim, "normalAttack"))
        {
            ChasePlayer();
        }
        else
        {
            IdleMode();
        }
        attackCoolTimeRecord = LowerCoolTime(attackCoolTimeRecord);
    }

    /// <summary>
    /// ��Ϣ״̬
    /// </summary>
    public virtual void IdleMode()
    {
        enemyAnim.SetBool("run", false);
    }

    /// <summary>
    /// ��ͨ����
    /// </summary>
    public void NormalAttack()
    {
        if (attackCoolTimeRecord <= 0)
        {//���������ȴ�����ͷ�������
            enemyAnim.SetTrigger("attack");
            attackCoolTimeRecord = attackCoolTime;
        }
    }

    /// <summary>
    /// �������ڹ�����Ұ�ھ�׷�����
    /// </summary>
    public virtual void ChasePlayer()
    {
        if (CheckDistanceWithPlayer(minDistanceWithPlayer))
        {
            enemyAnim.SetBool("run", false);
            transform.LookAt(player);
        }
        else
        {
            enemyAnim.SetBool("run", true);
            MyTools.FindTarget(transform, player, moveSpeed);
        }
    }

    /// <summary>
    /// ���������ҵľ���
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckDistanceWithPlayer(float distance)
    {
        if (player != null)
        {
            return MyTools.CheckDistanceIsContain(transform, player, distance);
        }
        return false;
    }

    /// <summary>
    /// �������δ��ȴ��ɣ��ͼ��ټ�����ȴ
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public float LowerCoolTime(float time)
    {
        if (time >= 0)
        {
            return time -= Time.deltaTime;
        }
        return time;
    }

    /// <summary>
    /// ����Ѫ��
    /// </summary>
    public void GenerateHP()
    {
        GameObject obj = MyTools.LoadPrefab("Enemy/StatusUI/HealthValue", Vector3.zero);
        var hp = obj.GetComponent<EnemyHP>();
        hp.selfCtrl = this;
        hp.healthPos = healthPos;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (MyTools.IsEqualLayer(collision.gameObject, "Ground"))
        {//�жϵ�ǰ�����Ƿ��ڵ���
            isGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (MyTools.IsEqualLayer(collision.gameObject, "Ground"))
        {//�жϵ�ǰ�����Ƿ��ڵ���
            isGround = false;
        }
    }

}
