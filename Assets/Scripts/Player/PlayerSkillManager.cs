using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public float skillSpeed;
    public int skill4Count;
    private PlayerCtrl playerCtrl;

    public Transform SkillParent;
    public Transform SkillStartPos;
    // Start is called before the first frame update
    void Start()
    {
        playerCtrl = GetComponentInParent<PlayerCtrl>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SkillStart()
    {

    }

    void attack1()
    {
        playerCtrl.EnterCoolingTime(0);
        Skilling();
        ReleaseRemoteSkill("Skill1", playerCtrl.skillDamage[0]);
    }

    void attack2()
    {
        playerCtrl.EnterCoolingTime(1);
        Skilling();
        ReleaseRemoteSkill("Skill2", playerCtrl.skillDamage[1]);
    }
    void attack3()
    {
        playerCtrl.EnterCoolingTime(2);
        Skilling();
        ReleaseRemoteSkill("Skill3", playerCtrl.skillDamage[2]);
    }
    void attack4()
    {
        playerCtrl.EnterCoolingTime(3);
        Skilling();
        StartCoroutine(skill4(5, 1, playerCtrl.skillDamage[3]));
        StartCoroutine(skill4(10, 1.5f, playerCtrl.skillDamage[3]));
    }

    /// <summary>
    /// 表示当前技能已经释放
    /// </summary>
    private void Skilling()
    {
        playerCtrl.isStartSkill = false;
        GetComponentInChildren<Animator>().SetBool("skilling", false);
    }

    /// <summary>
    /// 释放大招-4技能
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="size"></param>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    IEnumerator skill4(float distance, float size, float skillDamage)
    {
        yield return null;
        for (int i = 0; i < skill4Count; i++)
        {
            var pos = transform.position;
            var skill = MyTools.LoadPrefab($"Skill/Player/Skill4", pos);
            skill.transform.SetParent(SkillParent);
            skill.transform.localScale = skill.transform.localScale * size;
            SkillBase skillBase = skill.GetComponent<SkillBase>();
            skillBase.triggerEvent = (collider) =>
            {
                if (collider.tag == "Enemy")
                {
                    EnemyCtrl enemyCtrl = collider.GetComponent<EnemyCtrl>();
                    enemyCtrl.currentHeath -= skillDamage;
                }
            };
            var t = skill.transform;
            for (int j = 0; j < t.childCount; j++)
            {
                if (j == 0)
                {
                    var child = t.GetChild(j);
                    child.position = new Vector3(pos.x + distance, 1, pos.z);
                }
                if (j == 1)
                {
                    var child = t.GetChild(j);
                    child.position = new Vector3(pos.x, 1, pos.z + distance);
                }
                if (j == 2)
                {
                    var child = t.GetChild(j);
                    child.position = new Vector3(pos.x - distance, 1, pos.z);
                }
                if (j == 3)
                {
                    var child = t.GetChild(j);
                    child.position = new Vector3(pos.x, 1, pos.z - distance);
                }
            }
            Destroy(skill, 1);
            yield return new WaitForSeconds(1.5f);
        }
    }

    /// <summary>
    /// 发射远程技能
    /// </summary>
    /// <param name="name"></param>
    /// <param name="destroy"></param>
    void ReleaseRemoteSkill(string name, float skillDamage, float destroy = 5)
    {
        SkillBase skillBase = SkillManager.ReleaseRemoteSkill($"Skill/Player/{name}", transform.parent.localEulerAngles, SkillStartPos.position, skillSpeed, destroy);

        skillBase.triggerEvent = (collider) =>
        {
            if (collider.tag == "Enemy")
            {
                EnemyCtrl enemyCtrl = collider.GetComponent<EnemyCtrl>();
                enemyCtrl.currentHeath -= skillDamage;
                print(enemyCtrl.name + "的生命值还剩" + enemyCtrl.currentHeath);
                Destroy(skillBase.gameObject);
            }
        };
    }
}
