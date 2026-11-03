using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    Effect,
    BGM,
}
public class AudioClipPath
{
    public static string MainClipPath = "Audio/MainClip/";

    //BGM
    public static string MainBGMclipPath = MainClipPath + "BGMclips/";
    public static string start = MainBGMclipPath + "Start";
    public static string mainCity = MainBGMclipPath + "MainCity";

    //BGM/Cave
    private static AudioClip[] randomCave;
    public static AudioClip[] RandomCave
    {
        get
        {
            if (randomCave == null || randomCave.Length <= 0)
                randomCave = ResMgr.GetInstance().LoadAll<AudioClip>(MainClipPath + "RandomCave");
            return randomCave;
        }
    }

    //UI
    public static string MainUIclipPath = MainClipPath + "UIclips/";
    public static string click_1 = MainUIclipPath + "click_1";
    public static string click_2 = MainUIclipPath + "click_2";
    public static string close = MainUIclipPath + "close";
    public static string open = MainUIclipPath + "open";
    public static string change = MainUIclipPath + "change";
    public static string equip = MainUIclipPath + "equip_put_on";

    //Behaviour
    public static string MainBehaviourClips = MainClipPath + "MainBehaviourClips/";
    public static string openJar = MainBehaviourClips + "Item/OpenJar";
    public static string openTreasureBox = MainBehaviourClips + "Item/OpenTreasureBox";

    //Behaviour/Coin
    public static string changeCoin = MainBehaviourClips + "Coin/money";

    public static string GetBGM_SceneName(string sceneName)
    {
        return MainBGMclipPath + sceneName;
    }
}

public class AudioSourceManager
{
    private List<AudioSource> freeAudioSource = new List<AudioSource>();
    private List<AudioSource> busyAudioSource = new List<AudioSource>();

    public void SetVolume(float value)
    {
        for (int i = 0; i < freeAudioSource.Count; i++)
        {
            freeAudioSource[i].volume = value;
        }
        for (int i = 0; i < busyAudioSource.Count; i++)
        {
            busyAudioSource[i].volume = value;
        }
    }
    public AudioSourceManager()
    {
        MonoMgr.GetInstance().AddUpdateListener(Updata);
    }
    public string GetPrintInfo()
    {
        string countent = "空闲：" + freeAudioSource.Count;
        countent += "播放中：" + busyAudioSource.Count;
        return countent;
    }
    private void Updata()
    {
        if (busyAudioSource == null || busyAudioSource.Count <= 0)
            return;
        if (freeAudioSource == null)
            return;
        for (int i = 0; i < busyAudioSource.Count; i++)
        {
            if (busyAudioSource[i].isPlaying == false)
            {
                AudioSource source = busyAudioSource[i];
                busyAudioSource.RemoveAt(i);
                freeAudioSource.Add(source);
                i--;
            }
        }
    }
    public AudioSource GetFreeSource()
    {
        if (freeAudioSource == null)
            freeAudioSource = new List<AudioSource>();
        if (freeAudioSource.Count <= 0 && busyAudioSource.Count < AudioMgr.MaxSourceCount)
            freeAudioSource.Add(AudioMgr.Instance.CreateAudioSource());
        if (freeAudioSource.Count <= 0)
        {
            if (busyAudioSource.Count > 0)
                return GetBusySource();
            return null;
        }
        AudioSource source = freeAudioSource[freeAudioSource.Count - 1];
        freeAudioSource.RemoveAt(freeAudioSource.Count - 1);//先移除，这样当找这个还没有被使用时，下一个来的就可以使用另一个
        float waitTime = 0;
        MonoMgr.GetInstance().WaitUntil(() =>
        {
            waitTime += Time.deltaTime;
            if (waitTime >= 3)//超过3秒这个Source还没有被使用就直接释放该协程，避免一直等待
            {
                freeAudioSource.Add(source);//并让该Source成为空闲的
                return true;
            }
            if (source.isPlaying == true)//因为AudioClip是异步加载，所以要等待他开始播放才可以交换
            {
                busyAudioSource.Add(source);
                return true;
            }
            return false;
        }, null);
        return source;
    }
    public AudioSource GetBusySource()
    {
        if (busyAudioSource == null)
            busyAudioSource = new List<AudioSource>();
        if (busyAudioSource.Count <= 0)
            return GetFreeSource();

        return busyAudioSource[0];
    }
}
public class AudioMgr : Singleton<AudioMgr>
{
    #region AudioMixer信息
    //private static float defaultValue = 1;
    //public AudioMixer audioMaxer;
    //private AudioMixerGroup[] groups;
    //if(groups == null)
    //{
    //    audioMaxer = Resources.Load<AudioMixer>("Main");
    //    groups = audioMaxer.FindMatchingGroups("Master");//通过/分割，可以找到子Group的所有子Group
    //}
    //if (sourceType == AudioType.BGM)
    //    musicDic[sourceType].outputAudioMixerGroup = groups[1];
    //else
    //    musicDic[sourceType].outputAudioMixerGroup = groups[2];
    #endregion
    #region 测试信息
    public void PringInfo()
    {
        foreach (var item in musicDic)
        {
            Debug.Log(item.Key + "：" + item.Value.GetPrintInfo());
        }
    }
    public void Play()
    {
        if (musicDic.ContainsKey(AudioType.BGM))
        {
            musicDic[AudioType.BGM].GetBusySource().Play();
            //musicDic[AudioType.BGM].Play();
        }
    }
    public void Pause()//暂停后再次播放，从停止播放的地方开始播放
    {
        if (musicDic.ContainsKey(AudioType.BGM))
            musicDic[AudioType.BGM].GetBusySource().Pause();
    }
    public void Stop()//停止后再次播放，从播放开始的地方播放
    {
        if (musicDic.ContainsKey(AudioType.BGM))
            musicDic[AudioType.BGM].GetBusySource().Stop();
    }

    public bool IsPlaying()
    {
        if (!musicDic.ContainsKey(AudioType.BGM))
            return false;

        return musicDic[AudioType.BGM].GetBusySource().isPlaying;
    }
    #endregion
    public static int MaxSourceCount = 10;
    public static Dictionary<AudioType, float> DefaultAudioVolumeDic = new Dictionary<AudioType, float>() { { AudioType.BGM, 1 }, { AudioType.Effect, 1 } };

    private GameObject obj = null;
    private Dictionary<string, AudioClip> clipDic = new Dictionary<string, AudioClip>();
    private Dictionary<AudioType, AudioSourceManager> musicDic = new Dictionary<AudioType, AudioSourceManager>();
    private Dictionary<AudioType, float> audioVolumeDic = new Dictionary<AudioType, float>();

    public void ClearRunTimeData()
    {
        this.audioVolumeDic = new Dictionary<AudioType, float>();
    }

    public void PlayBGM(string clipName, float value = -1, bool isLoop = true, bool isSmooth = true, float smoothTime = 0.5f)
    {
        PlayBusySource(clipName, AudioType.BGM, GetVolume(AudioType.BGM, value), isLoop, isSmooth, smoothTime);
    }
    public void PlayBGM(AudioClip clip, float value = -1, bool isLoop = true, bool isSmooth = true, float smoothTime = 0.5f)
    {
        PlayBusySource(clip, AudioType.BGM, GetVolume(AudioType.BGM, value), isLoop, isSmooth, smoothTime);
    }
    public void PlayEffect(string clipName, float value = -1, bool isLoop = false, bool isSmooth = false, float smoothTime = 0.5f)
    {
        PlayFreeSource(clipName, AudioType.Effect, GetVolume(AudioType.Effect, value), isLoop, isSmooth, smoothTime);
    }
    public void PlayEffect(AudioClip clip, float value = -1, bool isLoop = false, bool isSmooth = false, float smoothTime = 0.5f)
    {
        PlayFreeSource(clip, AudioType.Effect, GetVolume(AudioType.Effect, value), isLoop, isSmooth, smoothTime);
    }
    public Dictionary<AudioType, float> GetAudioVolumeDic()
    {
        return audioVolumeDic;
    }
    public void SetAudioVolumeDic(Dictionary<AudioType, float> audioVolumeDic)
    {
        this.audioVolumeDic = audioVolumeDic;
    }
    public void SetVolume(AudioType sourceType, float value)
    {
        audioVolumeDic[sourceType] = value;
        if (!musicDic.ContainsKey(sourceType))
            return;
        musicDic[sourceType].SetVolume(value);
    }
    public float GetVolume(AudioType sourceType, float value = -1)
    {
        if (!audioVolumeDic.ContainsKey(sourceType))
            audioVolumeDic[sourceType] = 1;

        if (value < 0)//小于0则使用默认从音量
            value = audioVolumeDic[sourceType];
        else//大于等于0则受到整体通道的影响
            value *= audioVolumeDic[sourceType];
        return value;
    }
    public AudioSource CreateAudioSource()
    {
        return obj.AddComponent<AudioSource>();
    }

    private void Initial(AudioType sourceType)
    {
        if (obj == null)
        {
            obj = new GameObject("MusicMgr");
            obj.transform.SetParent(GameManager.Instance.transform);//让obj(MusicMgr)成为GameManager的子物体，防止切换场景时被销毁
        }
        if (!musicDic.ContainsKey(sourceType))
        {
            musicDic.Add(sourceType, new AudioSourceManager());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clipName">想要加载的音乐片段的名字</param>
    /// <param name="sourceType">一个音乐种类的名字,此值意味着AudioSource</param>
    /// <param name="isLoop">是否循环播放，默认值为false</param>
    private void PlayFreeSource(string clipName, AudioType sourceType, float value = 1, bool isLoop = false, bool isSmooth = false, float smoothTime = 0.5f)
    {
        Initial(sourceType);
        AudioSource source = musicDic[sourceType].GetFreeSource();
        Play(clipName, source, value, isLoop, isSmooth, smoothTime);
    }
    private void PlayFreeSource(AudioClip clip, AudioType sourceType, float value = 1, bool isLoop = false, bool isSmooth = false, float smoothTime = 0.5f)
    {
        Initial(sourceType);
        AudioSource source = musicDic[sourceType].GetFreeSource();
        Play(clip, source, value, isLoop, isSmooth, smoothTime);
    }
    private void PlayBusySource(string clipName, AudioType sourceType, float value = 1, bool isLoop = false, bool isSmooth = false, float smoothTime = 0.5f)
    {
        Initial(sourceType);
        AudioSource source = musicDic[sourceType].GetBusySource();
        Play(clipName, source, value, isLoop, isSmooth, smoothTime);
    }
    private void PlayBusySource(AudioClip clip, AudioType sourceType, float value = 1, bool isLoop = false, bool isSmooth = false, float smoothTime = 0.5f)
    {
        Initial(sourceType);
        AudioSource source = musicDic[sourceType].GetBusySource();
        Play(clip, source, value, isLoop, isSmooth, smoothTime);
    }

    private void Play(string clipName, AudioSource source, float value = 1, bool isLoop = false, bool isSmooth = false, float smoothTime = 0.5f)
    {
        if (source == null)
            return;
        if (clipDic.ContainsKey(clipName))
        {
            Play(clipDic[clipName], source, value, isLoop, isSmooth, smoothTime);
        }
        else
        {
            ResMgr.GetInstance().LoadAsync(clipName, (AudioClip clip) =>
            {
                if (clip == null)
                    return;
                clipDic[clipName] = clip;
                Play(clipDic[clipName], source, value, isLoop, isSmooth, smoothTime);
            });
        }
    }
    private void Play(AudioClip clip, AudioSource source, float value = 1, bool isLoop = false, bool isSmooth = false, float smoothTime = 0.5f)
    {
        if (isSmooth == false)
            PlayClip(source, clip, value, isLoop);
        else
            ChangeClipSmooth(source, clip, value, isLoop, smoothTime);
    }
    private void PlayClip(AudioSource source, AudioClip clip, float value, bool isLoop)
    {
        if (source == null)
            return;
        source.clip = clip;
        source.loop = isLoop;
        source.volume = value;
        source.Play();
    }
    public void ChangeClipSmooth(AudioSource source, AudioClip clip, float targetValue, bool isLoop, float smoothTime = 0.5f)
    {
        if (source == null)
            return;
        if (source.clip == null || source.volume == 0 || source.isPlaying == false)
        {
            PlayClipSmooth(source, clip, targetValue, isLoop, smoothTime);
            return;
        }
        StopClipSmooth(source, 0, smoothTime, (CountdownTimerInfo countdownTimerInfo) =>
        {
            PlayClipSmooth(source, clip, targetValue, isLoop, smoothTime);
        });
    }
    private void StopClipSmooth(AudioSource source, float targetValue = 0, float smoothTime = 0.5f, Action<CountdownTimerInfo> afterAction = null)
    {
        if (source == null)
            return;
        OperiateClipSmooth(source, targetValue, smoothTime, (CountdownTimerInfo countdownTimerInfo) =>
        {
            source.Stop();
            if (afterAction != null)
                afterAction.Invoke(countdownTimerInfo);
        });
    }
    private void PlayClipSmooth(AudioSource source, AudioClip clip, float targetValue, bool isLoop, float smoothTime = 0.5f, Action<CountdownTimerInfo> afterAction = null)
    {
        if (source == null)
            return;
        PlayClip(source, clip, 0, isLoop);
        OperiateClipSmooth(source, targetValue, smoothTime, afterAction);
    }
    private void OperiateClipSmooth(AudioSource source, float targetValue, float smoothTime = 0.5f, Action<CountdownTimerInfo> afterAction = null)
    {
        if (source == null)
            return;

        float valueArea = targetValue - source.volume;
        MonoMgr.GetInstance().CountdownTimer(smoothTime, null, (countdownTimeInfo) =>
        {
            source.volume += (valueArea / smoothTime) * Time.deltaTime;
        }, afterAction);
    }
}

//public class AudioMgr : Singleton<AudioMgr>
//{
//    #region AudioMixer信息
//    //private static float defaultValue = 1;
//    //public AudioMixer audioMaxer;
//    //private AudioMixerGroup[] groups;
//    //if(groups == null)
//    //{
//    //    audioMaxer = Resources.Load<AudioMixer>("Main");
//    //    groups = audioMaxer.FindMatchingGroups("Master");//通过/分割，可以找到子Group的所有子Group
//    //}
//    //if (sourceType == AudioType.BGM)
//    //    musicDic[sourceType].outputAudioMixerGroup = groups[1];
//    //else
//    //    musicDic[sourceType].outputAudioMixerGroup = groups[2];
//    #endregion
//    public static int MaxSourceCount = 10;

//    private GameObject obj = null;
//    private Dictionary<string, AudioClip> clipDic = new Dictionary<string, AudioClip>();
//    private Dictionary<AudioType,AudioSource> musicDic = new Dictionary<AudioType, AudioSource>();

//    public void PlayeBGM(string clipName, float value = 1, bool isLoop = true, bool isSmooth = true, float smoothTime = 0.5f)
//    {
//        Play(clipName, AudioType.BGM, value, isLoop, isSmooth, smoothTime);
//    }
//    public void PlayEffect(string clipName, AudioType sourceType = AudioType.Effect, float value = 1, bool isLoop = false, bool isSmooth = false, float smoothTime = 0.5f)
//    {
//        Play(clipName, sourceType, value, isLoop, isSmooth, smoothTime);
//    }
//    public void PlayEffect(AudioType sourceType, AudioClip clip, float value = 1)
//    {
//        //PlayClip(AudioType.EffectOne)
//    }

//    public AudioSource CreateAudioSource()
//    {
//        return obj.AddComponent<AudioSource>();
//    }
//    #region 测试信息
//    public void Play()
//    {
//        if (musicDic.ContainsKey(AudioType.BGM))
//            musicDic[AudioType.BGM].Play();
//    }
//    public void Pause()//暂停后再次播放，从停止播放的地方开始播放
//    {
//        if (musicDic.ContainsKey(AudioType.BGM))
//            musicDic[AudioType.BGM].Pause();
//    }
//    public void Stop()//停止后再次播放，从播放开始的地方播放
//    {
//        if (musicDic.ContainsKey(AudioType.BGM))
//            musicDic[AudioType.BGM].Stop();
//    }

//    public bool IsPlaying()
//    {
//        if (!musicDic.ContainsKey(AudioType.BGM))
//            return false;
//        return musicDic[AudioType.BGM].isPlaying;
//    }
//    #endregion

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="clipName">想要加载的音乐片段的名字</param>
//    /// <param name="sourceType">一个音乐种类的名字,此值意味着AudioSource</param>
//    /// <param name="isLoop">是否循环播放，默认值为false</param>
//    public void Play(string clipName, AudioType sourceType, float value = 1, bool isLoop = false, bool isSmooth = false, float smoothTime = 0.5f)
//    {
//        if (obj == null)
//        {
//            obj = new GameObject("MusicMgr");
//            obj.transform.SetParent(GameManager.Instance.transform);//让obj(MusicMgr)成为GameManager的子物体，防止切换场景时被销毁
//        }
//        if (!musicDic.ContainsKey(sourceType))
//        {
//            musicDic.Add(sourceType, obj.AddComponent<AudioSource>());
//        }
//        if (clipDic.ContainsKey(clipName))
//        {
//            if (isSmooth == false)
//                PlayClip(sourceType, clipDic[clipName], value, isLoop);
//            else
//                ChangeClipSmooth(sourceType, clipDic[clipName], value, isLoop, smoothTime);
//        }
//        else
//        {
//            ResMgr.GetInstance().LoadAsync(clipName, (AudioClip clip) =>
//            {
//                clipDic.Add(clipName, clip);
//                if (isSmooth == false)
//                    PlayClip(sourceType, clip, value, isLoop);
//                else
//                    ChangeClipSmooth(sourceType, clipDic[clipName], value, isLoop, smoothTime);
//            });
//        }
//    }
//    private void PlayClip(AudioType sourceType, AudioClip clip, float value, bool isLoop)
//    {
//        musicDic[sourceType].clip = clip;
//        musicDic[sourceType].loop = isLoop;
//        musicDic[sourceType].volume = value;
//        musicDic[sourceType].Play();
//    }
//    public void ChangeClipSmooth(AudioType sourceType, AudioClip clip, float targetValue, bool isLoop, float smoothTime = 0.5f)
//    {
//        if (!musicDic.ContainsKey(sourceType))
//            return;
//        if (musicDic[sourceType].clip == null || musicDic[sourceType].volume == 0 || musicDic[sourceType].isPlaying == false)
//        {
//            PlayClipSmooth(sourceType, clip, targetValue, isLoop, smoothTime);
//            return;
//        }
//        StopClipSmooth(sourceType, 0, smoothTime, () =>
//          {
//              PlayClipSmooth(sourceType, clip, targetValue, isLoop, smoothTime);
//          });
//    }

//    private void StopClipSmooth(AudioType sourceType, float targetValue = 0, float smoothTime = 0.5f, Action afterAction = null)
//    {
//        if (!musicDic.ContainsKey(sourceType))
//            return;

//        OperiateClipSmooth(sourceType, targetValue, smoothTime, () =>
//        {
//            musicDic[sourceType].Stop();
//            if (afterAction != null)
//                afterAction.Invoke();
//        });
//    }
//    private void PlayClipSmooth(AudioType sourceType, AudioClip clip, float targetValue, bool isLoop, float smoothTime = 0.5f, Action afterAction = null)
//    {
//        if (!musicDic.ContainsKey(sourceType))
//            return;
//        PlayClip(sourceType, clip, 0, isLoop);
//        OperiateClipSmooth(sourceType, targetValue, smoothTime, afterAction);
//    }

//    private void OperiateClipSmooth(AudioType sourceType, float targetValue, float smoothTime = 0.5f, Action afterAction = null)
//    {
//        if (!musicDic.ContainsKey(sourceType))
//        {
//            if (afterAction != null)
//                afterAction.Invoke();
//            return;
//        }
//        float valueArea = targetValue - musicDic[sourceType].volume;
//        MonoMgr.GetInstance().CountdownTimer(smoothTime, null, (countdownTimeInfo) =>
//        {
//            musicDic[sourceType].volume += (valueArea / smoothTime) * Time.deltaTime;
//        }, afterAction);
//    }
//}
