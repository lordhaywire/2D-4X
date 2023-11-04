using Godot;

namespace PlayerSpace
{
    public partial class SelectCounty : Node2D
    {
        [Export] public CountyData countyData;

        [ExportGroup("Attached Nodes")]
        [Export] public Sprite2D capitalSprite;
        [Export] public Node2D heroSpawn;

        public override void _Ready()
        {
            capitalSprite = GetNode<Sprite2D>("County Overlay Node2D/Capital Sprite2D");
            heroSpawn = GetNode<Node2D>("County Overlay Node2D/Hero Spawn Node2D");

        }
        public void OnClick(Viewport _viewport, InputEvent @event, int _shapeIdx)
        {
            if (@event is InputEventMouseButton eventMouseButton)
            {
                if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false)
                {
                    // When you select a county with left click it unselects the selected hero.
                    if (Globals.Instance.selectedToken != null)
                    {
                        SelectToken selectToken = (SelectToken)Globals.Instance.selectedToken;
                        selectToken.IsSelected = false;
                    }
                    GD.Print("You left clicked, dude!");
                    Globals.Instance.countyNameLabel.Text = countyData.countyName;
                    Globals.Instance.selectedCountyData = countyData;
                    Globals.Instance.selectedCounty = this;
                    CountyInfoControl.Instance.UpdateCountyPopulationLabel(countyData.population);
                    CountyInfoControl.Instance.UpdateIdleWorkersLabel(countyData.idleWorkers);
                    CountyInfoControl.Instance.GenerateHeroesPanelList();
                    Globals.Instance.countyInfoControl.Show(); // This has to be last.
                }
                if (eventMouseButton.ButtonIndex == MouseButton.Right && eventMouseButton.Pressed == false)
                {
                    GD.Print("You right clicked, dude!");
                    if (Globals.Instance.selectedToken != null)
                    {
                        SelectToken selectToken = (SelectToken)Globals.Instance.selectedToken;
                        Globals.Instance.heroMoveTarget = heroSpawn.GlobalPosition;
                        selectToken.tokenMovement.move = true;
                        selectToken.countyPopulation.destination = countyData.countyID;
                    }

                }
            }
        }
    }
}

