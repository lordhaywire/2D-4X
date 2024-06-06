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

            Clock.Instance.SetDay += HourZero;
        }

        private void HourZero()
        {
            CountyAI countyAI = new();
            PopulationAI populationAI = new();
            Research research = new();
            Work work = new();
            GD.Print("County Hour Zero.");
            countyAI.DecideBuildingCountyImprovements(this);
            research.PopulationResearch(this);

            work.WorkDayOverForPopulation(countyData.countyNode);
            populationAI.CheckForWork(this);
        }

        private void OnTreeExit()
        {
            Clock.Instance.SetDay -= HourZero;
        }
    }
}

