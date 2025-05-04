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

    // Change this to an enum.
    [Export] public bool isRetreating;
    [Export] private bool inCombat;

    [Export]
    public bool InCombat
    {
        get => inCombat;
        set
        {
            inCombat = value;
            if (value == false)
            {
                if (populationData.moraleExpendable < 100)
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
        get => isSelected;
        set
        {
            isSelected = value;
            if (value)
            {
                //GD.Print("Token County Population? " + populationData.firstName);
                sprite.Texture = selectedTexture;
                if (Globals.Instance.SelectedCountyPopulation != null &&
                    populationData != Globals.Instance.SelectedCountyPopulation)
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
        tokenMovement.heroToken = this;
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
            int moraleIncrease =
                Globals.Instance.random.Next(Globals.Instance.moraleRecoveryMin, Globals.Instance.moraleRecoveryMax);
            populationData.moraleExpendable = Math.Min(populationData.moraleExpendable + moraleIncrease, 100);
        }
    }

    public void UpdateSpriteTexture()
    {
        sprite.Texture = isSelected ? selectedTexture : unselectedTexture;
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
        if (@event is InputEventMouseButton eventMouseButton &&
            populationData.factionData == Globals.Instance.playerFactionData)
        {
            if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed == false)
            {
                //GD.Print($"You have clicked on {populationData.firstName} {populationData.lastName}");
                IsSelected = true;
            }
        }
    }

    public void RemoveHeroAndSubordinatesFromStartingCounty()
    {
        //GD.Print($"Removed token from Starting County {token.populationData.firstName} {token.populationData.location}");
        //GD.Print($"Removed {token.populationData.firstName} {token.populationData.factionData.factionName}");
        // Remove populationData from the heroes starting county location list.
        County startingCounty = (County)Globals.Instance.countiesParent.GetChild(populationData.location);
        FactionData locationFactionData = FactionData.GetFactionDataFromId(populationData.location);

        // We don't need to check which list the hero is in because C# doesn't give a shit if the hero isn't in the list.
        // So we just try to remove it from both, and it will remove it from the correct one.
        startingCounty.countyData.heroesInCountyList.Remove(populationData);
        startingCounty.countyData.armiesInCountyList.Remove(populationData);
        startingCounty.countyData.visitingHeroList.Remove(populationData);
        startingCounty.countyData.visitingArmyList.Remove(populationData);

        startingCounty.countyData.spawnedTokenButtons.Remove(spawnedTokenButton);

        // Remove Subordinates.
        if (populationData.heroSubordinates.Count <= 0 ||
            !Globals.Instance.CheckIfPlayerFaction(locationFactionData)) return;
        CountyData countyData = startingCounty.countyData;
        foreach (PopulationData person in populationData.heroSubordinates)
        {
            countyData.populationDataList.Remove(person);
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

    public void AddHeroAndSubordinatesToDestinationCounty(County destinationCounty)
    {
        //GD.Print("Add To Destination County " + token.populationData.firstName);
        FactionData locationFactionData = FactionData.GetFactionDataFromId(populationData.factionData.factionId);
        populationData.location = destinationCounty.countyData.countyId;
        
        destinationCounty.countyData.spawnedTokenButtons.Add(spawnedTokenButton);

        if (populationData.heroSubordinates.Count <= 0 ||
            !Globals.Instance.CheckIfPlayerFaction(locationFactionData)) return;
        CountyData countyData = destinationCounty.countyData;
        foreach (PopulationData person in populationData.heroSubordinates)
        {
            countyData.populationDataList.Add(person);
            person.location = destinationCounty.countyData.countyId;
            person.destination = -1;
        }
    }
}