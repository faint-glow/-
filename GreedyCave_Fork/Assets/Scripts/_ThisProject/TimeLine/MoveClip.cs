using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MoveClip : PlayableAsset
{
    public Vector3 targetMove;

    public MoveBehaviour moveBehaviour;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        //ScriptPlayable<MoveBehaviour> playable = ScriptPlayable<MoveBehaviour>.Create(graph);
        ScriptPlayable<MoveBehaviour> playable = ScriptPlayable<MoveBehaviour>.Create(graph, moveBehaviour);
        //moveBehaviour = playable.GetBehaviour();
        //moveBehaviour.targetPos = targetMove;
        //playable.SetDuration(moveBehaviour.speed);
        return playable;
    }
}
