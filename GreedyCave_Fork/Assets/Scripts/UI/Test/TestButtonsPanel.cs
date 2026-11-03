using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButtonsPanel : ContainerPanel<TestButton,TestButtonInfo>
{
    public static string prefabsPath = "Test/TestButtonsPanel";

    private Transform childFather;

    public override void ShowMeFirst()
    {
        childFather = GetPanel("Scroll View/Viewport/Content").transform;
        SetButtonInfos(GetTestActions());
    }
    private List<TestButtonInfo> GetTestActions()
    {
        List<TestButtonInfo> testButtonInfos = new List<TestButtonInfo>();
        TestButtonInfo testButtonInfo_1 = new TestButtonInfo(TestAction_1,"回城");
        TestButtonInfo testButtonInfo_2 = new TestButtonInfo(TestAction_2, "回复状态");
        TestButtonInfo testButtonInfo_3 = new TestButtonInfo(TestAction_3, "升一级");
        TestButtonInfo testButtonInfo_4 = new TestButtonInfo(TestAction_4, "升五级");
        TestButtonInfo testButtonInfo_5 = new TestButtonInfo(TestAction_5, "下一层");
        TestButtonInfo testButtonInfo_6 = new TestButtonInfo(TestAction_6, "下五层");

        TestButtonInfo testButtonInfo_7 = new TestButtonInfo(null, "空");

        TestButtonInfo testButtonInfo_8 = new TestButtonInfo(TestAction_8, "开黄金箱一次");
        TestButtonInfo testButtonInfo_9 = new TestButtonInfo(TestAction_9, "开黄金箱五次");
        TestButtonInfo testButtonInfo_10 = new TestButtonInfo(TestAction_10, "开国王箱一次");
        TestButtonInfo testButtonInfo_11 = new TestButtonInfo(TestAction_11, "开国王箱五次");
        TestButtonInfo testButtonInfo_12 = new TestButtonInfo(TestAction_12, "开罐子一次");
        TestButtonInfo testButtonInfo_13 = new TestButtonInfo(TestAction_13, "开罐子五次");

        TestButtonInfo testButtonInfo_14 = new TestButtonInfo(TestAction_14, "测试");

        testButtonInfos.Add(testButtonInfo_1);
        testButtonInfos.Add(testButtonInfo_2);
        testButtonInfos.Add(testButtonInfo_3);
        testButtonInfos.Add(testButtonInfo_4);
        testButtonInfos.Add(testButtonInfo_5);
        testButtonInfos.Add(testButtonInfo_6);
        testButtonInfos.Add(testButtonInfo_7);
        testButtonInfos.Add(testButtonInfo_8);
        testButtonInfos.Add(testButtonInfo_9);
        testButtonInfos.Add(testButtonInfo_10);
        testButtonInfos.Add(testButtonInfo_11);
        testButtonInfos.Add(testButtonInfo_12);
        testButtonInfos.Add(testButtonInfo_13);

        testButtonInfos.Add(testButtonInfo_14);

        return testButtonInfos;
    }
    public void SetButtonInfos(List<TestButtonInfo> testButtonInfos)
    {
        RefreshContainer(testButtonInfos);
    }
    public override string GetCellPath()
    {
        return TestButton.prefabsPath;
    }
    public override Transform GetContainterTrans()
    {
        return childFather;
    }
    public override IHolderAble GetHolder()
    {
        return null;
    }
    public override ScrollRect GetScrollRect()
    {
        return null;
    }


    private void TestAction_1()
    {
        //回城
        GameManager.Instance.ChangeSceneAfterVignette(SceneName.MainCity);
    }
    private void TestAction_2()
    {
        //回复状态
        PlayerMgr.Instance.GetWarrior().RecoverState_Handler();
    }
    private void TestAction_3()
    {
        //升一级
        int level = PlayerMgr.Instance.GetWarrior().GetLevel();
        PlayerMgr.Instance.GetWarrior().LevelUp(level + 1, 0);
    }
    private void TestAction_4()
    {
        //升五级
        int level = PlayerMgr.Instance.GetWarrior().GetLevel();
        PlayerMgr.Instance.GetWarrior().LevelUp(level + 5, 0, true);

    }
    private void TestAction_5()
    {
        //下一层
        GameManager.Instance.ChangeSceneAfterVignette(SceneName.MazeScene, null, () =>
        {
            GameManager.Instance.NextLayer();
        });
    }
    private void TestAction_6()
    {
        if (GameManager.Instance.GetMapInfo() == null)
        {
            Debug.Log("缺失地图信息");
            return;
        }
        //下五层
        GameManager.Instance.ChangeSceneAfterVignette(SceneName.MazeScene, null, () =>
        {
            GameManager.Instance.RefreshTargetMap(GameManager.Instance.GetMapInfo().LayerCount + 5);
        });
    }
    private void TestAction_7()
    {
        
    }
    private void TestAction_8()
    {
        //开黄金箱一次
        Equip equip = TreasureBoxObj.OpenExcellentTreasureBox();
        PlayerMgr.Instance.OpenTreasureBox(equip);
    }
    private void TestAction_9()
    {
        //开黄金箱五次
        for (int i = 0; i < 5; i++)
        {
            Equip equip = TreasureBoxObj.OpenExcellentTreasureBox();
            PlayerMgr.Instance.OpenTreasureBox(equip);
        }
    }
    private void TestAction_10()
    {
        //开国王箱一次
        Equip equip = TreasureBoxObj.OpenKingTreasureBox();
        PlayerMgr.Instance.OpenTreasureBox(equip);
    }
    private void TestAction_11()
    {
        //开国王箱五次
        for (int i = 0; i < 5; i++)
        {
            Equip equip = TreasureBoxObj.OpenKingTreasureBox();
            PlayerMgr.Instance.OpenTreasureBox(equip);
        }
    }
    private void TestAction_12()
    {
        //开罐子一次
        PlayerMgr.Instance.UseBooty(JarObj.OpenJar_Test(PlayerMgr.Instance));
        
    }
    private void TestAction_13()
    {
        //开罐子五次
        for (int i = 0; i < 5; i++)
        {
            PlayerMgr.Instance.UseBooty(JarObj.OpenJar_Test(PlayerMgr.Instance));
        }
    }

    private void TestAction_14()
    {
        MoveControlPanel panel = (MoveControlPanel)UIManager.GetInstance().GetExistPanel(MoveControlPanel.prefabsPath);
        if (panel != null)
            panel.EndControl();
    }
}
