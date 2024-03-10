using Godot;

namespace PlayerSpace
{
    public class CountyPopulation
    {
        public FactionData factionData;
        public int location;

        public int lastLocation;
        public int destination;

        //public string faction;
        [ExportGroup("Info")]
        [Export] public string firstName;
        public string lastName;
        public bool isMale;
        public int age;

        public bool isHero;
        public bool isFactionLeader;
        public bool isAide;
        private bool isArmyLeader;
        public bool isWorker;

        public bool IsArmyLeader
        {
            get { return isArmyLeader; }
            set
            {
                isArmyLeader = value;
                if (token != null)
                {
                    AllTokenTextures.Instance.AssignTokenTextures(token);
                }
            }
        }

        [ExportGroup("Perks")]
        public bool leaderOfPeoplePerk;

        [ExportGroup("Token Expendables")]
        public int moraleExpendable; // I think we are going to have to have this as leader morale or army morale or some shit.

        [ExportGroup("Attributes")]
        public int loyaltyAttribute;

        [ExportGroup("Skills")]
        public int coolSkill; // I think this might be more of a skill.
        public int constructionSkill;
        public int rifleSkill;

        [ExportGroup("Activities")]
        public string currentActivity;
        public CountyImprovementData currentImprovement; // What this person is currently building that day.
        public string nextActivity;
        public CountyImprovementData nextImprovement;

        [ExportGroup("Token")]
        public SelectToken token;

        public void ChangeToArmy()
        {
            isHero = true;
            IsArmyLeader = true;
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(location);
            selectCounty.countyData.heroCountyList.Add(this);
            selectCounty.countyData.countyPopulationList.Remove(this);
        }

        public CountyPopulation(
            FactionData factionData, int location, int lastLocation, int destination, string firstName, string lastName, bool isMale, int age, bool isHero, bool isLeader
            , bool isAide, bool IsArmyLeader, bool isWorker, bool leaderOfPeoplePerk, int moraleExpendable,
            int coolSkill, int loyaltyAttribute, int constructionSkill, int rifleSkill, string currentActivity
            , CountyImprovementData currentImprovement, string nextActivity, CountyImprovementData nextImprovement)
        {
            this.factionData = factionData;
            this.location = location;
            this.lastLocation = lastLocation;
            this.destination = destination;
            this.firstName = firstName;
            this.lastName = lastName;
            this.isMale = isMale;
            this.age = age;

            this.isHero = isHero;
            this.isFactionLeader = isLeader;
            this.isAide = isAide;
            this.IsArmyLeader = IsArmyLeader;
            this.isWorker = isWorker;

            this.leaderOfPeoplePerk = leaderOfPeoplePerk;

            this.moraleExpendable = moraleExpendable;

            this.loyaltyAttribute = loyaltyAttribute;

            this.coolSkill = coolSkill;
            this.constructionSkill = constructionSkill;
            this.rifleSkill = rifleSkill;

            this.currentImprovement = currentImprovement;
            this.currentActivity = currentActivity;
            this.nextImprovement = nextImprovement;
            this.nextActivity = nextActivity;
        }
    }
}
