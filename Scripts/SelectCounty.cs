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
        public void OnClick(Viewport viewport, InputEvent @event, int _shapeIdx)
        {
            if (@event is InputEventMouseButton eventMouseButton)
            {
                // Left Click on County
                if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false 
                    && Globals.Instance.isInsideToken == false)
                {
                    EventLog.Instance.AddLog($"{Name} was clicked on.");
                    // When you select a county with left click it unselects the selected hero.

                    Globals.Instance.CurrentlySelectedToken = null;
                    GD.Print($"You left clicked on {Name}, dude!");
                    Globals.Instance.selectedCountyData = countyData;
                    Globals.Instance.selectedSelectCounty = this;
                    CountyInfoControl.Instance.UpdateNameLabels();
                    CountyInfoControl.Instance.UpdateCountyPopulationLabel();
                    CountyInfoControl.Instance.UpdateIdleWorkersLabel();
                    CountyInfoControl.Instance.GenerateHeroesPanelList();
                    CountyInfoControl.Instance.countyInfoControl.Show(); // This has to be last.
                    
                }
                // Right Click on County
                if (eventMouseButton.ButtonIndex == MouseButton.Right && eventMouseButton.Pressed == false)
                {
                    GD.Print("You right clicked, dude!");
                    if (Globals.Instance.CurrentlySelectedToken != null)
                    {
                        SelectToken selectToken = Globals.Instance.CurrentlySelectedToken;
                        CountyPopulation countyPopulation = selectToken.countyPopulation;

                        if (selectToken.tokenMovement.MoveToken != true)
                        {
                            GD.Print($"{selectToken.countyPopulation.firstName} has location of {countyPopulation.location}");
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
                        else
                        {
                            SelectCounty homeCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(Globals.Instance.CurrentlySelectedToken.countyPopulation.location);
                            countyPopulation.destination = homeCounty.countyData.countyID;
                            Globals.Instance.heroMoveTarget = homeCounty.heroSpawn.GlobalPosition;
                        }
                    }

                }
            }
        }
    }
}

