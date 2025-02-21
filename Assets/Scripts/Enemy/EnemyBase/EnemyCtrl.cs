using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    [Header("手动赋值")]
    public float observationRange = 20f;//怪物视野范围
    public float attackRange = 2.5f;//怪物攻击范围
    public float attackCoolTime = 3;//普通攻击冷却时间设置
    public float moveSpeed = 5;//移动速度
    public float minDistanceWithPlayer = 2.5f;//此距离以内，怪物无需追逐玩家
    public float maxHeath = 100;//生命值上限
    public float destroyTime = 3;//尸体销毁时间
    public Transform healthPos;
    public Dictionary<string, float> skillDamage = new Dictionary<string, float>();//动画名，伤害值

    [Header("自动赋值")]
    public float attackCoolTimeRecord;//怪物普通攻击当前冷却时间
    public Animator enemyAnim;//怪物动画控制器
    public Transform player;//玩家
    public EnemyAI ai;
    public bool isGround;//是否接触到地面
    public float distanceWithPlayer;//与玩家的距离
    public float currentHeath;//当前生命值
    public Transform enemyListParent;//所有怪物的父对象
    public int enemyId;//怪物id

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
    /// 设置怪物的ai模式
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
    /// 死亡状态
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
    /// 进攻状态
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
    /// 休息状态
    /// </summary>
    public virtual void IdleMode()
    {
        enemyAnim.SetBool("run", false);
    }

    /// <summary>
    /// 普通攻击
    /// </summary>
    public void NormalAttack()
    {
        if (attackCoolTimeRecord <= 0)
        {//如果攻击冷却结束就发动攻击
            enemyAnim.SetTrigger("attack");
            attackCoolTimeRecord = attackCoolTime;
        }
    }

    /// <summary>
    /// 如果玩家在怪物视野内就追逐玩家
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
    /// 检测怪物和玩家的距离
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
    /// 如果技能未冷却完成，就减少技能冷却
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
    /// 生成血条
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
        {//判断当前物体是否在地面
            isGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (MyTools.IsEqualLayer(collision.gameObject, "Ground"))
        {//判断当前物体是否在地面
            isGround = false;
        }
    }

}
