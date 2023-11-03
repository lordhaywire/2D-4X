using Godot;

namespace PlayerSpace
{
    public partial class SelectCounty : Node2D
    {
        [Export] public CountyData countyData;

        [ExportGroup("Attached Nodes")]
        [Export] public Node2D heroSpawn;
        public void OnClick(Viewport _viewport, InputEvent @event, int _shapeIdx)
        {
            if (@event is InputEventMouseButton eventMouseButton)
            {
                if(eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false) 
                {
                    Globals.Instance.countyNameLabel.Text = countyData.countyName;
                    Globals.Instance.selectedCountyData = countyData;
                    Globals.Instance.selectedCounty = this;
                    CountyInfoControl.Instance.UpdateCountyPopulationLabel(countyData.population);
                    CountyInfoControl.Instance.UpdateIdleWorkersLabel(countyData.idleWorkers);
                    CountyInfoControl.Instance.GenerateHeroesPanelList();
                    Globals.Instance.countyInfoControl.Show(); // This has to be last.
                }
            }
        }
    }
}

