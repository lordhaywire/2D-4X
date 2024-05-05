using GlobalSpace;
using Godot;
using System;
using System.Linq;

namespace PlayerSpace
{
    public partial class PlayerControls : StaticBody2D
    {
        public static PlayerControls Instance { get; private set; }

        public bool playerControlsEnabled = true;
        public bool stopClickThrough = false;

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
            if (x > 0 && y > 0 && x < mapWidth - 5 && y < mapHeight - 5 && playerControlsEnabled == true && stopClickThrough == false)
            {
                Color countyColor = mapImage.GetPixel(x, y);

                // Check every countyData to find the color it finds.  If it finds that color then it turns on the 
                // grey overlay.
                foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
                {
                    Sprite2D maskSprite = county.maskSprite;

                    if (county.countyData.color.IsEqualApprox(countyColor))
                    {
                        maskSprite.Show();

                        // Mouse Click
                        if (@event is InputEventMouseButton eventMouseButton)
                        {
                            // Left Click on County
                            if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false)
                            //&& Globals.Instance.isInsideToken == false)
                            {
                                //EventLog.Instance.AddLog($"{county.countyData.countyName} was clicked on.");

                                Globals.Instance.CurrentlySelectedCounty = county;
                                Globals.Instance.selectedCountyId = county.countyData.countyId;
                                Globals.Instance.selectedLeftClickCounty = county;
                                TopBarControl.Instance.UpdateTopBarWithCountyResources();
                                CountyInfoControl.Instance.UpdateEverything();
                                CountyInfoControl.Instance.countyInfoControl.Show(); // This has to be last.
                            }

                            // Right Click on County
                            if (eventMouseButton.ButtonIndex == MouseButton.Right && eventMouseButton.Pressed == false
                                && Globals.Instance.SelectedCountyPopulation != null)
                            {
                                GD.Print("You are moving to a place you right clicked, dude! " + county.countyData.countyName);
                                if (Globals.Instance.SelectedCountyPopulation.destination == -1)
                                {
                                    MoveSelectedToken(county.countyData);
                                }
                                else if (Globals.Instance.SelectedCountyPopulation.destination == county.countyData.countyId)
                                {
                                    MoveSelectedToken(county.countyData);
                                }
                                else
                                {
                                    County moveToSelectCounty
                                        = (County)Globals.Instance.countiesParent.GetChild(Globals.Instance.SelectedCountyPopulation.location);
                                    GD.PrintRich("[rainbow]Are we ever even hitting this?" + moveToSelectCounty.countyData.countyId);
                                    MoveSelectedToken(moveToSelectCounty.countyData);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Globals.Instance.selectedCountyId != county.countyData.countyId)
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

        private static void MoveSelectedToken(CountyData moveTargetCountyData)
        {
            Globals.Instance.selectedRightClickCounty = moveTargetCountyData.countyNode;
            GD.Print($"Move Target County: {moveTargetCountyData.countyName}" +
                $" {moveTargetCountyData.countyId}");
            CountyPopulation countyPopulation = Globals.Instance.SelectedCountyPopulation;
            SelectToken selectToken = Globals.Instance.SelectedCountyPopulation.token;

            selectToken.Show();

            if (selectToken.tokenMovement.MoveToken != true)
            {
                if (countyPopulation.IsArmyLeader == false)
                {
                    selectToken.tokenMovement.StartMove(moveTargetCountyData.countyId);
                }
                else
                {
                    if (Globals.Instance.playerFactionData == moveTargetCountyData.factionData)
                    {
                        selectToken.tokenMovement.StartMove(moveTargetCountyData.countyId);
                    }
                    else
                    {
                        GD.Print("You are about to declare war, because you are an army.");
                        if (Globals.Instance.playerFactionData.factionWarDictionary[Globals.Instance.selectedRightClickCounty.countyData.factionData.factionName]
                            != true)
                        {
                            Globals.Instance.playerFactionData.diplomacy.DeclareWarConfirmation(moveTargetCountyData);
                        }
                        else
                        {
                            selectToken.tokenMovement.StartMove(moveTargetCountyData.countyId);
                        }

                    }
                }
            }
            else
            {
                if(selectToken.isRetreating == false)
                {
                    selectToken.tokenMovement.StartMove(moveTargetCountyData.countyId);
                }
                else
                {
                    EventLog.Instance.AddLog($"{selectToken.countyPopulation.firstName} " +
                        $"{selectToken.countyPopulation.lastName} is retreating!");
                }
            }

        }

        public void SelectedChanged(County county, bool selected)
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
