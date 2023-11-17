using Godot;

namespace PlayerSpace
{
    public partial class SelectCounty : Node2D
    {
        [Export] public CountyData countyData;

        [ExportGroup("Attached Nodes")]
        [Export] public Sprite2D capitalSprite;
        [Export] public HeroStacker heroSpawn;
        public override void _Ready()
        {
            capitalSprite = GetNode<Sprite2D>("County Overlay Node2D/Capital Sprite2D");
            heroSpawn = GetNode<HeroStacker>("County Overlay Node2D/Hero Spawn Location Node2D");
        }
        public void OnClick(Viewport _viewport, InputEvent @event, int _shapeIdx)
        {
            if (@event is InputEventMouseButton eventMouseButton)
            {
                // Left Click on County
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
                // Right Click on County
                if (eventMouseButton.ButtonIndex == MouseButton.Right && eventMouseButton.Pressed == false)
                {
                    GD.Print("You right clicked, dude!");
                    if (Globals.Instance.selectedToken != null)
                    {
                        SelectToken selectToken = (SelectToken)Globals.Instance.selectedToken;
                        CountyPopulation countyPopulation = selectToken.countyPopulation;
                        SelectCounty selectLocationCounty 
                            = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
                        Globals.Instance.heroMoveTarget = heroSpawn.GlobalPosition;

                        countyPopulation.destination = countyData.countyID;
                        selectToken.tokenMovement.MoveToken = true;

                        // Remove countyPopulation from the heroes starting county location list.
                        selectLocationCounty.countyData.heroCountyPopulation.Remove(countyPopulation);

                        // Removed from spawnedTokenList in starting county location.
                        selectLocationCounty.heroSpawn.spawnedTokenList.Remove(selectToken);
                    }

                }
            }
        }
    }
}

