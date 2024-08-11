using Godot;
using System;

namespace PlayerSpace
{
    public partial class SelectToken : CharacterBody2D
    {
        public CountyPopulation countyPopulation;
        [Export] public Sprite2D sprite;

        [Export] public Texture2D selectedTexture;
        [Export] public Texture2D unselectedTexture;

        [Export] public Label tokenNameLabel;
        //[Export] public Label stackCountLabel; // This can probably be deleted.

        public SpawnedTokenButton spawnedTokenButton;

        [Export] public TokenMovement tokenMovement;

        [Export] public bool isRetreating;
        [Export] private bool inCombat;
        public bool InCombat
        {
            get { return inCombat; }
            set
            {
                inCombat = value;
                if (value == false)
                {
                    if (countyPopulation.moraleExpendable == 100)
                    {
                        return;
                    }
                    else
                    {
                        Clock.Instance.HourChanged += IncreaseMorale;
                    }
                }
                else
                {
                    Clock.Instance.HourChanged -= IncreaseMorale;
                }
            }
        }

        private void IncreaseMorale()
        {
            if (countyPopulation.moraleExpendable == 100)
            {
                Clock.Instance.HourChanged -= IncreaseMorale;
                return;
            }

            int coolCheck = Globals.Instance.random.Next(1, 101);
            if (countyPopulation.skills[AllEnums.Skills.Cool].skillLevel > coolCheck)
            {
                int moraleIncrease = Globals.Instance.random.Next(Globals.Instance.moraleRecoveryMin, Globals.Instance.moraleRecoveryMax);
                countyPopulation.moraleExpendable = Math.Min(countyPopulation.moraleExpendable + moraleIncrease, 100);
            }

        }

        [Export] private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if (value == true)
                {
                    //GD.Print("Token County Population? " + countyPopulation.firstName);
                    sprite.Texture = selectedTexture;
                    if (Globals.Instance.SelectedCountyPopulation != null && countyPopulation != Globals.Instance.SelectedCountyPopulation)
                    {
                        SelectToken currentSelectToken = Globals.Instance.SelectedCountyPopulation.token;
                        //GD.PrintRich("[rainbow]Current Select Token Value True: " + currentSelectToken.Name);
                        currentSelectToken.IsSelected = false;
                    }
                    Globals.Instance.SelectedCountyPopulation = countyPopulation;
                    //GD.Print("Globals Instance County Population: " + Globals.Instance.SelectedCountyPopulation.firstName);

                }
                else
                {
                    sprite.Texture = unselectedTexture;
                    spawnedTokenButton.tokenIconTextureRect.Texture = sprite.Texture;
                    //GD.Print($"Is Selected: {value} {unselectedTexture} {sprite.Texture}");
                }
                //GD.Print($"{Name} selection is: " + value);
            }
        }

        public override void _Ready()
        {
            tokenMovement.token = this;
        }

        public void UpdateSpriteTexture()
        {
            if (isSelected == true)
            {
                sprite.Texture = selectedTexture;
            }
            else
            {
                sprite.Texture = unselectedTexture;
            }
        }
        private void OnMouseEnter()
        {
            //GD.Print("Mouse is inside the token.");
            PlayerControls.Instance.stopClickThrough = true;
            spawnedTokenButton.TooltipText = $"{countyPopulation.firstName} {countyPopulation.lastName} " +
                $"\n Morale: {countyPopulation.moraleExpendable}";
        }

        private static void OnMouseExit()
        {
            //GD.Print("Mouse is outside the token.");
            PlayerControls.Instance.stopClickThrough = false;
        }

        private void OnClick(Viewport viewport, InputEvent @event, int _shapeIdx)
        {
            if (@event is InputEventMouseButton eventMouseButton && countyPopulation.factionData == Globals.Instance.playerFactionData)
            {
                if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false)
                {
                    GD.Print($"You have clicked on {countyPopulation.firstName} {countyPopulation.lastName}");
                    IsSelected = true;
                }
            }
        }

        public void RemoveFromResearch()
        {
            countyPopulation.currentResearchItemData = null;
            ResearchControl.Instance.assignedResearchers.Remove(countyPopulation);
            //GD.Print("Removed from Research - Assigned Researchers Count: " + ResearchControl.Instance.assignedResearchers.Count);
            CountyInfoControl.Instance.GenerateHeroesPanelList();
        }

        private void OnTreeExit()
        {
            Clock.Instance.HourChanged -= IncreaseMorale;
            Clock.Instance.HourChanged -= IncreaseMorale;
        }
    }
}