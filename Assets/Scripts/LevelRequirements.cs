using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Level Reqs", order = 1)]
public class LevelRequirements : ScriptableObject
{
    public int CameraSize;
    public int TilesRemaining;
    
    public string[] StartLevelMessages;
    public int StartLevelWait = 3;

    public string[] EndLevelMessages;
    public int EndLevelWait = 3;

    public string FailMessage_Dead = "Ouch!";

    public string[] FailMessages = { "Too soon!" };
    public int FailWait = 3;

}
