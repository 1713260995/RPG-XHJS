using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SkillManager
{

    /// <summary>
    /// 发射远程技能
    /// </summary>
    /// <param name="prefabName">预制件名称</param>
    /// <param name="direction">方向</param>
    /// <param name="startPos">起始位置</param>
    /// <param name="skillSpeed">发射速度</param>
    /// <param name="destroy">销毁时间</param>
    public static SkillBase ReleaseRemoteSkill(string prefabName, Vector3 direction, Vector3 startPos, float skillSpeed, float destroy = 5)
    {
        var skill = MyTools.LoadPrefab(prefabName, startPos);
        skill.transform.SetParent(GameManager.instance.SkillParent);
        SkillBase skillBase = skill.GetComponent<SkillBase>();
        GameObject.Destroy(skill, destroy);
        var forword = Vector3.forward;
        skill.transform.localEulerAngles = direction;
        skillBase.updateEvent = () => { skill.transform.Translate(forword * Time.deltaTime * skillSpeed, Space.Self); };
        return skillBase;
    }

    /// <summary>
    /// 释放范围伤害技能
    /// </summary>
    public static SkillBase ReleaseAoeSkill(string prefabName, Vector3 startPos, float destroy = 5)
    {
        var skill = MyTools.LoadPrefab(prefabName, startPos);
        skill.transform.SetParent(GameManager.instance.SkillParent);
        SkillBase skillBase = skill.GetComponent<SkillBase>();
        GameObject.Destroy(skill, destroy);
        return skillBase;
    }
}