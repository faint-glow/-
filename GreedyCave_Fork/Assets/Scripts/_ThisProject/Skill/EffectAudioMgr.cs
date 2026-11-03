using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class EffectAudioMgr
{
    public List<AudioClipInfo> audioClips;//同时播放所有元素

    public void Play()
    {
        if (audioClips == null)
            return;
        for (int i = 0; i < audioClips.Count; i++)
        {
            audioClips[i].Play();
        }
    }
}
[System.Serializable]
public class AudioClipInfo
{
    public float delayTime;//延迟多少时间播放
    public List<AudioClip> randomAudioClips;//随机选取一个进行播放

    public void Play()
    {
        if (randomAudioClips == null || randomAudioClips.Count <= 0)
            return;
        MonoMgr.GetInstance().DelayAction(delayTime, () => 
        {
            int index = Random.Range(0, randomAudioClips.Count);
            AudioMgr.Instance.PlayEffect(randomAudioClips[index]);
        });
    }
}
