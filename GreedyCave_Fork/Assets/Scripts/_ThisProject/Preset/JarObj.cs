using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JarObj : MazeObj, IBootyHolderAble
{
    private MazeGrid grid;//敌人对应的迷宫格子
    private bool isOpen = false;
    public Sprite ashesSprite;
    //private List<ItemInfoAndRatio> itemInfoAndRatioList = new List<ItemInfoAndRatio>();
    private Vector3 openPosition;

    protected void Start()
    {
        MazeGrid grid = MazeGridMgr.GetMazeGird(transform.position);
        if (grid == null)
            return;
        if (grid.gridSpecialInfo == true.ToString())
            isOpen = true;
        CheckSprite();

        if (DefaultBootys == null)
            DefaultBootys = bootys;
    }
    //private void Start()
    //{
    //    MazeGrid grid = MazeGridMgr.GetMazeGird(transform.position);
    //    if (grid == null)
    //        return;
    //    if (grid.gridSpecialInfo == true.ToString())
    //        isOpen = true;
    //    CheckSprite();

    //    if(DefaultBootys == null)
    //        DefaultBootys = bootys;
    //}
    [SerializeField] private List<Booty> bootys = new List<Booty>();
    public static List<Booty> DefaultBootys;
    public static int Medicine_HP_Index = 1;
    public static int Medicine_MP_Index = 2;
    public static int Scroll_BackCity_Index = 3;

    public override string GetSmallMapSpritePath()
    {
        return "Sprite/SmallMapObj/Jar";
    }
    protected override Vector2 CollOffset()
    {
        return new Vector2(0, 0.4f);
    }
    public override bool PresetEventTrigger(IPlayerAble player)
    {
        player.OpenJar(this);
        return base.PresetEventTrigger(player);
    }
    public Booty OpenJar(IPlayerAble playerAble)
    {
        if (isOpen == true)
            return null;
        if (playerAble == null)
            return null;
        isOpen = true;
        openPosition = transform.position;

        MazeGridMgr.SetGridSpecialInfo(transform.position, isOpen.ToString());
        CheckSprite();
        Dictionary<Booty, float> ratioObjDic = RewardLibrary.RatioObjListToDic<Booty>(bootys);

        float allMP = playerAble.GetWarrior().GetCurrentProperty(PropertyType.魔法值上限);
        float currentMP = playerAble.GetWarrior().GetCurrentProperty(PropertyType.魔法值);
        float addRatio_MP = (1 - (currentMP / allMP)) / 10f;
        //Debug.Log("魔法值");
        ratioObjDic = RewardLibrary.AdjustRatio<Booty>(ratioObjDic, bootys[Medicine_MP_Index], addRatio_MP);

        float allHP = playerAble.GetWarrior().GetCurrentProperty(PropertyType.生命值上限);
        float currentHP = playerAble.GetWarrior().GetCurrentProperty(PropertyType.生命值);
        float addRatio_HP = (1 - (currentHP / allHP)) / 6.7f;
        //Debug.Log("生命值");
        ratioObjDic = RewardLibrary.AdjustRatio<Booty>(ratioObjDic, bootys[Medicine_HP_Index], addRatio_HP);

        IHolderAble holder = playerAble as IHolderAble;//下面是对已经拥有回城卷轴的处理
        bool isHaveBackCityScroll = false;
        if (holder != null)
        {
            //这里只是简单的处理，回城卷轴所在Booty里的任意物品玩家拥有时，该booty都出现概率都为0，所以回城卷轴所在Booty里的物品应当只有回城卷轴一个
            HashSet<string> allItemName = Booty.GetAllItemName(bootys[Scroll_BackCity_Index]);//获取回城卷轴所在booty的所有物品的名字
            foreach (string item in allItemName)
            {
                if (holder.GetBag().IsContainItem(item) == true)//如果玩家背包已经有回城卷轴所有booty的任意物品
                {
                    isHaveBackCityScroll = true;
                    break;
                }
            }
        }
        if (isHaveBackCityScroll)
            ratioObjDic = RewardLibrary.AdjustRatio<Booty>(ratioObjDic, bootys[Scroll_BackCity_Index], -1);//设置 - 1的概率则必定为0
        else
        {
            MapInfo mapInfo = GameManager.Instance.GetMapInfo();
            if (mapInfo != null)
            {
                int count = mapInfo.LayerCount % 10;
                count = count == 0 ? 10 : count;//如果是第10层的倍数则返回值为0，重新设为10
                float addRatio_BackCity = (count) / 45.0f;
                if (count != 10)
                    ratioObjDic = RewardLibrary.AdjustRatio<Booty>(ratioObjDic, bootys[Scroll_BackCity_Index], addRatio_BackCity);
                else
                    ratioObjDic = RewardLibrary.AdjustRatio<Booty>(ratioObjDic, bootys[Scroll_BackCity_Index], 1);//第十层必须有回城卷轴，算是保底吧
            }
        }

        Booty booty = RewardLibrary.GetRandomType<Booty>(ratioObjDic);
        AudioMgr.Instance.PlayEffect(AudioClipPath.openJar);

        return booty;
    }
    private void CheckSprite()
    {
        if (isOpen == true)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = ashesSprite;
                smallMapObj.SetActive(false);
            }
        }
        else
            smallMapObj.SetActive(true);
    }
    public Vector3 GetPosition()
    {
        return openPosition;
    }
    public float AdjustReward_Property(float targetValue, ref PropertyType targetType)
    {
        MapInfo mapInfo = GameManager.Instance.GetMapInfo();
        if (mapInfo != null)
            targetValue += mapInfo.LayerCount * targetValue * 0.1f * Random.Range(0.9f, 1.1f);

        return targetValue;
    }


    public static Booty OpenJar_Test(IPlayerAble playerAble)
    {
        if (DefaultBootys == null)
            return null;
        List<Booty> bootys = DefaultBootys;

        Dictionary<Booty, float> ratioObjDic = RewardLibrary.RatioObjListToDic<Booty>(bootys);

        float allMP = playerAble.GetWarrior().GetCurrentProperty(PropertyType.魔法值上限);
        float currentMP = playerAble.GetWarrior().GetCurrentProperty(PropertyType.魔法值);
        float addRatio_MP = (1 - (currentMP / allMP)) / 4;
        ratioObjDic = RewardLibrary.AdjustRatio<Booty>(ratioObjDic, bootys[Medicine_MP_Index], addRatio_MP);

        float allHP = playerAble.GetWarrior().GetCurrentProperty(PropertyType.生命值上限);
        float currentHP = playerAble.GetWarrior().GetCurrentProperty(PropertyType.生命值);
        float addRatio_HP = (1 - (currentHP / allHP)) / 4;
        ratioObjDic = RewardLibrary.AdjustRatio<Booty>(ratioObjDic, bootys[Medicine_HP_Index], addRatio_HP);

        IHolderAble holder = playerAble as IHolderAble;//下面是对已经拥有回城卷轴的处理
        bool isHaveBackCityScroll = false;
        if (holder != null)
        {
            //这里只是简单的处理，回城卷轴所在Booty里的任意物品玩家拥有时，该booty都出现概率都为0，所以回城卷轴所在Booty里的物品应当只有回城卷轴一个
            HashSet<string> allItemName = Booty.GetAllItemName(bootys[Scroll_BackCity_Index]);//获取回城卷轴所在booty的所有物品的名字
            foreach (string item in allItemName)
            {
                if (holder.GetBag().IsContainItem(item) == true)//如果玩家背包已经有回城卷轴所有booty的任意物品
                {
                    isHaveBackCityScroll = true;
                    break;
                }
            }
        }
        if (isHaveBackCityScroll)
            ratioObjDic = RewardLibrary.AdjustRatio<Booty>(ratioObjDic, bootys[Scroll_BackCity_Index], -1);//设置 - 1的概率则必定为0
        else
        {
            MapInfo mapInfo = GameManager.Instance.GetMapInfo();
            if (mapInfo != null)
            {
                int count = mapInfo.LayerCount % 10;
                count = count == 0 ? 10 : count;//如果是第10层的倍数则返回值为0，重新设为10
                float addRatio_BackCity = (count) / 45.0f;
                if (count != 10)
                    ratioObjDic = RewardLibrary.AdjustRatio<Booty>(ratioObjDic, bootys[Scroll_BackCity_Index], addRatio_BackCity);
                else
                    ratioObjDic = RewardLibrary.AdjustRatio<Booty>(ratioObjDic, bootys[Scroll_BackCity_Index], 1);//第十层必须有回城卷轴，算是保底吧
            }
        }

        Booty booty = RewardLibrary.GetRandomType<Booty>(ratioObjDic);
        AudioMgr.Instance.PlayEffect(AudioClipPath.openJar);

        return booty;
    }
}
