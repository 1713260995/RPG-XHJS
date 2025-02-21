using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WolfManCtrl : EnemyCtrl
{
    [Header("�ֶ���ֵ")]
    public float thumpCoolTime = 3;//�ػ���ȴʱ������
    public float thumpRange = 5;//�ػ����ܷ�Χ
    public float thumpMoveSpeed;//�ͷ��ػ�ʱ������ٶ�
    public float thumpDamage = 250;

    [Header("�Զ���ֵ")]
    public float thumpCoolTimeRecord;//������ͨ������ǰ��ȴʱ��
    public Vector3 thumpTargetPos;


    public override void Start()
    {
        base.Start();
        thumpCoolTimeRecord = 0;
        skillDamage.Add("thump", 250);
        GameManager.instance.enemyPosList.Add(new EnemyPos { id = enemyId, t = transform, enemyType = EnemyType.WolfMan });
    }

    public override void Update()
    {
        base.Update();
        if (currentHeath <= 0)
        {
            GameManager.instance.wolfManIsDie = true;
        }
    }


    /// <summary>
    /// �ͷ��ػ�����
    /// </summary>
    void Thump()
    {
        if (thumpCoolTimeRecord <= 0)
        {
            thumpCoolTimeRecord = thumpCoolTime;
            enemyAnim.SetTrigger("thump");
            thumpTargetPos = player.position;
        }
        if (MyTools.PlayingThisAnim(enemyAnim, "thump"))
        {
            if (MyTools.CheakCurrentAnimNormalized(enemyAnim) <= 0.3f)
            {//�������е�ǰ30%����ǰ������Ծ
                Vector3 pos = Vector3.zero;
                float dis = Vector3.Distance(transform.position, thumpTargetPos);
                if (dis <= 3)
                {
                    pos = Vector3.up;
                }
                else
                {
                    pos = Vector3.up + Vector3.forward;
                }
                transform.Translate(pos * Time.deltaTime * thumpMoveSpeed);
            }
            if (MyTools.CheakCurrentAnimNormalized(enemyAnim) >= 0.3f && !isGround)
            {//������70%�������û�Ӵ�������ͽ������䲢����ǰ��
                transform.Translate(new Vector3(0, -1, 0.05f) * Time.deltaTime * thumpMoveSpeed);
            }
        }
    }


    /// <summary>
    /// ��д����ģʽ�µ�ai
    /// </summary>
    public override void AttackMode()
    {
        var isMoveRange = CheckDistanceWithPlayer(observationRange);
        var isThumpRange = CheckDistanceWithPlayer(thumpRange);
        var isAttackRange = CheckDistanceWithPlayer(attackRange);

        if (isThumpRange && !MyTools.PlayingThisAnim(enemyAnim, "normalAttack"))
        {
            Thump();
        }
        if (isAttackRange && !MyTools.PlayingThisAnim(enemyAnim, "thump"))
        {
            NormalAttack();
        }
        if (isMoveRange && !MyTools.PlayingThisAnim(enemyAnim, "thump") && !MyTools.PlayingThisAnim(enemyAnim, "normalAttack"))
        {
            ChasePlayer();
        }
        else
        {
            IdleMode();
        }
        thumpCoolTimeRecord = LowerCoolTime(thumpCoolTimeRecord);
        attackCoolTimeRecord = LowerCoolTime(attackCoolTimeRecord);
    }


}
