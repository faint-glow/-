using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IDecorateAble
{
    public string GetName();
    public IHolderAble GetHolder();
    public Vector2 GetSize();
    public Sprite GetSprite();
    //是否可以回收，如果不可以，返回为什么不可以回收的提示
    public bool IsCanRecycle(out string tip);
    //返回true，成为A*寻路的事件格子（事件格子的机制是平常寻路会当作障碍物处理，但是如果是寻路的最终位置，就会当作可行区域）
    public bool IsAstarEvent();
    //传入要出发预设物体世界的玩家，返回是否停止移动
    public bool PresetEventTrigger(IPlayerAble player);

}
public class DecorateMgr : BaseManager<DecorateMgr>
{
    private GameObject decorateGridPrefab;
    public GameObject DecorateGridPrefab
    {
        get
        {
            if (decorateGridPrefab == null)
                decorateGridPrefab = ResMgr.GetInstance().Load<GameObject>("Decorate/DecorateGrid", false);
            return decorateGridPrefab;
        }
    }

    //场景名对应装扮
    //public static Dictionary<string, IDecorateAble[,]> sceneDecorateAble = new Dictionary<string, IDecorateAble[,]>();
    private Transform decorateGridFather;
    private IDecorateAble currentdecorateAble;
    private DecorateObj currentDecorateObj;
    private List<DecorateGrid> lastBlueGrid = new List<DecorateGrid>();
    //相机控制
    private CinemachineVirtualCamera virtualCamera;
    private float cameraDistance;
    private Vector3 targetPos;
    private Vector3 dragStartPos_Screen;
    private Vector3 dragStartPos, dragCurrentPos;

    private bool lastIsStartScale = false;//安卓版控制需要

    public void SelectObj(IDecorateAble decorateAble)
    {
        this.currentdecorateAble = decorateAble;
        this.currentDecorateObj = CreateDecorateObj(decorateAble, Vector3.zero);
    }
    public void PlaceObj(Vector3 targetPos)
    {
        targetPos = new Vector3((int)targetPos.x + 0.5f, (int)targetPos.y + 0.5f, 0);
        List<DecorateGrid> decorateGrids = ToolsMgr.CheckPrestEvents<DecorateGrid>(targetPos);
        bool isAccessible = true;
        if (decorateGrids == null || decorateGrids.Count <= 0)
        {
            isAccessible = false;
            UIManager.GetInstance().ShowTips("此位置不可放置");
        }
        else
        {
            for (int i = 0; i < decorateGrids.Count; i++)
            {
                if (decorateGrids[i].IsAccessible() == false)
                {
                    isAccessible = false;
                    UIManager.GetInstance().ShowTips("此位置不可放置");
                    break;
                }
            }
        }
        if (isAccessible == true)
        {
            if (currentDecorateObj.transform.position == targetPos)//再次点击同一位置
            {
                for (int i = 0; i < decorateGrids.Count; i++)//把目标位置设置成红色格子
                {
                    decorateGrids[i].SetSprite_Red();
                    if (SceneController.Instance != null)
                        SceneController.Instance.SetAstarEventGrid((int)targetPos.x, (int)targetPos.y, E_Node_Type.Event);
                }
                lastBlueGrid.Clear();
                Confirm(targetPos);
            }
            else//点击一个和上一次点击不同的位置或者第一次点击位置
            {
                for (int i = 0; i < lastBlueGrid.Count; i++)//还原之前的蓝色格子
                    lastBlueGrid[i].SetSprite_White();
                lastBlueGrid.Clear();
                for (int i = 0; i < decorateGrids.Count; i++)//设置新的蓝色格子
                {
                    decorateGrids[i].SetSprite_Blue();
                    lastBlueGrid.Add(decorateGrids[i]);
                }
                currentDecorateObj.transform.position = targetPos;
            }
        }
    }
    public void Confirm(Vector3 targetPos)
    {
        currentdecorateAble = currentDecorateObj.Confirm();
        PlayerMgr.Instance.AddDecorate(targetPos, currentdecorateAble);

        CloseDecoratePanel();
    }

    public void ShowDecorate(Vector3 targetPos, IDecorateAble decorateAble)
    {
        if (SceneController.Instance != null)
            SceneController.Instance.SetAstarEventGrid((int)targetPos.x, (int)targetPos.y, E_Node_Type.Event);

        CreateDecorateObj(decorateAble, targetPos);
        CloseDecoratePanel();
    }

    private DecorateObj CreateDecorateObj(IDecorateAble decorateAble, Vector3 targetPos)
    {
        GameObject obj = new GameObject();
        obj.transform.position = targetPos;
        DecorateObj decorateObj = obj.AddComponent<DecorateObj>();
        decorateObj.SetDecorate(decorateAble, 1);
        return decorateObj;
    }

    public void OpenDecoratePanel()
    {
        if (decorateGridFather != null)
        {
            decorateGridFather.gameObject.SetActive(true);

            ControlCamera();
            InputMgr.GetInstance().AddMouseInput(InputType.DecorateMouseInput, DecorateMouseInput_Android);
            return;
        }

        GameObject obj = new GameObject("DecorateGridFather");
        decorateGridFather = obj.transform;

        if (SceneController.Instance != null)
        {
            if (SceneMgr.GetInstance().GetCurrentScene().name != SceneName.MainCity)
            {
                UIManager.GetInstance().ShowTips("当前场景不可放置");
                return;
            }
            Tilemap tilemap = SceneController.Instance.GetRoadTilemap();

            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)//遍历所有Tilemap的Tile的位置
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                Vector3 place = tilemap.CellToWorld(localPlace);
                if (tilemap.HasTile(localPlace))
                {

                    place = new Vector3(place.x + 0.5f, place.y + 0.5f, 0);
                    GameObject grid = GameObject.Instantiate(DecorateGridPrefab, place, Quaternion.identity, decorateGridFather);
                }
            }
            ControlCamera();

            if (Application.platform == RuntimePlatform.Android)
                InputMgr.GetInstance().AddMouseInput(InputType.DecorateMouseInput, DecorateMouseInput_Android);
            else
                InputMgr.GetInstance().AddMouseInput(InputType.DecorateMouseInput, DecorateMouseInput_PC);
        }
        else
        {
            UIManager.GetInstance().ShowTips("当前场景不可放置");
        }

    }

    public void CloseDecoratePanel()
    {
        if (decorateGridFather != null)
            decorateGridFather.gameObject.SetActive(false);
        if (virtualCamera != null)
            GameObject.Destroy(virtualCamera.gameObject);
        currentdecorateAble = null;
        currentDecorateObj = null;
        CameraMgr.GetInstance().SeBlendCameraType(CameraMgr.GetInstance().defaultBlendType);
        InputMgr.GetInstance().RemoveMouseInput(InputType.DecorateMouseInput);
    }

    private void ControlCamera()
    {
        CinemachineFramingTransposer virtualCameraBody;
        CameraMgr.GetInstance().SeBlendCameraType(CinemachineBlendDefinition.Style.Cut);
        virtualCamera = CameraMgr.GetInstance().CreateNormalCamera(out virtualCameraBody);
        virtualCamera.Priority = 110;
        virtualCamera.transform.position = CameraMgr.GetInstance().GetCamera().transform.position;

        targetPos = virtualCamera.transform.position;
        cameraDistance = virtualCamera.m_Lens.OrthographicSize;
    }
    private void DecorateMouseInput_PC()
    {
        if (virtualCamera == null)
        {
            InputMgr.GetInstance().RemoveMouseInput(InputType.DecorateMouseInput);
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousScreenPos = Input.mousePosition;
            dragStartPos_Screen = Input.mousePosition;
            dragStartPos = CameraMgr.GetInstance().GetCamera().ScreenToWorldPoint(mousScreenPos);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 mousScreenPos = Input.mousePosition;
            dragCurrentPos = CameraMgr.GetInstance().GetCamera().ScreenToWorldPoint(mousScreenPos);
            Vector3 difference = dragStartPos - dragCurrentPos;
            targetPos = virtualCamera.transform.position + difference;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 endPos = CameraMgr.GetInstance().GetCamera().ScreenToWorldPoint(Input.mousePosition);
            if ((Input.mousePosition - dragStartPos_Screen).sqrMagnitude < 20)
                PlaceObj(endPos);
        }
        cameraDistance += -Input.mouseScrollDelta.y;
        cameraDistance = Mathf.Clamp(cameraDistance, 1, 20);
        Vector3 pos = Vector3.Lerp(virtualCamera.transform.position, targetPos, 20 * Time.deltaTime);
        virtualCamera.transform.position = pos;
        virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, cameraDistance, Time.deltaTime * 10);
    }

    private void DecorateMouseInput_Android()
    {
        if (virtualCamera == null)
        {
            InputMgr.GetInstance().RemoveMouseInput(InputType.DecorateMouseInput);
            return;
        }
        if (lastIsStartScale == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousScreenPos = Input.mousePosition;
                dragStartPos_Screen = Input.mousePosition;
                dragStartPos = CameraMgr.GetInstance().GetCamera().ScreenToWorldPoint(mousScreenPos);
            }
            if (Input.GetMouseButton(0))
            {
                Vector3 mousScreenPos = Input.mousePosition;
                dragCurrentPos = CameraMgr.GetInstance().GetCamera().ScreenToWorldPoint(mousScreenPos);

                Vector3 difference = dragStartPos - dragCurrentPos;
                targetPos = virtualCamera.transform.position + difference;
            }
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 endPos = CameraMgr.GetInstance().GetCamera().ScreenToWorldPoint(Input.mousePosition);
                if ((Input.mousePosition - dragStartPos_Screen).sqrMagnitude < 20)
                    PlaceObj(endPos);
            }
        }

        float delta = -InputMgr.GetInstance().GetTouchScale() * 0.005f;
        bool currentIsStartScale = InputMgr.GetInstance().IsStartScale();
        cameraDistance += delta;
        cameraDistance = Mathf.Clamp(cameraDistance, 1, 20);
        Vector3 pos = Vector3.Lerp(virtualCamera.transform.position, targetPos, 20 * Time.deltaTime);
        if (currentIsStartScale == false)
        {
            if (lastIsStartScale == true)
            {
                dragStartPos = CameraMgr.GetInstance().GetCamera().ScreenToWorldPoint(Input.mousePosition);
                targetPos = virtualCamera.transform.position;
            }
            else
            {
                virtualCamera.transform.position = pos;
            }
        }
        virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, cameraDistance, Time.deltaTime * 20);

        lastIsStartScale = currentIsStartScale;
    }


    private void MoveByButton()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {

                }
            }

        }
    }
}