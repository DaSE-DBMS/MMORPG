using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Gamekit3D;

[Serializable]
public class CutsceneScriptControlBehaviour : PlayableBehaviour
{
    public bool playerInputEnabled;
    public bool useRootMotion;
    public PlayerMyController playerInput;

    public override void OnGraphStart(Playable playable)
    {

    }
}
