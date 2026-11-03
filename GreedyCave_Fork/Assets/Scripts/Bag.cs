using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectAble<T>
{
    public int Compare(T item);
    string GetName();//获取物体的名字
    /// <summary>
    /// 是否是消耗品,消耗品才可以合并和堆叠
    /// </summary>
    /// <returns></returns>
    bool IsCanCombine();
    
    HashSet<string> GetCategory();
    /// <summary>
    /// 判断是否是可以合并的物体，比如品质是否相同
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool IsSame(T item);
    void ReverseSpltRules(T item);
    List<T> SplitRules(int countUpperLimit);

    /// <summary>
    /// 在背包中占用一个格子时调用的方法
    /// </summary>
    public void UseGrid();
    /// <summary>
    /// 非消耗品不用实现，返回null即可
    /// 堆叠规则，这个由物体自己实现，比如：伊洛娜的书本是增加使用次数，而其他消耗品是增加数量
    /// 返回溢出物体，如果没有溢出返回null，其实现应该是创建一个新物体，并设置其count
    /// </summary>
    /// <param name="item">新来的物体</param>
    /// <returns></returns>
    T StackRules(T item, int countUpperLimit);

    /// <summary>
    /// 当物体的GetCount返回值<=0时，物体会被删除
    /// 传入需要减少的数量，返回新物体，新物体的数量等于刚刚减去的数量。注意需要实现count未-1时丢弃全部
    /// </summary>
    /// <param name="count">外界需要减少的数量</param>
    /// <returns></returns>
    T DecreaseRules(int count);

    /// <summary>
    /// 物体自己记录自己有多少个
    /// </summary>
    /// <returns></returns>
    int GetCount();
}


public class DesignItems<T> where T : ICollectAble<T>
{
    public int allCount;
    public List<T> itemList = new List<T>();
}

public interface ITemporaryBagAble
{

}
public class BagMgr<T> where T : ICollectAble<T>
{
    public BagMgr()
    {
        this.mainBag = new Bag<T>();
    }
    public BagMgr(int allCapacity, int cellUpLimit = 99)
    {
        this.mainBag = new Bag<T>(allCapacity, cellUpLimit);
    }
    private Bag<T> mainBag;
    private Dictionary<ITemporaryBagAble, Bag<T>> temporaryBagDic = new Dictionary<ITemporaryBagAble, Bag<T>>();

    public Bag<T> GetMainBag()
    {
        return mainBag;
    }
    public Dictionary<ITemporaryBagAble, Bag<T>> GetTemporaryBagDic()
    {
        return temporaryBagDic;
    }
    public void SetMainBag(Bag<T> bag)
    {
        mainBag = bag;
    }
    public bool IsUsingTemporaryBag()
    {
        return temporaryBagDic.Count > 0;//如果大于0说明还有人在用，此时无法保存
    }
    public void LoadTemporaryBags(List<Bag<T>> bags)
    {
        if (bags == null)
            return;
        for (int i = 0; i < bags.Count; i++)
        {
            LoadTemporaryBag(bags[i]);
        }
    }
    public void LoadTemporaryBag(Bag<T> bag)
    {
        if (bag == null)
            return;

        mainBag.BeUseCapacity -= bag.UseCapacity;
        bag.UseCapacity = 0;

        List<T> items = bag.RemoveBagItems();
        if (items == null)
            return;

        for (int i = 0; i < items.Count; i++)
        {
            mainBag.AddItem(items[i]);
        }

    }
    public T UseTemporaryBag(ITemporaryBagAble temporaryBagAble, T item)
    {
        if (temporaryBagAble == null)
            return default(T);
        if (item == null)
            return default(T);
        if (mainBag.IsContainItem(item.GetName()) == false)
            return default(T);
        T newItem = mainBag.RemoveItem(item);

        if (!temporaryBagDic.ContainsKey(temporaryBagAble))
            temporaryBagDic[temporaryBagAble] = new Bag<T>();

        temporaryBagDic[temporaryBagAble].AddItem(newItem);
        if (item.GetCount() <= 0)//如果旧物体数量小于等于0，说明使用了一个格子
        {
            mainBag.BeUseCapacity += 1;
            temporaryBagDic[temporaryBagAble].UseCapacity += 1;
        }
        return newItem;
    }
    public void UseEndTemporaryBag(ITemporaryBagAble temporaryBagAble)
    {
        if (temporaryBagAble == null)
            return;
        if (temporaryBagDic.ContainsKey(temporaryBagAble) == false)
            return;

        LoadTemporaryBag(temporaryBagDic[temporaryBagAble]);
        temporaryBagDic.Remove(temporaryBagAble);
    }
}

public class Bag<T> where T : ICollectAble<T>
{
    public Bag()
    {
        this.mainBag = new List<T>();
    }
    public Bag(int allCapacity, int cellUpLimit = 99)
    {
        if(allCapacity <= 0)
        {
            this.mainBag = new List<T>();
            return;
        }
        this.allCapacity = allCapacity;
        this.mainBag = new List<T>(allCapacity);
        this.cellUpLimit = Mathf.Clamp(cellUpLimit, 1, int.MaxValue);
    }
    private List<T> mainBag;//主背包
    //物品类别对应 名字对应其在主背包中的下标
    private Dictionary<string, Dictionary<string, SortedSet<int>>> mainDic = new Dictionary<string, Dictionary<string, SortedSet<int>>>();
    private MinQue<int> emptyGrid = new MinQue<int>();//使用最小堆，记录暂时空着的格子
    private bool isRefreshBag;
    private int allCapacity = -1;//小于等于零为没有上限

    private int cellUpLimit = 99;
    private int useCapacity = 0;//使用的空间数量
    private int beUseCapacity = 0;//被使用空间的数量

    public int UseCapacity
    {
        get { return useCapacity; }
        set { useCapacity = value; }
    }
    public int BeUseCapacity
    {
        get { return beUseCapacity; }
        set { beUseCapacity = value; }
    }

    public int Count
    {
        get { return mainBag.Count - emptyGrid.Count; }
    }
    public int OriginalCapacity
    {
        get { return allCapacity; }
    }
    public int CurrentCapacity
    {
        get { return allCapacity - beUseCapacity; }
    }
    
    //public void AddUseCapacity(int count)
    //{
    //    useCapacity += count;
    //}
    //public void AddBeUseCapacity(int count)
    //{
    //    beUseCapacity += count;
    //}
    public void CopyItem(T item, int index)
    {
        mainBag.Add(item);
        HashSet<string> categorys = item.GetCategory();
        foreach (var hashSet in categorys)
        {
            string category = hashSet;
            if (!mainDic.ContainsKey(hashSet))
                mainDic[category] = new Dictionary<string, SortedSet<int>>();
            if (!mainDic[category].ContainsKey(item.GetName()))
                mainDic[category][item.GetName()] = new SortedSet<int>();

            mainDic[category][item.GetName()].Add(index);
        }
    }
    public void ResetAllCapacity(int allCapacity)
    {
        this.allCapacity = allCapacity;
    }
    public bool IsCapacityFull()
    {
        if (isRefreshBag == true)
            return false;
        if (allCapacity <= 0)
            return false;
        if (emptyGrid.Count > 0)
            return false;

        return Count >= allCapacity;
    }
    public bool IsContainCategory(string category)
    {
        return mainDic.ContainsKey(category);
    }
    public bool IsContainItem(string itemName)
    {
        return IsContainItem(itemName, mainDic);
    }
    public bool IsContainItem(T item)
    {
        List<T> items = GetNameBagItem(item.GetName());
        for (int i = 0; i < items.Count; i++)
        {
            if (item.Equals(items[i]))
                return true;
        }
        return false;
    }

    //这里返回的是mainBag主体，里面可能含有Null
    public List<T> GetMainBag()
    {
        return mainBag;//GetBagItems(null);
    }
    public List<T> GetBagItems(Func<T, bool> func = null)
    {
        List<T> items = new List<T>();
        ForeachAllItems((item) =>
        {
            if (item != null && (func == null || func(item) == true))
                items.Add(item);
        });
        return items;
    }
    public T GetOneNameBagItem(string itemName, Func<T, bool> func = null)
    {
        List<T> items = GetNameBagItem(itemName, func);
        if (items != null && items.Count > 0)
            return items[0];

        return default(T);
    }
    public List<T> GetNameBagItem(string itemName, Func<T, bool> func = null)
    {
        List<T> items = new List<T>();
        ForeachAllNameItems(itemName, (item) =>
        {
            if (item != null && (func == null || func(item) == true))
                items.Add(item);
        });
        return items;
    }
    public List<T> GetCategoryBagItems(string itemCategory, Func<T, bool> func = null)
    {
        List<T> items = new List<T>();
        ForeachAllCategoryItems(itemCategory, (item) =>
        {
            if (item != null && (func == null || func(item) == true))
                items.Add(item);
        });

        return items;
    }
    /// <summary>
    /// 移除所有满足func条件的物品
    /// </summary>
    /// <param name="func">返回true为满足条件</param>
    /// <param name="count">移除的数量，小于等于0即为移除全部</param>
    /// <returns></returns>
    public List<T> RemoveBagItems(Func<T, bool> func = null)
    {
        List<T> removeItems = new List<T>();
        ForeachAllItems((item) =>
        {
            if (item != null && (func == null || func(item) == true))
            {
                T newItem = RemoveItem(item, item.GetCount());
                if (newItem != null)
                    removeItems.Add(newItem);
            }
        });
        return removeItems;
    }
    public List<T> RemoveCategoryBagItems(string category, Func<T, bool> func = null, int count = -1)
    {
        List<T> needRemoveItems = new List<T>();
        List<T> removeItems = new List<T>();
        ForeachAllCategoryItems(category, (item) =>
        {
            if (item != null && (func == null || func(item) == true))
                needRemoveItems.Add(item);
        });
        for (int i = 0; i < needRemoveItems.Count; i++)
        {
            if (count <= 0)
                count = needRemoveItems[i].GetCount();

            T newItem = RemoveItem(needRemoveItems[i], needRemoveItems[i].GetCount());
            if (newItem != null)
                removeItems.Add(newItem);
        }
        return removeItems;
    }
    public DesignItems<T> GetNameDesignItems(string itemName, Func<T, bool> func = null)//满足条件的对应物品才可以返回
    {
        DesignItems<T> designItems = new DesignItems<T>();
        ForeachAllNameItems(itemName, (item) =>
        {
            if (item != null && (func == null || func(item) == true))
            {
                designItems.itemList.Add(item);
                designItems.allCount += item.GetCount();
            }
        });
        return designItems;
    }
    public DesignItems<T> GetCategoryDesignItems(string itemCategory, Func<T, bool> func = null)
    {
        DesignItems<T> designItems = new DesignItems<T>();
        ForeachAllCategoryItems(itemCategory, (item) =>
        {
            if (item != null && (func == null || func(item) == true))
            {
                designItems.allCount += item.GetCount();
                designItems.itemList.Add(item);
            }
        });
        return designItems;
    }
    public DesignItems<T> GetAllDesignItems(Func<T, bool> func = null)//满足条件的对应物品才可以返回
    {
        DesignItems<T> designItems = new DesignItems<T>();

        ForeachAllItems((item) =>
        {
            if (item != null && (func == null || func(item) == true))
            {
                designItems.itemList.Add(item);
                designItems.allCount += item.GetCount();
            }
        });
        return designItems;
    }
    /// <summary>
    /// 注意此方法不可以移除物品，只可以访问
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="action"></param>
    private void ForeachAllNameItems(string itemName, Action<T> action)
    {
        foreach (var dicItem in mainDic)
        {
            Dictionary<string, SortedSet<int>> nameDic = dicItem.Value;
            if (!nameDic.ContainsKey(itemName))
                continue;

            SortedSet<int> list = nameDic[itemName];
            if (list == null)
                return;
            foreach (int index in list)
            {
                int itemIndex = index;
                T item = GetItem_Index(itemIndex);
                if (item != null)
                    action(item);
            }
            //for (int i = 0; i < list.Count; i++)
            //{
            //    int itemIndex = list[i];
            //    T item = GetItem_Index(itemIndex);
            //    if (item != null)
            //        action(item);
            //}
            break;
        }
    }
    /// <summary>
    /// 注意此方法不可以移除背包物体，只可以访问
    /// </summary>
    /// <param name="itemCategory"></param>
    /// <param name="action"></param>
    private void ForeachAllCategoryItems(string itemCategory, Action<T> action)
    {
        if (!mainDic.ContainsKey(itemCategory))
            return;
        Dictionary<string, SortedSet<int>> nameDic = mainDic[itemCategory];
        foreach (var nameDicItem in nameDic)
        {
            string itemName = nameDicItem.Key;
            SortedSet<int> indexs = nameDicItem.Value;
            foreach (int index in indexs)
            {
                int itemIndex = index;
                T item = mainBag[itemIndex];
                if (item != null)
                    action(item);
            }
            //for (int i = 0; i < indexs.Count; i++)
            //{
            //    int itemIndex = indexs[i];
            //    T item = mainBag[itemIndex];
            //    if (item != null)
            //        action(item);
            //}
        }
    }
    /// <summary>
    /// 此方法可以移除物品
    /// </summary>
    /// <param name="action"></param>
    public void ForeachAllItems(Action<T> action)
    {
        int count = mainBag.Count;
        for (int i = 0; i < count; i++)
        {
            T item = mainBag[i];
            if (item != null)
                action(item);
        }
    }
    public int GetIndex_Item(T item)
    {
        return GetIndex_Item(mainBag, mainDic, item);
    }
    public T GetItem_Index(int index)
    {
        if (mainBag.Count <= index || index < 0)
            return default(T);
        return mainBag[index];
    }
    public T AddItem(T item)
    {
        if (item == null || item.GetCount() <= 0)
            return default(T);

        List<T> splitItems = item.SplitRules(cellUpLimit);//如果物体数量大于背包的最大单元物品数，就进行分割
        if (splitItems == null)
            splitItems = new List<T>() { item };

        T returnItem = default(T);//溢出物体
        for (int i = 0; i < splitItems.Count; i++)
        {
            T overlflow = AddItem(mainBag, mainDic, splitItems[i]);
            if (overlflow != null)
            {
                if (returnItem == null)
                    returnItem = overlflow;
                else
                    returnItem.ReverseSpltRules(overlflow);//合并溢出物体
            }
        }
        return returnItem;
    }
    public T RemoveItem(T item, int count = 1)
    {
        if (item == null || count <= 0)
            return default(T);

        return RemoveItem(mainBag, mainDic, item, count);
    }

    private T AddItem(List<T> bag, Dictionary<string, Dictionary<string, SortedSet<int>>> dic, T item)
    {
        if (item == null)
            return item;
        string itemName = item.GetName();

        if (IsCapacityFull() == false && !IsContainItem(itemName, dic))//如果背包中不包含此物体，并且背包容量没有满
        {
            UseSpace(bag, dic, item);//占用格子空间
        }
        else//如果背包中已经有此物体，或者容量已经满了，就执行合并，并且返回溢出对象
        {
            return Combine(bag, dic, item);
        }
        return default(T);
    }
    private bool IsContainItem(string itemName, Dictionary<string, Dictionary<string, SortedSet<int>>> dic)
    {
        foreach (var item in dic)
        {
            if (item.Value.ContainsKey(itemName))
                return true;
        }
        return false;
    }
    private Dictionary<string, SortedSet<int>> GetNameDic(T item, Dictionary<string, Dictionary<string, SortedSet<int>>> dic)
    {
        HashSet<string> categorys = item.GetCategory();
        foreach (var hashSetItem in categorys)
        {
            if (dic.ContainsKey(hashSetItem))
                return dic[hashSetItem];
        }
        return null;
    }
    private T Combine(List<T> bag, Dictionary<string, Dictionary<string, SortedSet<int>>> dic, T item)
    {
        string itemName = item.GetName();
        if (!IsContainItem(itemName, dic))//如果不包含该物品，就不可以合并并且把这个物体返回
            return item;

        Dictionary<string, SortedSet<int>> nameDic = GetNameDic(item, dic);
        if (nameDic == null)
            return item;

        List<T> olderItems = new List<T>();
        if(item.IsCanCombine())
        {
            if (!nameDic.ContainsKey(itemName))
            {
                Debug.Log("请查阅是否含有两个同名物体拥有不同的物品分类");
                return item;
            }
            foreach (var index in nameDic[itemName])
            {
                T oldItem = bag[index];
                //如果是相同品质 并且 没有达到数量上限 的物品才有可能合并
                if (oldItem.IsSame(item) && oldItem.GetCount() < cellUpLimit && oldItem.IsCanCombine())
                    olderItems.Add(bag[index]);
            }

            T overflowItem = item;
            for (int i = 0; i < olderItems.Count; i++)
            {
                overflowItem = olderItems[i].StackRules(overflowItem, cellUpLimit);//让背包中的物体自己实现堆叠规则。返回溢出物体。没有溢出返回空

                if (overflowItem == null)//如果已经没有溢出物体了，就可以结束方法
                    return default(T);
            }
            if (overflowItem != null)//如果还有溢出物体
            {
                if (IsCapacityFull() == true)//如果背包已经满了,就把多余的返回
                    return overflowItem;
                UseSpace(bag, dic, overflowItem);//如果背包没满，就使用一个新的格子
            }
        }
        else//不可堆叠的物品，或者没有可以合并的物品
        {
            if (IsCapacityFull() == true)//如果背包已经满了,就把多余的返回
                return item;
            UseSpace(bag, dic, item);
        }
        return default(T);
    }
    //占用格子空间
    private void UseSpace(List<T> bag, Dictionary<string, Dictionary<string, SortedSet<int>>> dic, T item)
    {
        int index = 0;
        if (!emptyGrid.IsEmpty)
        {
            index = emptyGrid.Deque();
            bag[index] = item;
        }
        else
        {
            bag.Add(item);
            index = bag.Count - 1;
        }

        OperiateDic(item,index,dic);
        item.UseGrid();
    }
    private void OperiateDic(T item,int index,Dictionary<string, Dictionary<string, SortedSet<int>>> dic)
    {
        HashSet<string> categorys = item.GetCategory();

        foreach (var hashSetItem in categorys)
        {
            if (!dic.ContainsKey(hashSetItem))
                dic[hashSetItem] = new Dictionary<string, SortedSet<int>>();
            if (!dic[hashSetItem].ContainsKey(item.GetName()))
                dic[hashSetItem][item.GetName()] = new SortedSet<int>();
            dic[hashSetItem][item.GetName()].Add(index);
        }
        //Debug.Log("==============================");
        //ForeachAllItems((foreachItem)=> { Debug.Log(foreachItem.GetName() + ":" + index); });
    }
    private T RemoveItem(List<T> bag, Dictionary<string, Dictionary<string, SortedSet<int>>> dic, T item, int count = 1)
    {
        int index = GetIndex_Item(bag, dic, item);
        if (index >= 0)
            return RemoveItem(bag, dic, index, count);
        else
            Debug.Log("背包不含物体:" + item.GetName());

        return default(T);
    }
    private int GetIndex_Item(List<T> bag, Dictionary<string, Dictionary<string, SortedSet<int>>> dic, T item)
    {
        string itemName = item.GetName();
        if (!IsContainItem(itemName, dic))
            return -1;
        Dictionary<string, SortedSet<int>> nameDic = GetNameDic(item, dic);
        SortedSet<int> itemIndexList = nameDic[itemName];
        int targetIndex = -1;
        
        foreach (var index in itemIndexList)
        {
            if (bag[index].Equals(item))//Object类型的约束也可以让两个泛型进行==符号运算
            {
                targetIndex = index;
                break;
            }
        }
        //for (int i = 0; i < itemIndexList.Count; i++)
        //{
        //    if (bag[itemIndexList[i]].Equals(item))//Object类型的约束也可以让两个泛型进行==符号运算
        //    {
        //        targetIndex = itemIndexList[i];
        //        break;
        //    }
        //}
        return targetIndex;
    }
    private T RemoveItem(List<T> bag, Dictionary<string, Dictionary<string, SortedSet<int>>> dic, int index, int count = 1)//使用下标删除一个物体。默认-1就是丢弃这个物品的所有数量
    {
        if (index < 0 || index >= bag.Count)
        {
            Debug.Log("删除下标越界");
            return default(T);//相当于null
        }
        T decreaseItem = bag[index].DecreaseRules(count);//是否丢完
        if (bag[index].GetCount() <= 0)
        {
            string itemName = bag[index].GetName();
            HashSet<string> categorys = bag[index].GetCategory();
            foreach (var hashSetItem in categorys)
            {
                if (!dic.ContainsKey(hashSetItem) || !dic[hashSetItem].ContainsKey(itemName))
                    continue;
                dic[hashSetItem][itemName].Remove(index);

                if (dic[hashSetItem][itemName].Count <= 0)
                    dic[hashSetItem].Remove(itemName);
                if (dic[hashSetItem].Count <= 0)
                    dic.Remove(hashSetItem);
            }

            bag[index] = default(T);//一般情况在背包丢弃一个物品以后，不会立刻释放此格子。这时如果增加一个物体，就会占用此格子
            emptyGrid.Enque(index);//使用最小堆记录这个暂时还没释放的格子
        }
        return decreaseItem;
    }
    public void Clear()
    {
        mainBag.Clear();
        mainDic.Clear();
        emptyGrid.Clear();
    }
    /// <summary>
    /// 刷新背包
    /// </summary>
    /// <param name="isBreakCheck">是否跳过背包数量检测，如果背包物体数量没有改变，就不会刷新背包，但是可堆叠的物体无法更新，跳过就可以更新</param>
    public void RefreshBag(bool isBreakCheck = false)
    {
        if (emptyGrid.Count <= 0 && isBreakCheck == false)//如果没有变化，就不需要刷新
            return;
        isRefreshBag = true;//刷新背包过程中,不可以进行上限检测，否则，如果背包已满，添加会失败
        List<T> newBag = new List<T>();//主背包
        Dictionary<string, Dictionary<string, SortedSet<int>>> newDic = new Dictionary<string, Dictionary<string, SortedSet<int>>>();//名字对应主背包的下标
        emptyGrid.Clear();
        for (int i = 0; i < mainBag.Count; i++)
        {
            //if(mainBag[i] is Item)
            //{
            //    Debug.Log((mainBag[i] as Item).GetName() + ":" + (mainBag[i] as Item).IsLocked);
            //}
            if (mainBag[i] != null)
            {
                AddItem(newBag, newDic, mainBag[i]);
            }
        }
        mainBag = newBag;
        mainDic = newDic;
        isRefreshBag = false;
    }

    public void CombineAllItems()
    {

    }

    public void Tidy_Quality_Category()
    {
        RefreshBag(true);
        for (int i = 1; i < mainBag.Count; i++)
        {
            T item = mainBag[i];
            if (item == null)
                continue;
            int j;
            for (j = i ; j > 0; j--)
            {
                T lastItem = mainBag[j - 1];
                if (lastItem == null || item.Compare(lastItem) > 0)//大小比较
                    mainBag[j] = mainBag[j - 1];//上一个物体后退
                else
                    break;
            }
            mainBag[j] = item;
        }
        //有可能有为空的空间，把它移除
        int allCount = mainBag.Count;
        for (int i = allCount - 1; i >= 0 ; i--)
        {
            if (mainBag[i] == null)
                mainBag.RemoveAt(i);
        }
        emptyGrid.Clear();
        mainDic.Clear();
        for (int i = 0; i < mainBag.Count; i++)
        {
            OperiateDic(mainBag[i], i, mainDic);
        }
    }
}