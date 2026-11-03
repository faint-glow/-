using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public interface IPlayerAble
{
    //public bool IsBelongMe(IBelongAble belongAble);
    public PlayerData GetPlayerData();
    public GameObject GetPlayerObject();
    public Animator GetAnimator();
    public PlayerAnimState GetPlayerAnimState();
    public Vector3Int GetGirdPos();
    public PlayerAnimatorController GetPlayerAnimContoller();
    public Equip GetEquip(WearEquipType equipType);
    public void SetPlayerPos(Vector3 targetPos);
    public PlayerController GetPlayerController();
    //public void CreatePlayerObj(PlayerData playerData);
    public void DestoryPlayer();
    public Warrior GetWarrior();
    public Dictionary<WearEquipType, List<Equip>> GetEquips();
    public void SavePlayerData(bool isShowTip);
    public Texture GetStaticAvatar(bool isRebulid = false);

    public void StartBattle(Enemy enemy);
    public void OpenJar(JarObj jar);
    public void OpenTreasureBox(TreasureBoxObj box);

    public void NormalAttack();
    public void TriggerSkill(string skillInfoName);

    public bool RecycleDecorate(DecorateObj decorateObj);
}

public enum HolderType
{
    Player,
    Merchant,
    Farrier,//铁匠
    Box,
    Discard,

}

public class PlayerMgr : Singleton<PlayerMgr>, IPlayerAble, IHolderAble, ISpeakAble, IConditionTargetAble//,IBootyHolderAble
{
    public static string PlayerLayerName = "NormalObject";
    public static int PlayerLayerOrder = 5;
    private GameObject player;
    private Animator anim;

    private PlayerController playerController;
    private BagMgr<Item> playerBagMgr;
    private SkillController skillController;
    private PlayerClothesController playerClother;
    private PlayerDataMgr playerDataMgr = new PlayerDataMgr();
    private List<IShortcutAble> shortcutCDs = new List<IShortcutAble>();
    private ShortcutController shortcutController = new ShortcutController();
    private PlayerDecorateController playerDecorate = new PlayerDecorateController();
    private PlayerEquipController playerEquipController = new PlayerEquipController();

    public PlayerMgr()
    {
        playerBagMgr = new BagMgr<Item>(BagSaveData.DefaultCapacity);
        playerClother = new PlayerClothesController(this);
        playerController = new PlayerController(this);
        skillController = new SkillController(this.GetWarrior());

        MonoMgr.GetInstance().AddUpdateListener(Update);
    }

    public void ClearRunTimeData()
    {
        playerBagMgr = new BagMgr<Item>(BagSaveData.DefaultCapacity);

        playerEquipController.ClearRunTimeData();
        playerDecorate.ClearRunTimeData();
        skillController.ClearRunTimeData(GetWarrior());
        shortcutController.ClearRunTimeData();
    }
    public void LoadBasePlayerData(PlayerData playerData)
    {
        GetWarrior().LevelUp(playerData.level, 0);
        GetWarrior().SetProperty(PropertyType.金币, playerData.currentCoin);
        GetWarrior().SetProperty(PropertyType.饱腹度, playerData.currentPhysiology);
        GetWarrior().SetProperty(PropertyType.生命值, playerData.currentHp);
        GetWarrior().SetProperty(PropertyType.魔法值, playerData.currentMp);
        GetWarrior().SetProperty(PropertyType.经验值, playerData.currentExperience);
    }
    public void LoadBag(PlayerData playerData)
    {
        playerBagMgr.SetMainBag(PlayerDataMgr.LoadBag(playerData.mainBag, this));
        playerBagMgr.LoadTemporaryBags(PlayerDataMgr.LoadBags(playerData.temporaryBag, this));
    }
    public void LoadEquip(PlayerData playerData)
    {
        playerEquipController.LoadEquip(GetWarrior(), playerData.equipDic);
    }
    public void LoadDecorate(PlayerData playerData)
    {
        playerDecorate.LoadDecorate(playerData);
    }
    public void LoadSkillController(PlayerData playerData)
    {
        skillController.LoadSkillSaveData(playerData);
    }
    public void LoadShortcutController(PlayerData playerData)
    {
        shortcutController.LoadShortcutData(playerData);
    }
    public void LoadAudioController()
    {
        if (GetPlayerData() != null)
            AudioMgr.Instance.SetAudioVolumeDic(GetPlayerData().audioVolumeDic);
        else
            AudioMgr.Instance.SetAudioVolumeDic(AudioMgr.DefaultAudioVolumeDic);
    }

    public void ClearCD()
    {
        if (shortcutCDs.Count > 0)
        {
            for (int i = 0; i < shortcutCDs.Count; i++)
            {
                shortcutCDs[i].AddCurrentCD(-shortcutCDs[i].GetCurrentCD());
                if (shortcutCDs[i].GetCurrentCD() <= 0)
                {
                    shortcutCDs.RemoveAt(i);
                    i--;
                }
            }
        }
    }
    private void Update()
    {
        if (shortcutCDs.Count > 0)
        {
            for (int i = 0; i < shortcutCDs.Count; i++)
            {
                shortcutCDs[i].AddCurrentCD(-Time.deltaTime);
                if (shortcutCDs[i].GetCurrentCD() <= 0)
                {
                    shortcutCDs.RemoveAt(i);
                    i--;
                }
            }
        }
    }
    public GameObject GetPlayerObject()
    {
        if (player == null)
        {
            Gender targetGender = PlayerData.DefaultGender;
            if (GetPlayerData() != null)
                targetGender = GetPlayerData().gender;
            player = ResMgr.GetInstance().Load<GameObject>(PlayerData.GetPlayerPrefabsPath(targetGender));
            CameraMgr.GetInstance().Follow(player);

            EventCenter.GetInstance().EventTrigger<GameObject>(EventCenterEnum.创建玩家.ToString(), player);
            MazeObj.CreateSmallMapObj(player.transform, ResMgr.GetInstance().Load<Sprite>("Sprite/SmallMapObj/Player"));
        }
        return player;
    }
    public bool IsPlayerExist()
    {
        return player != null;
    }
    public Animator GetAnimator()
    {
        if (anim == null)
        {
            if (GetPlayerObject() != null)
                anim = GetPlayerObject().GetComponentInChildren<Animator>();

            SortingGroup sortingGroup = anim.gameObject.GetComponent<SortingGroup>();
            if (sortingGroup == null)
                sortingGroup = anim.gameObject.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerName = PlayerMgr.PlayerLayerName;
            sortingGroup.sortingOrder = PlayerMgr.PlayerLayerOrder;
        }

        return anim;
    }
    public Vector3Int GetGirdPos()
    {
        if (player != null)
            return ToolsMgr.GetGirdPos(player.transform.position);
        else
            return Vector3Int.zero;
    }
    public void DestoryPlayer()
    {
        //if (player != null)
        //    GameObject.Destroy(player);

        //playerController.GetPlayerAnimatorController().RemoveAllAnimationEvent();
    }
    public bool IsBelongMe(IShortcutAble shortcutAble)
    {
        if (shortcutAble == null)
            return false;

        if (shortcutAble.GetWarrior() != null)
            return shortcutAble.GetWarrior() == PlayerMgr.Instance.GetWarrior();

        return false;
    }
    public void AddShortcutCD(IShortcutAble shortcut)
    {
        shortcutCDs.Add(shortcut);
    }
    public bool IsHost(IPlayerAble player)//是否为主机
    {
        return player == this;
    }
    public bool IsHost(string playerID)//是否为主机
    {
        return playerID == GetHostID();
    }
    #region PlayerData
    public string GetHostID()
    {
        return GetPlayerData().ID;
    }
    public PlayerDataMgr GetPlayerDataMgr()
    {
        return playerDataMgr;
    }
    public PlayerData GetPlayerData()
    {
        return playerDataMgr.GetPlayerData();
    }
    public void SavePlayerData(bool isShowTip = true)
    {
        //if (playerBagMgr.IsUsingTemporaryBag())
        //{
        //    UIManager.GetInstance().ShowTips("正在使用临时背包，保存失败");
        //    return;
        //}
        playerDataMgr.SaveData(GetWarrior(), isShowTip);
    }
    public void SetPlayerData(PlayerData data)
    {
        playerDataMgr.SetPlayerData(data);
    }
    public int GetCoin()
    {
        return (int)GetWarrior().GetOriginalProperty(PropertyType.金币);
    }
    public void ChangeCoin(int count)
    {
        GetWarrior().ChangerProperty(PropertyType.金币, count); //playerDataMgr.ChangeCoin(count);
        RefreshBagPanelHandler(true);
    }
    #endregion
    //public void UseBootys(List<Booty> bootys)
    //{
    //    Booty booty = Booty.GetRandomBooty(bootys);
    //    UseBooty(booty);
    //}
    public void UseBooty(Booty booty, IBootyHolderAble bootyHolder = null)
    {
        if (booty == null)
            return;
        List<Reward_Property> rewards = booty.GetReward_Property();
        for (int i = 0; i < rewards.Count; i++)
        {
            Reward_Property reward = rewards[i];
            PropertyType targetType = reward.rewardType;
            float targetValue = reward.GetValue();
            if (bootyHolder != null)
                targetValue = bootyHolder.AdjustReward_Property(targetValue, ref targetType);

            GetWarrior().ChangeProperty_Auto_Anim(targetType, targetValue);
        }

        List<Item> items = booty.GetItems();
        for (int i = 0; i < items.Count; i++)
            AddItemWithDiscard(items[i]);

        if (bootyHolder != null)
        {
            Vector3 offset = new Vector3(0, 1.5f, 0);
            float delayTime = WorldSpritePanelMgr.ShowItems<Reward_Property>(rewards, bootyHolder.GetPosition() + offset);
            MonoMgr.GetInstance().DelayAction(delayTime, () =>
            {
                WorldSpritePanelMgr.ShowItems<Item>(items, bootyHolder.GetPosition() + offset);
            });
        }
    }
    public void OpenTreasureBox(Equip equip)
    {
        AddItemWithDiscard(equip);
        PlayerMgr.Instance.SavePlayerData(false);
    }
    public void OpenTreasureBox(TreasureBoxObj box)
    {
        Equip equip = box.OpenTreasureBox();
        WorldSpritePanelMgr.ShowItem(equip, box.GetPosition() + new Vector3(0, 1.5f, 0));

        AddItemWithDiscard(equip);
        PlayerMgr.Instance.SavePlayerData(false);
    }
    public void OpenJar(JarObj jar)
    {
        Booty booty = jar.OpenJar(this);
        PlayerMgr.Instance.UseBooty(booty, jar);
        //AddItemWithDiscard(item);

        PlayerMgr.Instance.SavePlayerData(false);
    }

    #region PlayerDecorateController
    public bool RecycleDecorate(DecorateObj decorateObj)
    {
        if (decorateObj == null)
            return false;
        if (playerBagMgr.GetMainBag().IsCapacityFull())
        {
            UIManager.GetInstance().ShowTips("背包已满，无法回收");
            return false;
        }
        AddItem(decorateObj.decorateAble as Item);
        playerDecorate.RemoveDecorate(decorateObj.transform.position);
        return true;
    }
    public Dictionary<string, Dictionary<Vector3, IDecorateAble>> GetAllDecorates()
    {
        return playerDecorate.GetPlayerDecorateDic();
    }
    public void ShowDecorate(string sceneName)
    {
        playerDecorate.ShowDecorate(sceneName);
    }
    public void AddDecorate(Vector3 pos, IDecorateAble decorateAble, string sceneName = "")
    {
        playerDecorate.AddDecorate(pos, decorateAble, sceneName);
    }

    public IDecorateAble RemoveDecorate(Vector3 pos, string sceneName = "")
    {
        return playerDecorate.RemoveDecorate(pos, sceneName);
    }
    #endregion


    #region ShortcutController

    public ShortcutController GetShortcutController()
    {
        return shortcutController;
    }
    public IShortcutAble[] GetShortcuts(ShortcutType shortCutType)
    {
        return shortcutController.GetShortcuts(shortCutType);
    }

    public void SetShortcut(IShortcutAble shortcutObj, ShortcutType shortcutType, int index)
    {
        shortcutController.SetShorucut(shortcutObj, shortcutType, index);
    }

    public void RemoveShortcut(ShortcutType shortCutType, int index)
    {
        shortcutController.RemoveShortcut(shortCutType, index);
    }
    #endregion

    #region SkillController
    public SkillController GetSkillController()
    {
        return skillController;
    }
    public List<SkillInfoMgr> GetSkills(SkillDamageType skillType)
    {
        return skillController.GetSkills(skillType);
    }
    public void ForgetAllSkill()
    {
        skillController.ForgetSkill();
    }
    #endregion

    #region PlayerEquipController
    public Equip GetEquip(WearEquipType equipType)
    {
        return playerEquipController.GetEquip(equipType);
    }
    public bool IsWearEquip(Equip equip, out int index)
    {
        return playerEquipController.IsWearEquip(equip, out index);
    }

    public void WearEquip(Equip equip)
    {
        playerEquipController.WearEquip(GetWarrior(), equip);
        RefreshEquipPanel();
    }
    public void WearEquip(Equip equip, int index)
    {
        playerEquipController.WearEquip(GetWarrior(), equip, index);
        RefreshEquipPanel();
    }
    public void TakeOffEquip(Equip equip, int index)
    {
        playerEquipController.TakeOffEquip(equip, index);
        RefreshEquipPanel();
    }
    public void TakeOffEquips(Func<Equip, bool> func)
    {
        playerEquipController.TakeOffEquips(func);
    }
    private void RefreshEquipPanel()
    {
        playerController.RefreshAvata();
        BagPanel bagPanel = (BagPanel)UIManager.GetInstance().GetExistPanel(BagPanel.prefabsPath);
        if (bagPanel == null)
            return;
        PlayerInfoPanel playerInfoPanel = (PlayerInfoPanel)bagPanel.GetExistPanel(PlayerInfoPanel.prefabsPath);
        if (playerInfoPanel == null)
            return;
        if (!playerInfoPanel.gameObject.activeInHierarchy)
            return;
        EquipPanel equipPanel = playerInfoPanel.GetEquipPanel();
        if (equipPanel == null)
            return;

        equipPanel.RefreshEquipPanel();
    }

    public Dictionary<WearEquipType, List<Equip>> GetEquips()
    {
        return playerEquipController.EquipDic;
    }

    #endregion

    #region Bag
    public string GetBagName()
    {
        return "玩家背包";
    }
    public BagMgr<Item> GetBagMgr()
    {
        return playerBagMgr;
    }
    public Bag<Item> GetBag()
    {
        return playerBagMgr.GetMainBag();
    }
    public List<Item> GetBagItems(ItemCategory itemCategory)
    {
        return playerBagMgr.GetMainBag().GetCategoryBagItems(itemCategory.ToString(), null);
        //return playerBag.GetBagItems((item)=> {return item.GetItemCategory() == itemCategory; });
    }
    public int GetItemPrice(Item item)
    {
        if (item == null)
            return 0;
        return item.GetPrice();
    }
    public HolderType GetHolderType()
    {
        return HolderType.Player;
    }
    public Item AddItemWithDiscard(Item item)
    {
        if (item == null)
            return null;

        item.holder = this;
        Item overflowItem = playerBagMgr.GetMainBag().AddItem(item);//内部已经把item更换过索引，对Item的处理必须在Add之前
        if (overflowItem != null)
        {
            UIManager.GetInstance().ShowTips("玩家背包已满");
            UIManager.GetInstance().ShowPanel<DiscardPanel>(DiscardPanel.prefabsPath, PanelLayer.Top, null, (panel) =>
            {
                panel.AddItem(overflowItem);
            });
        }
        RefreshBagPanelHandler();
        return overflowItem;
    }
    public bool IsBagFull()
    {
        return playerBagMgr.GetMainBag().IsCapacityFull();
    }
    public Item AddItem(Item item)
    {
        if (item == null)
            return null;

        item.holder = this;
        Item overflowItem = playerBagMgr.GetMainBag().AddItem(item);//内部已经把item更换过索引，对Item的处理必须在Add之前
        if (overflowItem != null)
            UIManager.GetInstance().ShowTips("玩家背包已满");

        RefreshBagPanelHandler();
        return overflowItem;
    }
    public Item RemoveItem(Item item, int count = 1)
    {
        Item newItem = playerBagMgr.GetMainBag().RemoveItem(item, count);
        RefreshBagPanelHandler();
        return newItem;
    }
    public void RefreshBag()//IHolderAble
    {
        RefreshBagPanelHandler();
    }
    //刷新UI界面
    public void RefreshBagPanelHandler(bool isOnlyUI = false)
    {
        BagPanel bagPanel = (BagPanel)UIManager.GetInstance().GetExistPanel(BagPanel.prefabsPath);
        if (bagPanel != null)
            RefreshBagPanel(bagPanel, ItemBagPanel.prefabsPath, isOnlyUI);

        StorePanel storePanel = (StorePanel)UIManager.GetInstance().GetExistPanel(StorePanel.prefabsPath);
        if (storePanel != null)
            RefreshBagPanel(storePanel, ItemBagPanel.prefabsPath_Storage, isOnlyUI);

        StrengthenPanel strengthenPanel = (StrengthenPanel)UIManager.GetInstance().GetExistPanel(StrengthenPanel.prefabsPath);
        if (strengthenPanel != null)
            RefreshBagPanel(strengthenPanel, ItemBagPanel.prefabsPath_Strengthen, isOnlyUI);

        BoxPanel boxPanel = (BoxPanel)UIManager.GetInstance().GetExistPanel(BoxPanel.prefabsPath);
        if (boxPanel != null)
            RefreshBagPanel(boxPanel, ItemBagPanel.prefabsPath_Box, isOnlyUI);

        DiscardPanel discardPanel = (DiscardPanel)UIManager.GetInstance().GetExistPanel(DiscardPanel.prefabsPath);
        if (discardPanel != null)
            RefreshBagPanel(discardPanel, ItemBagPanel.prefabsPath_Discard, isOnlyUI);
    }
    private void RefreshBagPanel(BasePanel basePanel, string targetPrefabsPath, bool isOnlyUI = false)
    {
        ItemBagPanel itemBagePanel = (ItemBagPanel)basePanel.GetExistPanel(targetPrefabsPath);

        if (itemBagePanel == null)
            return;

        if (itemBagePanel.gameObject.activeInHierarchy)//如果没有显现就不用刷新。界面每次显现的时候内部会刷新
        {
            if (isOnlyUI == false)
                itemBagePanel.RefreshPanel();
            else
                itemBagePanel.RefreshUI();
        }
    }
    #endregion

    #region IConditionTargetAble
    //获取等级
    public int GetLevel()
    {
        return GetWarrior().GetLevel();
    }
    //获取物品数量——特定物品版
    public DesignItems<Item> GetDesignItems(ItemQuality itemQuality, string itemName)
    {
        return playerBagMgr.GetMainBag().GetNameDesignItems(itemName, (item) => { return (item.itemQuality == itemQuality); });
    }
    //获取物品数量——物品类别版
    public DesignItems<Item> GetDesignItems_Category(ItemQuality itemQuality, ItemCategory itemCategory, EquipType equipType = EquipType.无)
    {
        DesignItems<Item> designItems = playerBagMgr.GetMainBag().GetCategoryDesignItems(itemCategory.ToString(), (item) =>
        {
            if ((item.itemQuality == itemQuality) && item.GetItemCategory() == itemCategory)
            {
                if (itemCategory == ItemCategory.Equip && equipType != EquipType.无)//指定了武器类型
                {
                    if (!(item is Equip) || (item as Equip).equipType != equipType)//但是物体不是武器或者武器类型不对应
                        return false;//返回不满足条件
                }
                return true;
            }
            return false;
        });
        return designItems;
    }
    #endregion

    #region ISpeakAble
    public TalkInfo GetBaseTalkInfo()
    {
        return null;
    }
    public List<Choice> GetChoices()
    {
        return null;
    }
    public string GetSpeakerName()
    {
        return GetPlayerData().playerName;
    }

    public string Relationship()
    {
        return "";
    }
    #endregion

    #region PlayerController
    public PlayerAnimState GetPlayerAnimState()
    {
        return playerController.GetPlayerAnimState();
    }
    public void Undress(WearEquipType equipType)
    {
        playerClother.Undress(equipType);
    }

    public Texture GetStaticAvatar(bool isReBuild = false)
    {
        return playerClother.GetStaticAvatar(isReBuild);
    }
    public Texture GetDynamicAvatar()
    {
        return playerClother.GetDynamicAvatar();
    }
    public void AvatarUsingEnd()
    {
        playerClother.AvatarUsingEnd();
    }
    public void WearClothes(WearEquipType equipType)
    {
        playerClother.WearClothes(equipType);
    }
    public PlayerController GetPlayerController()
    {
        return playerController;
    }
    public Warrior GetWarrior()
    {
        return playerController.GetWarrior();
    }
    public void NormalAttack()
    {
        //GetWarrior().NormalAttack();
    }
    public void TriggerSkill(string skillName)
    {
        //SkillInfoMgr skillInfoMgr = skillController.GetLearnedSkill(skillName);
        //skillInfoMgr.skill.TriggerSkill(GetWarrior(), null);
    }
    public void StartBattle(Enemy enemy)
    {
        playerController.StartBattle(enemy);
    }
    //设置玩家位置，主要用于初始化玩家位置
    public void SetPlayerPos(Vector3 targetPos)
    {
        playerController.SetPlayerPos(targetPos);
    }
    //移动到目标点
    public void MoveToTarget(Vector3 targetPos)
    {
        playerController.AstarMoveToTarget(targetPos);
    }
    public void MoveOneGrid(Vector3 targetPos)
    {
        playerController.MoveOneGrid(targetPos);
    }
    public void SetMoveOneGriding(bool isTrue)
    {
        playerController.SetMoveOneGriding(isTrue);
    }
    public float GetRunProcess()
    {
        return playerController.GetRunProcess();
    }
    public void StartRun()
    {
        playerController.StartRun();
    }

    public PlayerAnimatorController GetPlayerAnimContoller()
    {
        return GetWarrior().GetAnimatorController() as PlayerAnimatorController;
    }

    public Dictionary<string, string> GetForbidItemDic()
    {
        return null;
    }
    #endregion
}
