using Godot;
using System;

namespace PlayerSpace;

public partial class HeroToken : CharacterBody2D
{
    [Export] public PopulationData populationData;
    [Export] public Sprite2D sprite;

    [Export] public Texture2D selectedTexture;
    [Export] public Texture2D unselectedTexture;

    [Export] public Label tokenNameLabel;

    public SpawnedTokenButton spawnedTokenButton;

    [Export] public TokenMovement tokenMovement;

    [Export] public bool isRetreating;
    [Export] private bool inCombat;

    [Export]
    public bool InCombat
    {
        get { return inCombat; }
        set
        {
            inCombat = value;
            if (value == false)
            {
                if (populationData.moraleExpendable == 100)
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

    [Export] private bool isSelected;
    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            if (value == true)
            {
                //GD.Print("Token County Population? " + populationData.firstName);
                sprite.Texture = selectedTexture;
                if (Globals.Instance.SelectedCountyPopulation != null && populationData != Globals.Instance.SelectedCountyPopulation)
                {
                    HeroToken currentSelectToken = Globals.Instance.SelectedCountyPopulation.heroToken;
                    //GD.PrintRich("[rainbow]Current Select Token Value True: " + currentSelectToken.Name);
                    currentSelectToken.IsSelected = false;
                }
                Globals.Instance.SelectedCountyPopulation = populationData;
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

    private void IncreaseMorale()
    {
        if (populationData.moraleExpendable == 100)
        {
            Clock.Instance.HourChanged -= IncreaseMorale;
            return;
        }

        int coolCheck = Globals.Instance.random.Next(1, 101);
        if (populationData.skills[AllEnums.Skills.Cool].skillLevel > coolCheck)
        {
            int moraleIncrease = Globals.Instance.random.Next(Globals.Instance.moraleRecoveryMin, Globals.Instance.moraleRecoveryMax);
            populationData.moraleExpendable = Math.Min(populationData.moraleExpendable + moraleIncrease, 100);
        }

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
        spawnedTokenButton.TooltipText = $"{populationData.firstName} {populationData.lastName} " +
            $"\n Morale: {populationData.moraleExpendable}";
    }

    private static void OnMouseExit()
    {
        //GD.Print("Mouse is outside the token.");
        PlayerControls.Instance.stopClickThrough = false;
    }

    private void OnClick(Viewport viewport, InputEvent @event, int _shapeIdx)
    {
        if (@event is InputEventMouseButton eventMouseButton && populationData.factionData == Globals.Instance.playerFactionData)
        {
            if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false)
            {
                //GD.Print($"You have clicked on {populationData.firstName} {populationData.lastName}");
                IsSelected = true;
            }
        }
    }

    public void RemoveFromResearch()
    {
        populationData.currentResearchItemData = null;
        ResearchControl.Instance.assignedResearchers.Remove(populationData);
        //GD.Print("Removed from Research - Assigned Researchers Count: " + ResearchControl.Instance.assignedResearchers.Count);
        CountyInfoControl.Instance.GenerateHeroesPanelList();
    }

    private void OnTreeExit()
    {
        Clock.Instance.HourChanged -= IncreaseMorale;
        Clock.Instance.HourChanged -= IncreaseMorale;
    }
}