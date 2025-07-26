using Godot;
using PlayerSpace;

namespace GlobalSpace;

public partial class CountyResourcesAutoLoad : Node
{
    // This is for the map editor, I believe.
    public static CountyResourcesAutoLoad Instance { get; private set; }

    // Todo: This needs to use the Resource Loading method in Autoload.  Its already loaded, actually.
    private const string PathToCountyData = "res://Resources/Counties/";
    [Export] public Godot.Collections.Array<CountyData> countyDatas;// = [];

    public override void _Ready()
    {
        Instance = this;
        countyDatas = [];
        GD.Print("CountyResourcesAutoLoad.cs has loaded.");
        LoadCountyResources();
    }

    private void LoadCountyResources()
    {
        DirAccess directory = DirAccess.Open(PathToCountyData);
        if (directory != null)
        {
            directory.ListDirBegin();
            string[] files = directory.GetFiles();
            if (files.Length > 0)
            {
                // Load county resources from disk.
                foreach (string name in files)
                {
                    CountyData countyData = (CountyData)GD.Load(PathToCountyData + name).Duplicate();
                    countyDatas.Add(countyData);
                    //GD.Print("County that was added: " + countyData.countyName);
                }
            }
            else
            {
                //GD.Print($"No files in {pathToCountyDatas}");
            }
        }
        else
        {
            //GD.Print($"{pathToCountyDatas} directory is missing.");
        }
    }
}