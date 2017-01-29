using UnityEngine;
using FlipWebApps.GameFramework.Scripts.GameStructure;

public class SetLevels : MonoBehaviour
{
    void Awake()
    {
        GameManager.Instance.Levels = new Levels();
        GameManager.Instance.Levels.Load();
    }
}