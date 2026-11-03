using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public interface IPresetObjAble
{
    //方法并没有什么用，只是为了确保预设物体有BoxCollider2D组件,用于检测
    public BoxCollider2D GetBoxCollider2D();
    //返回true，成为A*寻路的事件格子（事件格子的机制是平常寻路会当作障碍物处理，但是如果是寻路的最终位置，就会当作可行区域）
    public bool IsAstarEvent();

    //传入要出发预设物体世界的玩家，返回是否停止移动
    public bool PresetEventTrigger(IPlayerAble player);
}

[RequireComponent(typeof(BoxCollider2D))]
public class NPC : MonoBehaviour, IPresetObjAble, ISpeakAble
{
    [HideInInspector] public static Dictionary<string, NPC> allNPC = new Dictionary<string, NPC>();

    [SerializeField] protected string name_NPC;//所有NPC不可以重名
    [SerializeField] protected string currentScene;
    [SerializeField] protected TalkInfo baseTalkInfo;
    [SerializeField] protected List<Condition> firstAppearCondition = new List<Condition>();//有些NPC会在完成某些任务或达到某些条件出现

    [HideInInspector] protected BoxCollider2D coll;
    [HideInInspector] protected Animator anim;
    [HideInInspector] protected List<Task> myTasks;
    [HideInInspector] protected AvatarImage avatarImage = new AvatarImage();
    [HideInInspector] protected List<Choice> baseChoice_Code = new List<Choice>();
    public static NPC GetNPC(string name)
    {
        if (allNPC.ContainsKey(name))
            return allNPC[name];

        return null;
    }
    private void Awake()
    {
        TryGetComponent<BoxCollider2D>(out coll);
        if (coll == null)
            coll = gameObject.AddComponent<BoxCollider2D>();

        if (allNPC.ContainsKey(name_NPC))
            Debug.Log("已有重复名字：" + name_NPC);

        coll.size = new Vector2(0.8f, 0.8f);
        coll.offset = new Vector2(0, 0.4f);
        coll.isTrigger = true;

        Vector3 currentPos = gameObject.transform.position;
        gameObject.transform.position = new Vector3((int)currentPos.x + 0.5f, (int)currentPos.y + 0.105f, currentPos.z);

        anim = GetComponentInChildren<Animator>();
        allNPC[name_NPC] = this;
        myTasks = TaskMgr.GetInstance().GetTasks(name_NPC);
        this.InitialBaseChoice_Code();

        SortingGroup sortingGroup = anim.gameObject.GetComponent<SortingGroup>();
        if (sortingGroup == null)
            sortingGroup = anim.gameObject.AddComponent<SortingGroup>();
        sortingGroup.sortingLayerName = PlayerMgr.PlayerLayerName;
        sortingGroup.sortingOrder = PlayerMgr.PlayerLayerOrder;
    }

    private void OnDestroy()
    {
        if (allNPC.ContainsKey(name_NPC))
            allNPC.Remove(name_NPC);

        //AvatarUsingEnd();
    }
    public string GetSpeakerName()
    {
        return name_NPC;
    }
    public void AvatarUsingEnd()
    {
        avatarImage.UsingEnd();
    }
    
    protected virtual void InitialBaseChoice_Code()
    {
        
    }
    protected void AddBaseChoice_Code(Choice choice)
    {
        this.baseChoice_Code.Add(choice);
    }

    public Texture GetDynamicAvatar()
    {
        if (anim == null)
        {
            Debug.Log("动画控制器缺失");
            return null;
        }
        GameObject avatarObj = anim.gameObject;
        return avatarImage.GetDynamicTex(avatarObj, new Vector3(0, 1.0f, -2f));
    }

    public string Relationship()
    {
        return "";
    }

    public BoxCollider2D GetBoxCollider2D()
    {
        return coll;
    }

    public TalkInfo GetBaseTalkInfo()
    {
        return this.baseTalkInfo;
    }
    //调整与NPC对话是可以出现的选项，任务方面：只有任务的总发起人，阶段性任务发起人，阶段性任务的结束人，对话任务的对话目标可以出现对应的任务选项
    public List<Choice> GetChoices()
    {
        List<Choice> choices = new List<Choice>(baseTalkInfo.choices);//添加默认选择
        for (int i = 0; i < baseChoice_Code.Count; i++)
        {
            choices.Add(baseChoice_Code[i]);
        }
        if (myTasks != null)
        {
            for (int i = 0; i < myTasks.Count; i++)//添加自己作为任务的总发起人的选项
            {
                if (TaskMgr.GetInstance().IsFinishTak(myTasks[i].GetTaskName()))
                    continue;
                if (myTasks[i].IsAccessibility())
                    CreateOperateTaskChoice(choices, myTasks[i]);
            }
        }

        //以下处理，不管什么情况，一个最多都只会出现一次对同一个任务的处理，就算同时有多个身份
        //也就是说，就算此时的NPC应该是任务奖励领取人，但是他同时也是阶段任务发起人，那么根据下面的执行顺序，他会因为是阶段任务发起人而获得该任务的处理选项
        //任务处理选项，只是对选项添加该任务，具体的选项结果，由TalkButton根据任务状态来判断如何处理
        TaskMgr.GetInstance().ForeachAllProcessTasks((task) =>
        {
            //如果是总发起人，那么他已经再上面拥有了该任务选项,跳过
            if (task.GetTaskPromoterName() != this.name_NPC)
            {
                //添加自己作为对话任务的对话目标的选项
                if (task.IsHaveConditionType(ConditionType.对话) && task.GetTargetTalkNPCs().Contains(this.name_NPC) && task.GetCurrentSetpState() != TaskStepState.Receive)//如果是Receive阶段，说明该任务还未领取
                    CreateOperateTaskChoice(choices, task);
                //添加自己作为阶段性任务发起人的选项
                else if (task.GetReceiveTalkNPC_Name() == this.name_NPC)//在第二及以后的阶段，发起人也需要看到任务处理选项
                    CreateOperateTaskChoice(choices, task);
                //添加自己作为阶段性任务的结束人的选项
                else if (task.GetFinishTalkNPC_Name() == this.name_NPC && task.IsSatisfied() && task.GetCurrentSetpState() != TaskStepState.Receive)//领奖人只有在领奖阶段，或者是总发起人才可以拥有任务处理选项
                    CreateOperateTaskChoice(choices, task);
            }
            return false;//返回true则跳出循环
        });

        return choices;
    }
    private void CreateOperateTaskChoice(List<Choice> choices, Task task)
    {
        Choice choice = new Choice();
        choice.SetOperateTask(task);
        choices.Add(choice);
    }
    protected void ShowBaseSpeak()
    {
        UIManager.GetInstance().ShowPanel<TalkPanel>(TalkPanel.prefabsPath, PanelLayer.Top_Down, null, (TalkPanel panel) =>
        {
            panel.SetSpeak(this);
        });
    }
    public virtual bool PresetEventTrigger(IPlayerAble player)
    {
        ShowBaseSpeak();
        return true;
    }
    public virtual bool IsAstarEvent()
    {
        return true;
    }
}
