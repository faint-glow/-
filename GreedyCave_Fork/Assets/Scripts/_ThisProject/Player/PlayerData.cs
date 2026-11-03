using System.Collections;
using System.Collections.Generic;

public enum Gender
{
    男,
    女
}
[System.Serializable]
public class PlayerData:IGetAble
{
    public static string DefaultID = "1";
    public static Gender DefaultGender = Gender.男;
    public static float DefaultPhysiology = 100;
    public static string GetPlayerPrefabsPath(Gender gender)
    {
        string path = "Player/NewPlayer(man)";
        if (gender == Gender.男)
            path = "Player/NewPlayer(man)";
        else
            path = "Player/NewPlayer(woman)";
        return path;
    }

    public string ID = "1";
    public string playerName = "暂未命名";
    public Gender gender = Gender.男;
    public int maxLayerCount = 1;//已经到达过的最大层数

    public int level = 1;
    public float currentHp = 0;
    public float currentMp = 0;
    public float currentPhysiology = 100;
    public float currentCoin = 0;
    public float currentExperience = 0;//玩家退出后，用于保存玩家当前的经验值，以便恢复
    
    public string currentSceneName;
    public int pos_x;
    public int pos_y;
    public int pos_z;

    public string mapInfoName;
    public int layerCount;

    public Dictionary<string, float> bankDic = new Dictionary<string, float>();
    public Dictionary<string, string> sceneSaveInfoDic;
    public Dictionary<AudioType, float> audioVolumeDic = new Dictionary<AudioType, float>() { { AudioType.BGM,1}, { AudioType.Effect, 1 } };
    public Dictionary<WearEquipType, List<ItemSaveData>> equipDic;
    public Dictionary<string, Dictionary<string, ItemSaveData>> playerDecorateDic;
    public MazeGrid[,] mazsGrids;
    public BagSaveData mainBag = new BagSaveData();
    public List<BagSaveData> temporaryBag = new List<BagSaveData>();

    public SkillSaveData skillSaveData;
    public ShortcutSaveData shortcutSaveData;
    public TaskMgrSaveInfo taskMgrSaveInfo;
}

[System.Serializable]
public class BagSaveData
{
    public static int DefaultCapacity = 50;
    public int allCapacity = 50;
    public int useCapacity = 0;
    public int beUseCapacity = 0;
    public List<ItemSaveData> bag = new List<ItemSaveData>();
}