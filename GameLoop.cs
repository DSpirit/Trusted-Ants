using FlipWebApps.GameFramework.Scripts.GameObjects.Components;
using FlipWebApps.GameFramework.Scripts.GameStructure;
using FlipWebApps.GameFramework.Scripts.GameStructure.Levels;
using FlipWebApps.GameFramework.Scripts.UI.Dialogs.Components;
using FlipWebApps.GameFramework.Scripts.UI.Other.Components;
using UnityEngine;
 
public class GameLoop : Singleton<GameLoop>
{
    public TimeRemaining Countdown;
    CustomLevel _level;

    void Start()
    {
        _level = (CustomLevel)GameManager.Instance.Levels.Selected;
        Countdown.Limit = _level.Time;
    }

    void Update()
    {
        //bool isWon = false;//Input.GetMouseButtonDown(0);
        bool isWon = LevelManager.Instance.SecondsRunning > _level.Time;
        if (LevelManager.Instance.IsLevelRunning && (isWon))
        {
            LevelManager.Instance.GameOver(isWon);
        }
    }
}