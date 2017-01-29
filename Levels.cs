using FlipWebApps.GameFramework.Scripts.GameStructure.GameItems;
using FlipWebApps.GameFramework.Scripts.GameStructure.GameItems.ObjectModel;
using FlipWebApps.GameFramework.Scripts.GameStructure.Levels.ObjectModel;

public class Levels : GameItemsManager<Level, GameItem>
{
    protected override void LoadItems()
    {
        Items = new Level[]
        {
            new CustomLevel(1),
            new CustomLevel(2),
            new CustomLevel(3),
            new CustomLevel(4),
            new CustomLevel(5),
            new CustomLevel(6),
        };
    }
}