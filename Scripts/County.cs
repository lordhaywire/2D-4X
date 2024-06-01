using Godot;
using System;
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
            countyData.countyNode = this; // Figure out why I did this.

            Clock.Instance.FirstRun += HourZero;
            Clock.Instance.HourZero += HourZero;

            Clock.Instance.WorkDayOver += WorkDayOverForPopulation;
        }

        private void WorkDayOverForPopulation()
        {
            Work work = new();
            work.WorkDayOverForPopulation(this);
        }

        private void HourZero()
        {
            CountyAI countyAI = new();
            PopulationAI populationAI = new();

            countyAI.CheckForBuildingCountyImprovements(this);
            populationAI.HourZero(this);
        }

        private void OnTreeExit()
        {
            Clock.Instance.FirstRun -= HourZero;
            Clock.Instance.HourZero -= HourZero;

            Clock.Instance.WorkDayOver -= WorkDayOverForPopulation;
        }
    }
}

