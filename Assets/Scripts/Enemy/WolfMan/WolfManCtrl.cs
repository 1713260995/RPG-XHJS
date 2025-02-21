using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WolfManCtrl : EnemyCtrl
{
    [Header("手动赋值")]
    public float thumpCoolTime = 3;//重击冷却时间设置
    public float thumpRange = 5;//重击技能范围
    public float thumpMoveSpeed;//释放重击时身体的速度
    public float thumpDamage = 250;

    [Header("自动赋值")]
    public float thumpCoolTimeRecord;//怪物普通攻击当前冷却时间
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
    /// 释放重击技能
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
            {//动画进行的前30%进行前进并跳跃
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
            {//动画后70%如果怪物没接触到地面就进行下落并缓慢前进
                transform.Translate(new Vector3(0, -1, 0.05f) * Time.deltaTime * thumpMoveSpeed);
            }
        }
    }


    /// <summary>
    /// 重写攻击模式下的ai
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
