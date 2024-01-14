using Godot;

namespace PlayerSpace
{
    public partial class CountyLoader : Node
    {
        public override void _Ready()
        {
            CheckMapAlreadyGenerated();
        }

        private void CheckMapAlreadyGenerated()
        {
            DirAccess directory = DirAccess.Open(Globals.Instance.pathToCounties);
            if (directory != null)
            {
                directory.ListDirBegin();
                string[] files = directory.GetFiles();
                if (files.Length > 0)
                {
                    //PlayerControls.Instance.controlsEnabled = true;
                    // Load from disk.
                    foreach (string name in files)
                    {
                        PackedScene countyScene = (PackedScene)GD.Load(Globals.Instance.pathToCounties + name);
                        Node2D county = (Node2D)countyScene.Instantiate();
                        Globals.Instance.countiesParent.AddChild(county);
                    }
                    /*
                    // Add node to array in MapEditorGlobals.
                    for (int i = 0; i < Globals.Instance.countyDatas.Length; i++)
                    {
                        Globals.Instance.countyDatas[i].countyNode = (Node2D)MapEditorGlobals.Instance.countiesParent.GetChild(i);
                    }
                    */
                }
                else
                {
                    GD.Print($"No files in {Globals.Instance.pathToCounties}");

                }
            }
            else
            {
                GD.Print($"{Globals.Instance.pathToCounties} directory is missing.");
            }
        }
    }
}