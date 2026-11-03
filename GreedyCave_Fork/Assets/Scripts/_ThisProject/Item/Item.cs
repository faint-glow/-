using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ItemQuality
{
    //只能按品质从低到高排列
    White,
    Green,
    Blue,
    Purple,
    Orange,
    Red,

    无,
}
public enum PropType
{
    Medicine,
    Material,
    Scroll,
    Containter,
    Food,
}
public enum ItemType
{
    CanCombine,//可以合并
    CannotCombine,//不可以合并

    无,
}

public enum ItemCategory
{
    Equip,
    Medicine,
    Food,
    Material,
    Sundary,
    无,
}
[System.Serializable]
public class ItemSaveData
{
    public string itemName;
    public ItemQuality itemQuality;
    public int count;
    public int level;
    public bool isLocked;
    //public string itemType;//用于区分物品类型
    //武器
    public Dictionary<PropertyType, float> strengthenProperty;
    public Dictionary<PropertyType, float> extraProperty;
    public List<string> equipRandomBuff;
    public int strengthenLevel = 0;
    public int price;
    //药剂

    //强化材料

    //容器
    public BagSaveData bagSaveData;

    public Item GetItem()
    {
        if(!Item.AllItemInfoDic.ContainsKey(itemName))
        {
            Debug.Log("全部物品索引中缺失:" + itemName + "。请检查路径");
            return null;
        }
        Item item = Item.AllItemInfoDic[itemName].GetItem(itemQuality, level);
        item.LoadItemSaveData(this);
        return item;
    }
}

//子类的类名必须是ItemType的类型，这样可以更自动
public abstract class Item : ICollectAble<Item>, IGetAble, IDragAble,IShowSpriteAble
{
    public Item(ItemInfo itemInfo, ItemQuality itemQuality, int level = 1)
    {
        if (itemInfo == null)
        {
            Debug.Log(itemInfo + "为空");
            return;
        }
        this.itemInfo = itemInfo;
        this.itemName = itemInfo.itemName;
        this.isNeverDrop = itemInfo.isNeverDrop;
        this.itemSprite = itemInfo.itemSprite;
        this.itemBaseInfo = itemInfo.itemBaseInfo;
        this.itemExtraInfo = itemInfo.itemExtraInfo;
        this.specialType = itemInfo.GetSpecialType();
        this.itemQuality = itemQuality;
        this.basePrice = itemInfo.basePrice;
        this.count = 1;
        
        this.level = level;
        this.price = CreatePrice();
        this.SetDetailButtons();
    }
    public Item(Item item, int count) : this(item.itemInfo, item.itemQuality, count)
    {
        this.count = count;
        this.holder = item.holder;
        this.level = item.level;
        this.price = item.price;
        this.IsLocked = item.IsLocked;
    }

    private static List<string> paths = new List<string>() 
    {
        "Item",
        //"Map/MapOne/Equip",
    };
    private static Dictionary<string, ItemInfo> allItemInfoDic;
    public static Dictionary<string, ItemInfo> AllItemInfoDic
    {
        get 
        {
            if (allItemInfoDic == null)
                allItemInfoDic = ToolsMgr.LoadAllResourceInfo<ItemInfo>(paths);
            return allItemInfoDic; 
        }
    }

    public ItemInfo itemInfo;
    public Sprite itemSprite;
    private Sprite itemGridSprite;
    public Sprite ItemGridSprite
    {
        get
        {
            if (itemGridSprite == null)
                itemGridSprite = ResMgr.GetInstance().Load<Sprite>("ItemQuality/" + itemQuality.ToString());
            return itemGridSprite;
        }
    }
    private static Sprite whiteGridSprite;
    public static Sprite WhiteGridSprite
    {
        get
        {
            if (whiteGridSprite == null)
                whiteGridSprite = ResMgr.GetInstance().Load<Sprite>("ItemQuality/" + "White");
            return whiteGridSprite;
        }
    }
    public IHolderAble holder;
    public ItemQuality itemQuality;
    public string itemName;
    public bool isNeverDrop;//是否绝不掉落
    public static string lockedTip = "该物品已锁定";
    private bool isLocked = false;
    public bool IsLocked
    {
        get { return isLocked; }
        set { isLocked = value; }
    }
    public bool isUnlimited = false;//是否为无限
    public string itemBaseInfo;
    public string itemExtraInfo;
    public int level = 1;
    protected int count;//物体自己记录自己有多少个，这是模仿伊洛纳的物品处理
    private string specialType;
    protected Dictionary<DetailButtonType, DetailButton> detailButtons = new Dictionary<DetailButtonType, DetailButton>();

    protected int basePrice;
    private int price = 100;
    public abstract ItemCategory GetItemCategory();
    public virtual Dictionary<DetailButtonType, DetailButton> DetailButtons { get { return detailButtons; } }
    public virtual ItemSaveData GetItemSaveData()
    {
        ItemSaveData itemSaveData = new ItemSaveData();
        itemSaveData.itemName = itemName;
        itemSaveData.itemQuality = itemQuality;
        itemSaveData.count = count;
        itemSaveData.level = level;
        itemSaveData.price = price;
        itemSaveData.isLocked = IsLocked;
        //itemSaveData.itemType = GetType().ToString();
        return itemSaveData;
    }
    public virtual int GetPrice()
    {
        return price;
    }
    //子类可以重写价格的计算规则
    protected virtual int CreatePrice()
    {
        int qualityValue = (int)itemQuality;
        float qualityRatio = 1 + qualityValue * qualityValue / 10.0f;
        float levelRatio = 1 + (level - 1) * 0.05f;
        return (int)(basePrice * qualityRatio * levelRatio);
    }
    public virtual void LoadItemSaveData(ItemSaveData itemSaveData)
    {
        this.count = itemSaveData.count;
        this.price = itemSaveData.price;
        this.IsLocked = itemSaveData.isLocked;
    }
    public virtual ItemType GetItemType()
    {
        return ItemType.CanCombine;
    }
    Sprite IDragAble.GetSprite_Drag()
    {
        return itemSprite;
    }
    //获取名字
    public string GetName()
    {
        return itemName;
    }
    //获取数量
    public int GetCount()
    {
        return count;
    }
    //是否是消耗品
    public bool IsCanCombine()
    {
        if (GetItemType() == ItemType.CanCombine)
            return true;
        else
            return false;
    }

    public virtual int GetUpLimitCount()
    {
        return -1;
    }

    //设置点按对应物体时可以进行的操作按钮，比如丢弃或装备或使用，可由子类重写
    //注意此处不能使用DetailDetailButtons，因为这个方法会在子类Equip等中的DetailDetailButtons被调用
    public virtual void SetDetailButtons()
    {
        detailButtons[DetailButtonType.Up] = new DetailButton("丢弃", Color.red, () =>
         {
             if(IsLocked == true)
             {
                 UIManager.GetInstance().ShowTips(Item.lockedTip);
                 return;
             }
             UIManager.GetInstance().ShowPanel<AbandonPanel>(AbandonPanel.prefabsPath, PanelLayer.Max, null, (panel) =>
             {
                 panel.SetItem(null, this);
             });
             UIManager.GetInstance().HidePanel(DetailPanel.prefabsPath);
         });
        detailButtons[DetailButtonType.Middle] = null;
        detailButtons[DetailButtonType.Down] = null;
    }
    public void SetItemCount(int count)
    {
        this.count = count;
    }
    public void ReverseSpltRules(Item item)
    {
        if (isUnlimited == true)
            return;
        this.count += item.count;
    }
    public virtual List<Item> SplitRules(int cellUpLimit)
    {
        if (GetUpLimitCount() > 0)//如果子类重新该方法，就使用子类的（重写成功的条件就是返回值大于0）
            cellUpLimit = GetUpLimitCount();

        int splitCeilCount = cellUpLimit;//默认使用背包的单元最大数量
        if (GetItemType() == ItemType.CannotCombine)//如果是不可堆叠物品，每个单元都只有1
        {
            if(count > 1)
                splitCeilCount = 1;
            else
                return new List<Item>() { this };//如果是不可堆叠物品，除了初始加载，数量不会大于一，不应该更换索引，比如魔法球
        }
        //切分规则会创建新的物体更换索引
        List<Item> splitItems = new List<Item>();
        int slipCount = this.count / splitCeilCount;//如果这个物体的数量大于当个格子数量的上限，就进行分割
        for (int i = 0; i < slipCount; i++)
        {
            this.count -= splitCeilCount;

            object obj = Activator.CreateInstance(this.GetType(), this, splitCeilCount);//返回溢出的物体
            Item newItem = obj as Item;
            Initial_SplitRules(newItem);
            splitItems.Add(newItem);
        }
        if (this.count > 0)//如果splitItems原本的数量是countUpperLimit的整数倍，那么分割完，就没了
            splitItems.Add(this);//不要忘了还有剩下的物品，也一并添加进去

        return splitItems;
    }

    /// <summary>
    /// 对切分规则创建的新的溢出物体进行初始处理
    /// </summary>
    /// <param name="newItem">溢出的物体</param>
    protected virtual void Initial_SplitRules(Item newItem)
    {

    }

    /// <summary>
    /// 这是用于物品库的分类的，比如装备还可以细分为武器等，建议用枚举类型，不容易打错字，不用细分也要返回一个特定的名字
    /// </summary>
    /// <returns></returns>
    public string GetSpecialType()
    {
        return specialType;
    }
    //堆叠规则,物品堆叠规则，返回多余的物体
    public virtual Item StackRules(Item item, int countUpperLimit)
    {
        if (GetUpLimitCount() > 0)//如果子类重新该方法，就使用子类的（重写成功的条件就是返回值大于0）
            countUpperLimit = GetUpLimitCount();

        Item newItem = null;
        if (IsCanCombine() == true)
        {
            if (this.count + item.GetCount() > countUpperLimit)//如果增加后超过上限
            {
                int useCount = countUpperLimit - this.count;//实际合并使用的物品数量
                this.count += useCount;//当前物品合并后的数量
                item.count -= useCount;//溢出物品的数量

                object obj = Activator.CreateInstance(this.GetType(), this, item.count);//返回溢出的物体
                newItem = obj as Item;
                Initial_StackRules(newItem);
            }
            else
            {
                this.count += item.GetCount();
                newItem = null;//返回null，既没有物体溢出
            }
        }
        return newItem;
    }

    /// <summary>
    /// 对堆叠规则创建的新的溢出物体进行初始处理
    /// </summary>
    /// <param name="newItem">溢出的物体</param>
    protected virtual void Initial_StackRules(Item newItem)
    {

    }

    //丢弃规则，数量为0会从背包中移除
    public virtual Item DecreaseRules(int count)
    {
        int decreaseCount = count;
        if (count < 0)
            decreaseCount = this.count;
        this.count -= decreaseCount;

        object obj = Activator.CreateInstance(this.GetType(), this, count);
        Item newItem = obj as Item;

        Initial_DecreaseRules(newItem);
        return newItem;
    }

    /// <summary>
    /// 对减少规则创建的移除的物体进行初始处理
    /// </summary>
    /// <param name="newItem">移除的物体</param>
    protected virtual void Initial_DecreaseRules(Item newItem)
    {

    }

    public bool IsSame(Item item)
    {
        return (this.itemQuality == item.itemQuality && this.IsLocked == item.IsLocked) ;
    }

    public bool PlayerStateCheck()
    {
        IPlayerAble player = holder as IPlayerAble;
        if (player == null)
            return false;

        //只有攻击和休息状态可以使用道具
        if ((player.GetPlayerAnimState() != PlayerAnimState.Attack && player.GetPlayerAnimState() != PlayerAnimState.Idle) || player.GetPlayerAnimContoller().GetCurrentAnimationSpeed() <= 0)
        {
            UIManager.GetInstance().ShowTips("当前状态不可使用");
            return false;
        }
        return true;
    }
    public void UseProp(Action action)
    {
        PlayerMgr.Instance.GetPlayerAnimContoller().SetTrigger("UseProp");
        MonoMgr.GetInstance().DelayAction(0.3f, () =>
        {
            PoolMgr.GetInstance().GetObj("FX/UseProp", (obj) =>
            {
                if (obj != null)
                {
                    obj.transform.position = PlayerMgr.Instance.GetPlayerObject().transform.position + new Vector3(0, 1, 0);

                    MonoMgr.GetInstance().DelayAction(0.2f, () =>
                    {
                        if (obj != null)
                            PoolMgr.GetInstance().PushObj(obj);
                    });
                }
            });
        });
        MonoMgr.GetInstance().DelayAction(0.5f, action);
    }

    public HashSet<string> GetCategory()
    {
        return new HashSet<string>() { GetItemCategory().ToString() };
    }
    public void UseGrid()
    {
        
    }
    public int Compare(Item item)
    {
        //被锁定的物体，一定在最上面
        if(isLocked == true && item.isLocked == false)//如果只有其中一个是锁定的，可以直接判定谁在前
        {
            return 1;
        }
        else if (isLocked == false && item.isLocked == true)//如果只有其中一个是锁定的，可以直接判定谁在前
        {
            return -1;
        }
        else//如果两个锁定情况相同，就开始正常判断
        {
            if ((int)this.itemQuality > (int)item.itemQuality)//质量高的上游
                return 1;
            else if ((int)this.itemQuality == (int)item.itemQuality)//如果质量相同
            {
                if ((int)this.GetItemCategory() < (int)item.GetItemCategory())//优先让某个类型在上面，这里装备我想显示在最上面，所以小的反而返回1
                {
                    return 1;
                }
                else if ((int)this.GetItemCategory() == (int)item.GetItemCategory())//如果类型相同
                {
                    if (this.GetSpecialType() == item.GetSpecialType())//如果特殊类型相同，比如都是装备，同时也都是法杖
                    {
                        if (this.GetName().CompareTo(item.GetName()) > 0)//名字更大的会上游，也就是说同名物体会在一起
                            return 1;
                        else if (this.GetName().CompareTo(item.GetName()) == 0 && this.GetCount() > item.GetCount())//名字相同，数量更多的上游
                            return 1;
                        else
                            return -1;
                    }
                    else
                    {
                        return CompareToSameCatagory(item);//子类自己实现特殊类型的排序，比如装备，装备需要自己判断是武器在上还是防具在上之类的
                    }
                }
                else
                    return -1;
            }
            else
            {
                return -1;
            }
        }
    }

    protected virtual int CompareToSameCatagory(Item item)
    {
        return -1;
    }

    public Sprite GetSprite()
    {
        return this.itemSprite;
    }
    public virtual Vector3 CenterPosOffset()
    {
        return Vector3.zero;
    }
    public virtual Vector3 GetShowScale()
    {
        return Vector3.one;
    }

}
