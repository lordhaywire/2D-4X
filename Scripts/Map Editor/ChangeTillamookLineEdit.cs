using Godot;
using MapEditorSpace;
using PlayerSpace;

public partial class ChangeTillamookLineEdit : LineEdit
{
    [Export] private CountyData tillamookData;
    private void TextEntered(string text)
    {
        County county = (County)MapEditorGlobals.Instance.countiesParent.GetChild(1);
        county.countyData.countyName = text;
        GD.Print(county.countyData.countyName);
        ResourceSaver.Save(county.countyData, "res://Resources/Counties/1 Tillamook.tres");
    }

}
