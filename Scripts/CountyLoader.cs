using Godot;
using GlobalSpace;

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
                    for (int i = 0; i < files.Length; i++)
                    {
                        PackedScene countyScene = (PackedScene)GD.Load(Globals.Instance.pathToCounties + files[i]);
                        SelectCounty county = (SelectCounty)countyScene.Instantiate();
                        CountyResourcesAutoLoad.Instance.countyDatas[i].countyNode = county;
                        Globals.Instance.countiesParent.AddChild(county);

                        // Since we are already going through all the countyDatas we add the countyData ID to the countyData.
                        CountyResourcesAutoLoad.Instance.countyDatas[i].countyId = i;
                        GD.Print($"County ID: {CountyResourcesAutoLoad.Instance.countyDatas[i].countyId} {county.Name}");
                    }
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