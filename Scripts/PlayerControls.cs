using GlobalSpace;
using Godot;
using MapEditorSpace;

namespace PlayerSpace
{
    public partial class PlayerControls : StaticBody2D
    {
        public static PlayerControls Instance { get; private set; }

        public bool playerControlsEnabled = true;

        private Image mapImage;
        [Export] private RectangleShape2D collisionRectangleShape;
        private Color backgroundColor = new(220, 220, 220);

        private int mapWidth;
        private int mapHeight;

        public override void _Ready()
        {
            Instance = this;

            mapImage = Globals.Instance.mapColorCoded.GetImage();

            mapWidth = mapImage.GetWidth();
            mapHeight = mapImage.GetHeight();
        }

        /*
        public void OnClick(Viewport viewport, InputEvent @event, int _shapeIdx)
        {
            if (@event is InputEventMouseButton eventMouseButton)
            {
                // Left Click on County
                if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false
                    && Globals.Instance.isInsideToken == false)
                {
                   /*
                    EventLog.Instance.AddLog($"{Name} was clicked on.");
                    // When you select a county with left click it unselects the selected hero.

                    Globals.Instance.CurrentlySelectedToken = null;
                    GD.Print($"You left clicked on {Name}, dude!");
                    Globals.Instance.selectedCountyData = countyData;
                    Globals.Instance.selectedLeftClickCounty = this;

                    CountyInfoControl.Instance.UpdateEverything();
                    CountyInfoControl.Instance.countyInfoControl.Show(); // This has to be last.

                }
                // Right Click on County
                if (eventMouseButton.ButtonIndex == MouseButton.Right && eventMouseButton.Pressed == false)
                {
                    GD.Print("You right clicked, dude!");
                    Globals.Instance.selectedRightClickCounty = this;
                    if (Globals.Instance.CurrentlySelectedToken != null)
                    {
                        selectToken = Globals.Instance.CurrentlySelectedToken;
                        countyPopulation = selectToken.countyPopulation;

                        if (selectToken.tokenMovement.MoveToken != true)
                        {
                            if (selectToken.countyPopulation.isArmyLeader == false)
                            {
                                StartMove();
                            }
                            else
                            {
                                if (Globals.Instance.playerFactionData == countyData.factionData)
                                {
                                    StartMove();
                                }
                                else
                                {
                                    GD.Print("You are about to declare war, because you are an army.");
                                    DeclareWarConfirmation();
                                }
                            }
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
        */
        public override void _Input(InputEvent @event)
        {
            int x = (int)GetGlobalMousePosition().X;
            int y = (int)GetGlobalMousePosition().Y;

            collisionRectangleShape.Size = new Vector2(mapWidth, mapHeight);

            // First check to make sure it is inside the map (a tiny bit more then the size of the map.)
            if (x > 0 && y > 0 && x < mapWidth - 5 && y < mapHeight - 5 && playerControlsEnabled == true)
            {
                Color countyColor = mapImage.GetPixel(x, y);

                //GD.Print("Color: " + countyColor);
                // Check every countyData to find the color it finds.  If it finds that color then it turns on the 
                // grey overlay.
                foreach (CountyData countyData in CountyResourcesAutoLoad.Instance.countyDatas)
                {
                    //GD.Print("County Color: " + countyData.color);
                    SelectCounty county = (SelectCounty)countyData.countyNode;
                    //GD.Print("Player Controls Select County: " + county.Name);
                    Sprite2D maskSprite = county.maskSprite;

                    if (countyData.color.IsEqualApprox(countyColor))
                    {
                        //GD.Print("County clicked on is: " + countyData.countyName);
                        maskSprite.Show();
                        //Globals.Instance.selectedCountyId = countyData.countyId;

                        // Left Click on County
                        if (@event is InputEventMouseButton eventMouseButton)
                        {
                            if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false
                                && Globals.Instance.isInsideToken == false)
                            {
                                EventLog.Instance.AddLog($"{countyData.countyName} was clicked on.");
                                // When you select a county with left click it unselects the selected hero.

                                Globals.Instance.CurrentlySelectedToken = null;
                                GD.Print($"You left clicked on {countyData.countyNode.Name}, dude!");
                                Globals.Instance.selectedCountyData = countyData;
                                Globals.Instance.selectedLeftClickCounty = (SelectCounty)countyData.countyNode;

                                CountyInfoControl.Instance.UpdateEverything();
                                CountyInfoControl.Instance.countyInfoControl.Show(); // This has to be last.
                            }

                            // Right Click on County
                            if (eventMouseButton.ButtonIndex == MouseButton.Right && eventMouseButton.Pressed == false)
                            {
                                GD.Print("You right clicked, dude!");
                                Globals.Instance.selectedRightClickCounty = (SelectCounty)countyData.countyNode;

                                if (Globals.Instance.CurrentlySelectedToken != null)
                                {
                                    SelectToken selectToken = Globals.Instance.CurrentlySelectedToken;
                                    CountyPopulation countyPopulation = selectToken.countyPopulation;

                                    if (selectToken.tokenMovement.MoveToken != true)
                                    {
                                        if (selectToken.countyPopulation.isArmyLeader == false)
                                        {
                                            Globals.Instance.selectedRightClickCounty.StartMove();
                                        }
                                        else
                                        {
                                            if (Globals.Instance.playerFactionData == countyData.factionData)
                                            {
                                                Globals.Instance.selectedRightClickCounty.StartMove();

                                            }
                                            else
                                            {
                                                GD.Print("You are about to declare war, because you are an army.");
                                            Globals.Instance.selectedRightClickCounty.DeclareWarConfirmation();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        SelectCounty homeCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(Globals.Instance.CurrentlySelectedToken.countyPopulation.location);
                                        countyPopulation.destination = homeCounty.countyData.countyId;
                                        Globals.Instance.heroMoveTarget = homeCounty.heroSpawn.GlobalPosition;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        maskSprite.Hide();
                    }
                }

                if (@event is InputEventKey keyEvent && keyEvent.Pressed == false)
                {
                    if (playerControlsEnabled == true)
                    {
                        GD.Print($"{keyEvent.Keycode}");
                        switch (keyEvent.Keycode)
                        {
                            case Key.Space:
                                Clock.Instance.PauseandUnpause();
                                break;
                        }
                    }
                }
            }
        }
        public void AdjustPlayerControls(bool controls)
        {
            playerControlsEnabled = controls;
        }
    }
}
