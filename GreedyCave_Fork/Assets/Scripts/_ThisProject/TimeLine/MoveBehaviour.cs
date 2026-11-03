using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class MoveBehaviour : PlayableBehaviour
{
    public Vector3 targetPos;
    public float speed = 5;

    PlayableDirector playableDirector;
    private double duration;
    private Vector3 startPos;
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        duration = playable.GetDuration();
    }

    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        GameObject obj = playerData as GameObject;
        
        
        obj.transform.position = Vector3.MoveTowards(obj.transform.position,targetPos,Time.deltaTime);
    }

}
