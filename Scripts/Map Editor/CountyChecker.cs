using Godot;

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
                        Node2D county = (Node2D)countyScene.Instantiate();
                        MapEditorGlobals.Instance.countiesParent.AddChild(county);
                    }
                    // Add node to array in MapEditorGlobals.
                    for (int i = 0; i < MapEditorGlobals.Instance.countyDatas.Length; i++)
                    {
                        MapEditorGlobals.Instance.countyDatas[i].countyNode = (Node2D)MapEditorGlobals.Instance.countiesParent.GetChild(i);
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