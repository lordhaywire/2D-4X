using GlobalSpace;
using Godot;

namespace PlayerSpace
{
    public partial class PlayerControls : StaticBody2D
    {
        public static PlayerControls Instance { get; private set; }

        public bool playerControlsEnabled = true;
        public bool mouseOverUI = false;

        private Image mapImage;
        [Export] private RectangleShape2D collisionRectangleShape;
        private Color backgroundColor = new(220, 220, 220);
        private Color outlineColor = new(0, 0, 0, 0.7f);
        private Color fillColor = new(1, 1, 1, 0.35f);

        private int mapWidth;
        private int mapHeight;

        public override void _Ready()
        {
            Instance = this;

            mapImage = Globals.Instance.mapColorCoded.GetImage();

            mapWidth = mapImage.GetWidth();
            mapHeight = mapImage.GetHeight();
        }

        public override void _Input(InputEvent @event)
        {
            int x = (int)GetGlobalMousePosition().X;
            int y = (int)GetGlobalMousePosition().Y;

            collisionRectangleShape.Size = new Vector2(mapWidth, mapHeight);

            // First check to make sure it is inside the map (a tiny bit more then the size of the map.)
            if (x > 0 && y > 0 && x < mapWidth - 5 && y < mapHeight - 5 && playerControlsEnabled == true && mouseOverUI == false)
            {
                Color countyColor = mapImage.GetPixel(x, y);

                // Check every countyData to find the color it finds.  If it finds that color then it turns on the 
                // grey overlay.
                foreach (CountyData countyData in CountyResourcesAutoLoad.Instance.countyDatas)
                {
                    SelectCounty county = (SelectCounty)countyData.countyNode;
                    Sprite2D maskSprite = county.maskSprite;

                    if (countyData.color.IsEqualApprox(countyColor))
                    {
                        maskSprite.Show();

                        // Left Click on County
                        if (@event is InputEventMouseButton eventMouseButton)
                        {
                            if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false)
                                //&& Globals.Instance.isInsideToken == false)
                            {
                                EventLog.Instance.AddLog($"{countyData.countyName} was clicked on.");

                                // When you select a county with left click it unselects the selected hero.
                                //Globals.Instance.selectedCountyPopulation = null;
                                //GD.PrintRich("[rainbow]I bet the mother fucking problem is this.");
                                Globals.Instance.SelectedCountyData = countyData;
                                Globals.Instance.selectedCountyId = countyData.countyId;
                                Globals.Instance.selectedLeftClickCounty = (SelectCounty)countyData.countyNode;
                                CountyInfoControl.Instance.UpdateEverything();
                                CountyInfoControl.Instance.countyInfoControl.Show(); // This has to be last.
                            }

                            // Right Click on County
                            if (eventMouseButton.ButtonIndex == MouseButton.Right && eventMouseButton.Pressed == false  
                                && Globals.Instance.selectedCountyPopulation != null)
                            {
                                GD.Print("You right clicked, dude!");
                                Globals.Instance.selectedRightClickCounty = (SelectCounty)countyData.countyNode;
                                GD.Print("Selected Right Click County: " + Globals.Instance.selectedRightClickCounty.Name);
                                SelectToken selectToken = (SelectToken)Globals.Instance.selectedCountyPopulation.token;
                                CountyPopulation countyPopulation = Globals.Instance.selectedCountyPopulation;
                                selectToken.Show();
                                SelectCounty startCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
                                selectToken.GlobalPosition = startCounty.heroSpawn.GlobalPosition;
                                if (selectToken != null && countyPopulation.location != countyData.countyId)
                                {
                                    if (selectToken.tokenMovement.MoveToken != true)
                                    {
                                        if (countyPopulation.isArmyLeader == false)
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
                                        countyPopulation.destination = startCounty.countyData.countyId;
                                        Globals.Instance.heroMoveTarget = startCounty.heroSpawn.GlobalPosition; // Why are we storing this in Globals?
                                        selectToken.tokenMovement.MoveToken = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if(Globals.Instance.selectedCountyId != countyData.countyId)
                        {
                            maskSprite.Hide();
                        }
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

        public void SelectedChanged(SelectCounty county, bool selected)
        {
            if (selected)
            {
                ((ShaderMaterial)county.maskSprite.Material).SetShaderParameter("outline_color", outlineColor);
                ((ShaderMaterial)county.maskSprite.Material).SetShaderParameter("fill_color", fillColor);
            }
            else
            {
                ((ShaderMaterial)county.maskSprite.Material).SetShaderParameter("outline_color", outlineColor.A * 0.5f);
                ((ShaderMaterial)county.maskSprite.Material).SetShaderParameter("fill_color", fillColor.A * 0.5f);
            }
        }
        public void AdjustPlayerControls(bool controls)
        {
            playerControlsEnabled = controls;
        }
    }
}
