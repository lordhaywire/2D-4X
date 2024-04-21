using Godot;
using System.ComponentModel;

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
        public int physicalStrength;
        public int agility;
        public int endurance;
        public int intelligence;
        public int mentalStrength;
        public int awareness;
        public int charisma;
        public int looks;

        public int loyaltyAttribute;

        [ExportGroup("Skills")]
        public SkillData constructionSkill;
        public SkillData coolSkill;
        public SkillData researchingSkill;
        public SkillData rifleSkill;

        [ExportGroup("Activities")]
        public string currentActivity;
        public CountyImprovementData currentImprovement; // What this person is currently building that day.
        public string nextActivity;
        public CountyImprovementData nextImprovement;
        private ResearchItemData currentResearchItemData;

        public ResearchItemData CurrentResearchItemData
        {
            get { return currentResearchItemData; }
            set
            {
                currentResearchItemData = value;
                if (currentResearchItemData == null)
                {
                    nextActivity = AllText.Activities.IDLE;
                    if (CountyInfoControl.Instance?.Visible == true)
                    {
                        CountyInfoControl.Instance.GenerateHeroesPanelList();
                    }
                }
                else
                {
                    nextActivity = AllText.Activities.RESEARCHING;
                }
                if (ResearchControl.Instance?.Visible == true)
                {
                    //GD.Print("It got to the Visiblity Check on Current Research Item Data.");
                    ResearchControl.Instance.CheckForResearchers();
                }
            }
        }

        [ExportGroup("Token")]
        public SelectToken token;

        // They always need to be a hero first for everything else to work.
        public void ChangeToArmy()
        {
            isHero = true;
            IsArmyLeader = true;
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(location);
            selectCounty.countyData.armiesInCountyList.Add(this);
            selectCounty.countyData.herosInCountyList.Remove(this);
        }

        public CountyPopulation(
            FactionData factionData, int location, int lastLocation, int destination, string firstName, string lastName
            , bool isMale, int age, bool isHero, bool isLeader, bool isAide, bool IsArmyLeader, bool isWorker
            , bool leaderOfPeoplePerk, int moraleExpendable,  int physicalStrength, int agility, int endurance
            , int intelligence, int mentalStrength, int awareness, int charisma, int looks, int loyaltyAttribute
            ,  SkillData constructionSkill, SkillData coolSkill, SkillData researchingSkill, SkillData rifleSkill
            , string currentActivity, CountyImprovementData currentImprovement, string nextActivity
            , CountyImprovementData nextImprovement, ResearchItemData CurrentResearchItemData)
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

            this.physicalStrength = physicalStrength;
            this.agility = agility;
            this.endurance = endurance;
            this.intelligence = intelligence;
            this.mentalStrength = mentalStrength;
            this.awareness = awareness;
            this.charisma = charisma;
            this.looks = looks;

            this.loyaltyAttribute = loyaltyAttribute;

            this.constructionSkill = constructionSkill;
            this.coolSkill = coolSkill;
            this.researchingSkill = researchingSkill;
            this.rifleSkill = rifleSkill;

            this.currentActivity = currentActivity;
            this.currentImprovement = currentImprovement;
            this.nextActivity = nextActivity;
            this.nextImprovement = nextImprovement;
            this.CurrentResearchItemData = CurrentResearchItemData;
        }
    }
}
