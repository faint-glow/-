using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class PlayerDataMgr
{
    public static int playerDataCount = 3;
    private int currrentPlayerDataIndex = 0;
    private bool isSaveing;
    private PlayerData playerData;

    public PlayerDataMgr()
    {
        AddPlayerChangeSceneEvent();
    }

    public void AddPlayerChangeSceneEvent()
    {
        EventCenter.GetInstance().AddEventListener<Scene>(EventCenterEnum.切换场景前.ToString(), BeforeChangeScene);
        EventCenter.GetInstance().AddEventListener<Scene>(EventCenterEnum.切换场景后.ToString(), AfterChangeScene);
    }
    public void RemovePlayerChangeSceneEvent()
    {
        EventCenter.GetInstance().RemoveEventListener<Scene>(EventCenterEnum.切换场景前.ToString(), BeforeChangeScene);
        EventCenter.GetInstance().RemoveEventListener<Scene>(EventCenterEnum.切换场景后.ToString(), AfterChangeScene);

    }

    private void BeforeChangeScene(Scene scene)
    {
        if (SceneController.Instance != null)
        {
            if (playerData == null)
                return;
            Dictionary<string, string> dirtySaveInfo = SceneController.Instance.GetSaveInfoDic_ChangeScene();
            SaveSceneInfoAided(dirtySaveInfo);
        }
    }
    private void AfterChangeScene(Scene scene)
    {
        if (SceneController.Instance != null)
        {
            if (playerData == null || playerData.sceneSaveInfoDic == null)
                return;

            SceneController.Instance.LoadSaveData_ChangeScene(playerData.sceneSaveInfoDic);
        }
    }
    private void SaveSceneInfoAided(Dictionary<string, string> dirtySaveInfo)
    {
        if (playerData.sceneSaveInfoDic == null)
            playerData.sceneSaveInfoDic = new Dictionary<string, string>();
        foreach (var item in dirtySaveInfo)
        {
            playerData.sceneSaveInfoDic[item.Key] = item.Value;
        }
    }

    public void SetCurrentDataIndex(int index)
    {
        currrentPlayerDataIndex = index;
    }
    public PlayerData GetPlayerData()
    {
        if (playerData == null)
            this.playerData = LoadPlayerData(currrentPlayerDataIndex);
        return playerData;
    }
    public void DeletePlayerData(int index)
    {
        if (IsHaveData(index) == false)
            return;
        SaveByJson.Delete(GetSavePath(index));
    }
    public List<PlayerData> GetAllPlayerData()
    {
        List<PlayerData> playerDatas = new List<PlayerData>();
        for (int i = 0; i < playerDataCount; i++)
        {
            PlayerData playerData = null;
            if (IsHaveData(i))
                playerData = LoadPlayerData(i);

            playerDatas.Add(playerData);
        }
        return playerDatas;
    }
    public string GetSavePath(int index)
    {
        return "Save/PlayerData_" + index + " .txt";
    }
    public bool IsHaveData(int index)
    {
        return SaveByJson.IsHaveFile(GetSavePath(index));
    }
    private PlayerData LoadPlayerData(int index)
    {
        if (IsHaveData(index) == false)
            return null;
        PlayerData playerData = (PlayerData)JsonConvert.DeserializeObject(SaveByJson.Read(GetSavePath(index)), typeof(PlayerData));
        return playerData;
    }
    //创建账号时会创建PlayerData数据
    public void SetPlayerData(PlayerData playerData)
    {
        this.playerData = playerData;
    }
    public void SaveData(Warrior warrior, bool isShowTip = true)
    {
        if (isSaveing == true)
            return;
        isSaveing = true;
        if (isShowTip)
            UIManager.GetInstance().ShowTips("保存中，请勿关闭游戏");

        if (PlayerMgr.Instance.IsPlayerExist() == false)
        {
            isSaveing = false;
            if (isShowTip == true)
                UIManager.GetInstance().ShowTips("保存失败");
            return;
        }
        SaveBaseData(warrior);
        SaveSceneInfo();
        SaveEquipInfo();
        SaveBageInfo();
        SaveTemporaryBagInfo();
        SaveDecorate();
        SaveSkillController();
        SaveShortcutController();
        SaveAudioController();
        SaveTaksMgrInfo();

        string content = JsonConvert.SerializeObject(playerData);
        SaveByJson.Save(GetSavePath(currrentPlayerDataIndex), content);

        isSaveing = false;
        if (isShowTip == true)
            UIManager.GetInstance().ShowTips("保存成功");

    }
    private void SaveBaseData(Warrior warrior)
    {
        playerData.level = warrior.GetLevel();
        playerData.currentHp = warrior.GetCurrentProperty(PropertyType.生命值, null);
        playerData.currentMp = warrior.GetCurrentProperty(PropertyType.魔法值, null);
        playerData.currentPhysiology = warrior.GetCurrentProperty(PropertyType.饱腹度, null);
        playerData.currentCoin = (int)warrior.GetCurrentProperty(PropertyType.金币, null);
        playerData.currentExperience = warrior.GetCurrentProperty(PropertyType.经验值, null);
    }
    private void SaveSceneInfo()
    {
        playerData.currentSceneName = SceneMgr.GetInstance().GetCurrentScene().name;
        playerData.pos_x = PlayerMgr.Instance.GetGirdPos().x;
        playerData.pos_y = PlayerMgr.Instance.GetGirdPos().y;
        playerData.pos_z = PlayerMgr.Instance.GetGirdPos().z;

        string sceneName = SceneMgr.GetInstance().GetCurrentScene().name;
        if (sceneName == SceneName.Lounge || sceneName == SceneName.BossRoom_1)
        {
            playerData.mapInfoName = GameManager.Instance.GetMapInfo().MapInfoName;
            playerData.layerCount = GameManager.Instance.GetMapInfo().LayerCount;
        }
        else if (sceneName == SceneName.MazeScene)
        {
            SaveMazeInfo();
        }

        if (SceneController.Instance != null)
            SaveSceneInfoAided(SceneController.Instance.GetSaveInfoDic());
    }
    private void SaveMazeInfo()
    {
        playerData.mapInfoName = GameManager.Instance.GetMapInfo().MapInfoName;
        playerData.layerCount = GameManager.Instance.GetMapInfo().LayerCount;
        playerData.mazsGrids = GameManager.Instance.GetRandomMaze().MapGrids;
    }
    private void SaveEquipInfo()
    {
        Dictionary<WearEquipType, List<Equip>> equipDic = PlayerMgr.Instance.GetEquips();
        Dictionary<WearEquipType, List<ItemSaveData>> itemSaveDataDic = new Dictionary<WearEquipType, List<ItemSaveData>>();
        foreach (var item in equipDic)
        {
            WearEquipType type = item.Key;
            for (int i = 0; i < item.Value.Count; i++)
            {
                Equip equip = item.Value[i];
                ItemSaveData data = null;
                if (equip != null)
                    data = equip.GetItemSaveData();
                if (!itemSaveDataDic.ContainsKey(type))
                    itemSaveDataDic[type] = new List<ItemSaveData>(1);
                itemSaveDataDic[type].Add(data);
            }
        }
        playerData.equipDic = itemSaveDataDic;
    }
    private void SaveBageInfo()
    {
        playerData.mainBag = GetBagSaveData(PlayerMgr.Instance.GetBag());
    }
    private void SaveTemporaryBagInfo()
    {
        BagMgr<Item> bagMgr = PlayerMgr.Instance.GetBagMgr();
        if (bagMgr == null)
            return;
        Dictionary<ITemporaryBagAble, Bag<Item>> temporaryBag = bagMgr.GetTemporaryBagDic();
        if (temporaryBag == null)
            return;

        List<BagSaveData> temporaryBagSaveInfo = new List<BagSaveData>();
        foreach (var item in temporaryBag)
        {
            Bag<Item> bag = item.Value;
            if (bag == null)
                continue;
            BagSaveData saveData = GetBagSaveData(bag);
            if (saveData == null)
                continue;
            temporaryBagSaveInfo.Add(saveData);
        }
        playerData.temporaryBag = temporaryBagSaveInfo;
    }
    private void SaveDecorate()
    {
        Dictionary<string, Dictionary<Vector3, IDecorateAble>> allDecorateDic = PlayerMgr.Instance.GetAllDecorates();
        Dictionary<string, Dictionary<string, ItemSaveData>> saveDecorateData = new Dictionary<string, Dictionary<string, ItemSaveData>>();
        foreach (var item in allDecorateDic)
        {
            string sceneName = item.Key;
            foreach (var itemElement in item.Value)
            {
                Vector3 decoratePos = itemElement.Key;
                IDecorateAble decorateAble = itemElement.Value;
                string myDecoratePos = PlayerDataMgr.Vector3ToString(decoratePos);
                Item saveItem = decorateAble as Item;
                ItemSaveData itemSaveData = null;
                if (saveItem != null)
                    itemSaveData = saveItem.GetItemSaveData();

                if (itemSaveData == null)
                {
                    Debug.Log("目标保存数据为空");
                    return;
                }
                if (!saveDecorateData.ContainsKey(sceneName))
                    saveDecorateData[sceneName] = new Dictionary<string, ItemSaveData>();

                saveDecorateData[sceneName][myDecoratePos] = itemSaveData;
            }
        }
        playerData.playerDecorateDic = saveDecorateData;
    }

    public void SaveSkillController()
    {
        SkillController skillController = PlayerMgr.Instance.GetSkillController();
        playerData.skillSaveData = skillController.GetSkillSaveData();
    }
    public void SaveShortcutController()
    {
        ShortcutController shortcutController = PlayerMgr.Instance.GetShortcutController();
        playerData.shortcutSaveData = shortcutController.GetShortcutSaveData();
    }

    public void SaveAudioController()
    {
        playerData.audioVolumeDic = AudioMgr.Instance.GetAudioVolumeDic();
    }

    public void SaveTaksMgrInfo()
    {
        playerData.taskMgrSaveInfo = TaskMgr.GetInstance().GetSaveInfo();
    }

    public static Vector3 StringToVector3(string content)
    {
        string[] strs = content.Split('|');
        if (strs.Length < 3)
            Debug.Log("数据有误");
        float x = float.Parse(strs[0]);
        float y = float.Parse(strs[1]);
        float z = float.Parse(strs[2]);
        return new Vector3(x, y, z);
    }
    public static string Vector3ToString(Vector3 vector)
    {
        return vector.x + "|" + vector.y + "|" + vector.z;
    }
    public static List<Bag<Item>> LoadBags(List<BagSaveData> bagSaveDatas, IHolderAble holder)
    {
        if (bagSaveDatas == null)
            return null;

        List<Bag<Item>> targetBags = new List<Bag<Item>>();
        for (int i = 0; i < bagSaveDatas.Count; i++)
        {
            if (bagSaveDatas[i] == null)
                continue;
            Bag<Item> bagItem = PlayerDataMgr.LoadBag(bagSaveDatas[i], holder);
            if (bagItem == null)
                continue;
            targetBags.Add(bagItem);
        }
        return targetBags;
    }
    public static Bag<Item> LoadBag(BagSaveData bagSaveData, IHolderAble holder)
    {
        if (bagSaveData == null)
            return null;

        Bag<Item> targetBag = new Bag<Item>(bagSaveData.allCapacity);
        targetBag.BeUseCapacity = bagSaveData.beUseCapacity;
        targetBag.UseCapacity = bagSaveData.useCapacity;
        for (int i = 0; i < bagSaveData.bag.Count; i++)
        {
            ItemSaveData itemSaveData = bagSaveData.bag[i];
            if (Item.AllItemInfoDic.ContainsKey(itemSaveData.itemName))
            {
                Item item = itemSaveData.GetItem();//Item.AllItemInfoDic[itemSaveData.itemName].GetItem(itemSaveData.itemQuality, itemSaveData.level);
                item.holder = holder;
                targetBag.CopyItem(item, i);
            }
        }
        return targetBag;
    }
    public static BagSaveData GetBagSaveData(Bag<Item> bag)
    {
        List<Item> itemList = bag.GetMainBag();
        List<ItemSaveData> itemSaveDatas = new List<ItemSaveData>();

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] == null)
                continue;
            ItemSaveData itemSaveData = itemList[i].GetItemSaveData();
            itemSaveDatas.Add(itemSaveData);
        }
        BagSaveData bagSaveData = new BagSaveData();
        bagSaveData.allCapacity = bag.OriginalCapacity;
        bagSaveData.bag = itemSaveDatas;
        bagSaveData.useCapacity = bag.UseCapacity;
        bagSaveData.beUseCapacity = bag.BeUseCapacity;
        return bagSaveData;
    }

}
