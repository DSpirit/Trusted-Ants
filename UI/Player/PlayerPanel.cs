using System;
using UnityEngine;
using System.Collections;
using System.Globalization;
using Assets.Scripts.Manager;
using Assets.Scripts.Player;
using UnityEngine.UI;
using GameManager = FlipWebApps.GameFramework.Scripts.GameStructure.GameManager;

public class PlayerPanel : MonoBehaviour
{
    public AntEater Player;

    public Image Health;
    public Text HealthText;
    public Image Mana;
    public Text ManaText;
    public Text GameTime;
    public Text Score;
    public Image Boost;
    public Text BoostText;
    public Image Flow;
    public Text FlowText;

	// Use this for initialization
	void Start ()
	{
	 
	}
	
	// Update is called once per frame
	void Update ()
	{
        var ci = new CultureInfo("de-DE");
	    ci.NumberFormat.PercentDecimalDigits = 0;
	    Health.fillAmount = Player.Health;
	    HealthText.text = Player.Health.ToString("P", ci);
	    Mana.fillAmount = Player.Mana;
	    ManaText.text = Player.Mana.ToString("P", ci);
	    Boost.fillAmount = Player.SpeedBooster;
	    BoostText.text = Player.SpeedBooster.ToString("P", ci);
      
	}

    public void OnScoreChanged()
    {
    }
}
