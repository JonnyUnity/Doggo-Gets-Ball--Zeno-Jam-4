using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Level Reqs", order = 1)]
public class LevelRequirements : ScriptableObject
{
    public int CameraSize;
    public int TilesRemaining;
    
    public string[] StartLevelMessages = { "There's always another ball! Woof!" };
    public int StartLevelWait = 3;

    public string[] EndLevelMessages = { "Woof! I did it!" };
    public int EndLevelWait = 3;

    public string FailMessage_TooSoon = "Too soon!";
    public string FailMessage_Dead = "Ouch!";

    public string[] FailMessages = { "Too soon!" };
    public int FailWait = 3;

}
