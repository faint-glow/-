using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RoomPanel : MapPanel, ISetAble<IGetAble>
{
    public static string prefabsPath = "SetupPanel/Room/RoomPanel";
    public MapPanel FatherPanel { get; set; }
    public int Index { get; set; }

    public static RoomPanel SelectRoomPanel;
    private RoomInfo roomInfo;
    public RoomInfo _roomInfo
    {
        get { return roomInfo; }
    }
    private TMP_Text roomNameText;
    private TMP_Text playerCountText;
    private GameObject selectMask;
    public override void ShowMeFirst()
    {
        roomNameText = GetPanel("Text (TMP)").GetUI<TMP_Text>();
        playerCountText = GetPanel("Text (TMP) (1)").GetUI<TMP_Text>();
        selectMask = GetPanel("SelectMask").transform.gameObject;

        Button roomButton = ThisPanel.GetUI<Button>();
        roomButton.onClick.RemoveAllListeners();
        roomButton.onClick.AddListener(Click);
    }

    public override void ShowMeEveryTime()
    {
        RectTransform rectTrans = ThisPanel.transform.GetComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector2(540, 120);
        if(RoomPanel.SelectRoomPanel != this)
        {
            selectMask.SetActive(false);
        }
    }
    public void SetItem_Container(IGetAble obj)
    {
        RoomInfo roomInfo = obj as RoomInfo;
        this.roomInfo = roomInfo;
        roomNameText.text = "房间名:" + roomInfo.roomName;
        playerCountText.text = "房间人数:" + roomInfo.players.Count.ToString() + "/" + roomInfo.playerMaxCount;
    }

    public void Click()
    {
        if(RoomPanel.SelectRoomPanel != this)
        {
            selectMask.SetActive(true);
            RoomPanel.SelectRoomPanel = this;
        }
        else
        {
            selectMask.SetActive(false);
            RoomPanel.SelectRoomPanel = null;
        }
    }
}
