using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WolfManSkillManager : MonoBehaviour
{
    [Header("手动赋值")]
    public Transform thumpStartPos;
    public float thumpSkillDestoryTime = 1.5f;

    [Header("自动赋值")]
    public float thumpDamage;
    public PlayerCtrl playerCtrl;
    public WolfManCtrl wolfCtrl;

    private void Start()
    {
        wolfCtrl = transform.GetComponentInParent<WolfManCtrl>();
        thumpDamage = wolfCtrl.thumpDamage;
        playerCtrl = GameManager.instance.player.GetComponent<PlayerCtrl>();
    }

    void ThumpRelease()
    {
        SkillBase skill = SkillManager.ReleaseAoeSkill("Skill/WolfMan/WolfManThump", thumpStartPos.position, thumpSkillDestoryTime);
        skill.triggerEvent = (other) =>
        {
            if (other.tag == "Player")
            {
                playerCtrl.currentHP -= wolfCtrl.skillDamage["thump"];
            }
        };
    }

}
