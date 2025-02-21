using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    [Header("�Զ���ֵ")]
    public EnemyCtrl enemyCtrl;
    public Animator anim;
    public PlayerCtrl playerCtrl;
    public bool hasCausedDamage = false;//Ϊtrueʱ�Ѿ�����˺����޷�������˺�

    // Start is called before the first frame update
    void Start()
    {
        enemyCtrl = GetComponentInParent<EnemyCtrl>();
        anim = transform.parent.GetChild(0).GetComponent<Animator>();
        playerCtrl = GameManager.instance.player.GetComponent<PlayerCtrl>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }
        if (MyTools.PlayingThisAnim(anim, "normalAttack") && MyTools.CheakCurrentAnimNormalized(anim) >= 0.3f && hasCausedDamage == false)
        {
            playerCtrl.currentHP -= enemyCtrl.skillDamage["normalAttack"];
            hasCausedDamage = true;
        }
        if (!MyTools.PlayingThisAnim(anim, "normalAttack"))
        {
            hasCausedDamage = false;
        }
    }
}
