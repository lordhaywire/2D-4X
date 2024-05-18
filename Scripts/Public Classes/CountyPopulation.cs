using Godot;
using System.Collections.Generic;

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
        public List<PerkData> perks;

        [ExportGroup("Token Expendables")]
        public int moraleExpendable; // I think we are going to have to have this as leader morale or army morale or some shit.
        public int loyaltyAttribute; // I think this needs to be renamed to an expendable.

        [ExportGroup("Attributes")]
        public Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> attributes = [];

        [ExportGroup("Skills")]
        public Godot.Collections.Dictionary<AllEnums.Skills, SkillData> skills = [];
        public SkillData preferredSkill;

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
            , List<PerkData> perks, int moraleExpendable, int loyaltyAttribute
            , Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> attributes
            , Godot.Collections.Dictionary<AllEnums.Skills, SkillData> skills
            , SkillData preferredSkill, string currentActivity
            , CountyImprovementData currentImprovement, string nextActivity, CountyImprovementData nextImprovement
            , ResearchItemData CurrentResearchItemData)
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
            this.isFactionLeader = isLeader; // What the fuck?
            this.isAide = isAide;
            this.IsArmyLeader = IsArmyLeader;
            this.isWorker = isWorker;

            this.perks = perks;

            this.moraleExpendable = moraleExpendable;
            this.loyaltyAttribute = loyaltyAttribute;
            this.attributes = attributes;

            this.skills = skills;
            this.preferredSkill = preferredSkill;

            this.currentActivity = currentActivity;
            this.currentImprovement = currentImprovement;
            this.nextActivity = nextActivity;
            this.nextImprovement = nextImprovement;
            this.CurrentResearchItemData = CurrentResearchItemData;
        }
    }
}
