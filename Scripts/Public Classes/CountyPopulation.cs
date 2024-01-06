using Godot;
using System;

namespace PlayerSpace
{
    public class CountyPopulation
    {
        public FactionData factionData;
        public int location;
        public int destination;

        //public string faction;
        [ExportGroup("Info")]
        [Export] public string firstName;
        public string lastName;
        public bool isMale;
        public int age;

        public bool isHero;
        public bool isLeader;
        public bool isAide;
        public bool isArmyLeader;
        public bool isWorker;

        [ExportGroup("Perks")]
        public bool leaderOfPeoplePerk;

        [ExportGroup("Token Expendables")]
        public int moraleExpendable; // I think we are going to have to have this as leader morale or army morale or some shit.

        [ExportGroup("Attributes")]
        public int coolAttribute;

        [ExportGroup("Skills")]
        public int constructionSkill;
        public int rifleSkill;

        [ExportGroup("Activities")]
        public string currentActivity;
        public CountyImprovementData currentImprovement; // What this person is currently building that day.
        public string nextActivity;
        public CountyImprovementData nextImprovement;

        [ExportGroup("Token")]
        public CharacterBody2D token;

        public CountyPopulation(
            FactionData factionData, int location, int destination, string firstName, string lastName, bool isMale, int age, bool isHero, bool isLeader
            , bool isAide, bool isArmyLeader, bool isWorker, bool leaderOfPeoplePerk, int moraleExpendable, 
            int coolAttribute, int constructionSkill, int rifleSkill, string currentActivity
            , CountyImprovementData currentImprovement, string nextActivity, CountyImprovementData nextImprovement)
        {
            this.factionData = factionData;
            this.location = location;
            this.destination = destination;
            this.firstName = firstName;
            this.lastName = lastName;
            this.isMale = isMale;
            this.age = age;

            this.isHero = isHero;
            this.isLeader = isLeader;
            this.isAide = isAide;
            this.isArmyLeader = isArmyLeader;
            this.isWorker = isWorker;

            this.leaderOfPeoplePerk = leaderOfPeoplePerk;

            this.moraleExpendable = moraleExpendable;

            this.coolAttribute = coolAttribute;

            this.constructionSkill = constructionSkill;
            this.rifleSkill = rifleSkill;

            this.currentImprovement = currentImprovement;
            this.currentActivity = currentActivity;
            this.nextImprovement = nextImprovement;
            this.nextActivity = nextActivity;
        }
    }
}
