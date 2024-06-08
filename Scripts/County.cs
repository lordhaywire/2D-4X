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
            Work work = new();

            GD.Print("County Hour Zero.");
            countyAI.DecideBuildingCountyImprovements(this);
            // We are probably going to get rid of work, meaning move the code to another script.
            work.WorkDayOverForPopulation(countyData.countyNode);
        }

        private void DayStart()
        {
            PopulationAI populationAI = new();
            // This is the one with the dumb name.
            populationAI.CheckForWork(this);
        }
        private void OnTreeExit()
        {
            Clock.Instance.SetDay -= EndOfDay;
            Clock.Instance.SetDay -= DayStart;
        }
    }
}

