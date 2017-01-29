using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Manager;
using Assets.Scripts.Player;
using Assets.Scripts.SettingClasses;
using UnityStandardAssets.Characters.ThirdPerson;
using GameManager = FlipWebApps.GameFramework.Scripts.GameStructure.GameManager;

public enum PowerupType
{
    Healtpack, Manapack, Speedup
}

public class Powerup : MonoBehaviour
{
    public Vector3 RotateAround;
    public float Angle;
    public PowerupType Type;
    public FlowManager FlowManager;
    private FlowSetting _flow;

    // Use this for initialization
    void Start()
    {
        FlowManager = EnvironmentManager.Instance.FlowManager;
        _flow = FlowManager.Flow;
        var pos = transform.position;
        pos.y = 5;
        RotateAround = pos;
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(RotateAround, Vector3.up, Angle * Time.deltaTime);
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player") return;

        AntEater Player = collision.gameObject.GetComponent<AntEater>();
        Player.IncreaseScore();
        
        switch (Type)
        {
            case PowerupType.Healtpack:
                float hp = _flow.HealthIncrease * (1 - FlowManager.FlowLevel);
                Player.IncreaseHealth(hp);
                break;
            case PowerupType.Manapack:
                float mp = _flow.ManaConsumption * (1 - FlowManager.FlowLevel);
                Player.IncreaseMana(mp);
                break;
            case PowerupType.Speedup:
                float sp = _flow.ManaConsumption * (1 - FlowManager.FlowLevel) + _flow.HealthIncrease * (1 - FlowManager.FlowLevel);
                Player.IncreaseSpeedBooster(sp);
                break;
        }

        Destroy(gameObject);
    }
}
