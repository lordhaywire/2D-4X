using Godot;
using System.Collections.Generic;

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
            countyData.countyNode = this; // Figure out why I did this.  I bet you this could be removed.

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

            // This is busted until we add people to the County Improvement List.
            CountyAI.CheckIfCountyImprovementsAreDone(countyData);

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
            PopulationAI populationAI = new();
            // This is the one with the dumb name.
            populationAI.CheckForWork(this); // We need to add the population to the new peopleAtCountyImprovementList.
        }
        private void OnTreeExit()
        {
            Clock.Instance.SetDay -= EndOfDay;
            Clock.Instance.SetDay -= DayStart;
        }
    }
}

