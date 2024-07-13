using Godot;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace PlayerSpace
{
    public partial class County : Node2D
    {
        [Export] public CountyData countyData;

        [ExportGroup("Attached Nodes")]
        [Export] public Sprite2D maskSprite;
        [Export] public Sprite2D countySprite;
        [Export] public Sprite2D capitalSprite;
        [Export] public Node2D countyOverlayNode2D;
        [Export] public Node2D heroSpawn;
        [Export] public BattleControl battleControl;

        [Export] public HBoxContainer armiesHBox;
        [Export] public HBoxContainer heroesHBox;

        private SelectToken selectToken;

        public List<County> neighborCounties = [];

        public override void _Ready()
        {
            countyData.countyNode = this;

            // This is here so that it doesn't subscribe to the clock when the Map Editor is running.
            if (GetTree().CurrentScene.SceneFilePath == "res://Scenes/Main.tscn")
            {
                Clock.Instance.SetDay += EndOfDay;
                Clock.Instance.SetDay += DayStart;
            }
        }

        private void EndOfDay()
        {
            CountyAI countyAI = new();

            GD.Print("County Hour One.");
            // Subtract county resources yesterday from today.
            countyData.SubtractCountyResources();

            // Copy the county resources to yesterday.
            countyData.CopyCountyResourcesToYesterday(); // We will use this data to update the numbers on the top bar all day.

            // Do all the shit for the end of day.
            countyAI.DecideBuildingCountyImprovements(this);
            PopulationAI.WorkDayOverForPopulation(this);
            PopulationAI.IsThereEnoughFood(countyData); // This is a terrible name for this method.

            // This is a check for Occational needs.
            // Population uses other resources besides food.
            countyData.OccationalNeeds();

            countyData.CheckIfCountyImprovementsAreDone();

            // Update the county resources
            TopBarControl.UpdateCountyResources();

            // Update the county labels.  We might be able to take out the UpdateResourceLabels in the faction
            // level so this just runs after.  It is all happening pretty much at the same time.
            if (Globals.Instance.SelectedLeftClickCounty != null)
            {
                TopBarControl.Instance.UpdateResourceLabels();
            }
        }

        private void DayStart()
        {
            countyData.possibleWorkers.Clear(); // Clear the list at the start of each county.
            countyData.workersToRemoveFromPossibleWorkers.Clear();

            countyData.CheckForIdle();
            countyData.CheckForPreferredWork();
            countyData.CheckForAnyWork();
            countyData.CheckForConstruction();
            // Sets people to scavenge.
            countyData.CheckForScavengingFood();
            countyData.CheckForScavengingRemnants();
            countyData.CountIdleWorkers();
        }
        
        private void OnTreeExit()
        {
            Clock.Instance.SetDay -= EndOfDay;
            Clock.Instance.SetDay -= DayStart;
        }
    }
}

