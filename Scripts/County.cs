using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

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
            countyData.countyNode = this; // Figure out why I did this.

            Clock.Instance.SetDay += EndOfDay;
            Clock.Instance.SetDay += DayStart;
        }

        private void EndOfDay()
        {
            CountyAI countyAI = new();
            PopulationAI populationAI = new();

            GD.Print("County Hour Zero.");
            countyAI.DecideBuildingCountyImprovements(this);
            // We are probably going to get rid of work, meaning move the code to another script.
            populationAI.WorkDayOverForPopulation(countyData.countyNode);

            // We need something like "finish constructing buildings" which will check all county improvements
            // under construction and if they are done it will go through the list of people there and make them ide.
            // Here is old code for it:
            // Checks to see if the building is completed.
            /*
            if (countyPopulation.CurrentConstruction.currentAmountOfConstruction
                >= countyPopulation.CurrentConstruction.maxAmountOfConstruction)
            {
                // This is having every population working on that building set that building as built.
                // So it is repeating the setting to true a bunch of times.  This is ineffecient code.
                // Some of the population will be working on different buildings too....
                county.countyData.completedCountyImprovements.Add(countyPopulation.CurrentConstruction);
                countyPopulation.CurrentConstruction.status = AllEnums.CountyImprovementStatus.Completed;
            }
            */
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

