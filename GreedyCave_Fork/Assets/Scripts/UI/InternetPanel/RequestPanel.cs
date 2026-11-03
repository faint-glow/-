
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RequestPanel : MapPanel
{
    public static string prefabsPath = "InternetPanel/RequestPanel";

    private TMP_Text infoText;

    private Button buttonRefuse;
    private Button buttonAgree;
    public override void ShowMeFirst()
    {
        infoText = GetPanel("Main/Text (TMP)").GetUI<TMP_Text>();
        buttonRefuse = GetPanel("Main/Button").GetUI<Button>();
        buttonAgree = GetPanel("Main/Button (1)").GetUI<Button>();
    }

    public override void ShowMeEveryTime()
    {
        transform.localPosition = Vector3.zero;
    }

    public void SetRequestInfo(string content, UnityAction refuseAction, UnityAction agreeAction)
    {
        infoText.text = content;
        buttonRefuse.onClick.RemoveAllListeners();
        buttonRefuse.onClick.AddListener(() =>
        {
            if (refuseAction != null)
                refuseAction.Invoke();

            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
        });

        buttonAgree.onClick.RemoveAllListeners();
        buttonAgree.onClick.AddListener(() =>
        {
            if (agreeAction != null)
                agreeAction.Invoke();

            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
        });
    }

}
