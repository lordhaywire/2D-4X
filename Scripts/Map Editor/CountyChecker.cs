using Godot;
using GlobalSpace;

namespace MapEditorSpace
{
    public partial class CountyChecker : Node
    {
        public override void _Ready()
        {
            CheckMapAlreadyGenerated();
        }

        private void CheckMapAlreadyGenerated()
        {
            DirAccess directory = DirAccess.Open(MapEditorGlobals.Instance.pathToCounties);
            if (directory != null)
            {
                directory.ListDirBegin();
                string[] files = directory.GetFiles();
                if (files.Length > 0)
                {
                    MapEditorControls.Instance.controlsEnabled = true;
                    // Load from disk.
                    foreach (string name in files)
                    {
                        PackedScene countyScene = (PackedScene)GD.Load(MapEditorGlobals.Instance.pathToCounties + name);
                        PlayerSpace.County county = (PlayerSpace.County)countyScene.Instantiate();
                        MapEditorGlobals.Instance.countiesParent.AddChild(county);
                    }
                    // Add node to array in MapEditorGlobals.
                    for (int i = 0; i < CountyResourcesAutoLoad.Instance.countyDatas.Count; i++)
                    {
                        CountyResourcesAutoLoad.Instance.countyDatas[i].countyNode 
                            = (PlayerSpace.County)MapEditorGlobals.Instance.countiesParent.GetChild(i);
                    }
                }
                else
                {
                    GD.Print($"No files in {MapEditorGlobals.Instance.pathToCounties}");

                }
            }
            else
            {
                GD.Print($"{MapEditorGlobals.Instance.pathToCounties} directory is missing.");
            }
        }
    }
}