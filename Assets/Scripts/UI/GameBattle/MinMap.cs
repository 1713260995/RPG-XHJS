using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinMap : MonoBehaviour
{
    [Header("手动赋值")]
    public Image playerIcon;



    [Header("自动赋值")]
    public List<Image> images = new List<Image>();
    public Transform player;
    List<EnemyPos> enemyPosList = new List<EnemyPos>();
    Dictionary<EnemyType, Sprite> sprites = new Dictionary<EnemyType, Sprite>();
    string[] spritePaths = new string[] { EnemyType.Minin01.ToString(), EnemyType.WolfMan.ToString() };

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.player;
        GameManager.instance.enemyPosList = enemyPosList;
        LoadSprite();
    }

    // Update is called once per frame
    void Update()
    {
        playerIcon.transform.localEulerAngles = new Vector3(0, 0, (player.localEulerAngles.y + 90) * -1);
        SetEnemyIconPos();
    }


    /// <summary>
    /// 设置怪物的位置
    /// </summary>
    void SetEnemyIconPos()
    {
        for (int i = 0; i < enemyPosList.Count; i++)
        {
            var item = enemyPosList[i];
            CreateIcon(item);
            Vector2 origin = new Vector2(player.position.x, player.position.z);
            Vector2 target = new Vector2(item.pos.x, item.pos.z);
            Vector2 pos = MyTools.GetEndPointByDistance(origin, target, Vector2.zero, dis => Mathf.Clamp(dis, 0, 50));
            item.iconPos.rectTransform.anchoredPosition3D = new Vector2(pos.x * -1, pos.y * -1);
        }
    }

    /// <summary>
    /// 如果第一次生成怪物就生成对应地图坐标
    /// </summary>
    /// <param name="item"></param>
    private void CreateIcon(EnemyPos item)
    {
        if (item.iconPos == null)
        {
            GameObject obj = new GameObject(item.enemyType.ToString() + "-" + item.id);
            obj.transform.SetParent(transform);
            var rect = obj.AddComponent<RectTransform>();
            var img = obj.AddComponent<Image>();
            rect.sizeDelta = new Vector2(15, 15);
            if (item.enemyType == EnemyType.WolfMan)
            {
                img.sprite = sprites[EnemyType.WolfMan];
            }
            else
            {
                img.sprite = sprites[EnemyType.Minin01];
            }
            item.iconPos = img;
        }
    }

    /// <summary>
    /// 加载图片
    /// </summary>
    void LoadSprite()
    {
        for (int i = 0; i < spritePaths.Length; i++)
        {
            Sprite sprite = Resources.Load<Sprite>("UI/MinMap/" + spritePaths[i]);
            sprite = Instantiate(sprite);
            EnemyType type = (EnemyType)Enum.Parse(typeof(EnemyType), spritePaths[i]);
            sprites.Add(type, sprite);
        }
    }
}


public class EnemyPos
{
    public int id;
    public Transform t;
    public Vector3 pos { get => t.position; set { } }
    public Image iconPos = null;
    public EnemyType enemyType;
}