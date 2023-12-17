using Godot;
using System;

namespace PlayerSpace
{
    public partial class CountyPopulation : Node
    {
        public int location;
        public int destination;

        //public string faction;
        [ExportGroup("Info")]
        [Export] public string firstName;
        public string lastName;
        public bool isMale;
        public int age;

        public bool isFactionLeader;
        public bool isHero;
        public bool isArmyLeader;
        public bool isWorker;

        [ExportGroup("Perks")]
        public bool leaderOfPeoplePerk;

        //[ExportGroup("Attributes")]

        [ExportGroup("Skills")]
        public int constructionSkill;

        [ExportGroup("Activities")]
        public string currentActivity;
        public CountyImprovementData currentImprovement; // What this person is currently building that day.
        public string nextActivity;
        public CountyImprovementData nextImprovement;

        [ExportGroup("Token")]
        public CharacterBody2D token;

        public CountyPopulation(

            int location, int destionation, string firstName, string lastName, bool isMale, int age, bool isFactionLeader
            , bool isHero, bool isArmyLeader, bool isWorker, bool leaderOfPeoplePerk, int constructionSkill, string currentActivity
            , CountyImprovementData currentImprovement, string nextActivity, CountyImprovementData nextImprovement)
        {
            this.location = location;
            this.destination = destionation;
            this.firstName = firstName;
            this.lastName = lastName;
            this.isMale = isMale;
            this.age = age;

            this.isFactionLeader = isFactionLeader;
            this.isHero = isHero;
            this.isArmyLeader = isArmyLeader;
            this.isWorker = isWorker;

            this.leaderOfPeoplePerk = leaderOfPeoplePerk;

            this.constructionSkill = constructionSkill;

            this.currentImprovement = currentImprovement;
            this.currentActivity = currentActivity;
            this.nextImprovement = nextImprovement;
            this.nextActivity = nextActivity;
        }
    }
}
