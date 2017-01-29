using FlipWebApps.GameFramework.Scripts.GameStructure;
using FlipWebApps.GameFramework.Scripts.GameStructure.Levels.ObjectModel;
using FlipWebApps.GameFramework.Scripts.Helper;
using FlipWebApps.GameFramework.Scripts.Messaging;
using FlipWebApps.GameFramework.Scripts.UI.Dialogs.Components;

public class CustomLevel : Level
{
    public int Time;
    public string Difficulty;
    public int Setting;

    public CustomLevel(int levelNumber)
    {
        Initialise(levelNumber, loadFromResources: true);
    }

    public override void ParseData(JSONObject jsonObject)
    {
        base.ParseData(jsonObject);
        Difficulty = jsonObject.GetString("difficulty");
        Time = (int)jsonObject.GetNumber("time");
        Setting = (int) jsonObject.GetNumber("setting");
        //AntType type;
        //switch (Setting)
        //{
        //    case 1: type = AntType.AltruisticAnt; break;
        //    case 2: type = AntType.EgoistAnt; break;
        //    case 3: type = AntType.FreeriderAnt; break;
        //    case 4: type = AntType.SloppyAnt; break;
        //    case 5: type = AntType.AdaptiveAnt; break;
        //    case 6: type = AntType.CunningAnt; break;
        //    default: type = AntType.AltruisticAnt; break;
        //}
        
    }

}