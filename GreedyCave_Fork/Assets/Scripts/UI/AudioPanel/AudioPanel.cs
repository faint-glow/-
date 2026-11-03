using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioPanel : MapPanel
{
    public static string prefabsPath = "AudioPanel";

    private Slider bgmSlider;
    private Slider effectSlider;

    public override void ShowMeFirst()
    {
        Button clickButton = ThisPanel.GetUI<Button>();
        clickButton.onClick.AddListener(() => 
        {
            AudioMgr.Instance.PlayEffect(AudioClipPath.click_1);
            UIManager.GetInstance().HidePanel(AudioPanel.prefabsPath);
        });

        bgmSlider = GetPanel("Main/BGM/Slider").GetUI<Slider>();
        bgmSlider.value = 1;
        effectSlider = GetPanel("Main/Effect/Slider").GetUI<Slider>();
        effectSlider.value = 1;

        bgmSlider.onValueChanged.AddListener((targetValue) => 
        {
            AudioMgr.Instance.SetVolume(AudioType.BGM,targetValue);
        });
        effectSlider.onValueChanged.AddListener((targetValue) =>
        {
            AudioMgr.Instance.SetVolume(AudioType.Effect,targetValue);
        });
    }

    public override void ShowMeEveryTime()
    {
        bgmSlider.value = AudioMgr.Instance.GetVolume(AudioType.BGM);
        effectSlider.value = AudioMgr.Instance.GetVolume(AudioType.Effect);
    }
}
