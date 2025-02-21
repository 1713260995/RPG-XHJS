using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minin01Ctrl : EnemyCtrl
{
    public override void Start()
    {
        base.Start();
        GameManager.instance.enemyPosList.Add(new EnemyPos { id = enemyId, t = transform, enemyType = EnemyType.Minin01 });
    }

    public override void Update()
    {
        base.Update();
    }
}
