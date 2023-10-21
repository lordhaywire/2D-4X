using Godot;
using System;

namespace PlayerSpace
{
    public partial class CountyPopulation : Node
    {
        //public GameObject location;
        //public GameObject destination;

        //public string faction;

        [Export] public string firstName;
        public string lastName;
        public bool isMale;
        public int age;

        public bool isFactionLeader;
        public bool isHero;
        public bool isWorker;

        [ExportGroup("Perks")]
        public bool leaderOfPeoplePerk;

        //[ExportGroup("Attributes")]

        [ExportGroup("Skills")]
        public int constructionSkill;

        [ExportGroup("Activities")]
        public string currentActivity;
        //public GameObject currentBuilding; // What this person is currently building that day.
        public string nextActivity;
        //public GameObject nextBuilding;

        [ExportGroup("Token")]
        public bool isSpawned;

        
        public CountyPopulation(
            string firstName, string lastName, bool isMale, int age, bool isFactionLeader, bool isHero, bool isWorker
             , bool leaderOfPeoplePerk, int constructionSkill, string currentActivity
            , string nextActivity, bool isSpawned)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.isMale = isMale;
            this.age = age;

            this.isFactionLeader = isFactionLeader;
            this.isHero = isHero;
            this.isWorker = isWorker;

            this.leaderOfPeoplePerk = leaderOfPeoplePerk;

            this.constructionSkill = constructionSkill;

            this.currentActivity = currentActivity;
            this.nextActivity = nextActivity;

            this.isSpawned = isSpawned;
        }
        
    }

}
