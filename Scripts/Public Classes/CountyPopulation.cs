using Godot;
using System;

namespace PlayerSpace
{
    public partial class CountyPopulation : Resource
    {
        [Export] public FactionData factionData;
        [Export] public int location;

        [Export] public int lastLocation;
        [Export] public int destination;

        [ExportGroup("Info")]
        [Export] public string firstName;
        [Export] public string lastName;
        [Export] public bool isMale;
        [Export] public int age;

        [Export] public bool isHero;
        [Export] public bool isFactionLeader;
        [Export] public bool isAide;
        [Export] private bool isArmyLeader;
        [Export] public bool isWorker;

        [Export]
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
        [Export] public Godot.Collections.Dictionary<AllEnums.Perks, PerkData> perks;

        [ExportGroup("Expendables")]
        [Export] public int hitpoints;
        [Export] public int maxHitpoints;

        [Export] public int moraleExpendable; // I think we are going to have to have this as leader morale or army morale or some shit.
        [Export] private int loyaltyBase;
        [Export] private int loyaltyAdjusted;
        [Export]
        public int LoyaltyAdjusted
        {
            get { return loyaltyAdjusted; }
            set
            {
                // Make it so that loyaltyAdjusted can't go above 100.
                loyaltyAdjusted = Math.Min(value, 100);
            }
        }
        [Export] private int happiness;

        [Export]
        public int Happiness
        {
            get { return happiness; }
            set
            {
                happiness = Math.Min(value, 100);
                LoyaltyAdjusted = loyaltyBase + AttributeData.ApplyAttributeBonuses(value, false, false);
                //GD.Print($"{firstName} {lastName} loyalty adjusted: {LoyaltyAdjusted}");
            }
        }

        [Export] public int employedDaysIdle;
        [Export] public int daysStarving;

        // Resource needs, currently there is just 1 need, Remnants.
        [Export] public Godot.Collections.Dictionary<AllEnums.CountyGoodType, int> needs;

        [ExportGroup("Attributes")]
        [Export] public Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> attributes = [];

        [ExportGroup("Skills")]
        [Export] public Godot.Collections.Dictionary<AllEnums.Skills, SkillData> skills = [];
        [Export] public SkillData preferredSkill;
        [Export] public InterestData interestData;

        [ExportGroup("Work")]
        [Export] public AllEnums.Activities activity;
        [Export] public CountyImprovementData currentCountyImprovement; // Used for work and building.

        [Export] public ResearchItemData passiveResearchItemData;
        [Export] public ResearchItemData currentResearchItemData;

        public SelectToken token;

        public bool CheckForPerk(AllEnums.Perks perk)
        {
            if (perks.ContainsKey(perk))
            {
                return true;
            }
            return false;
        }

        // They always need to be a hero first for everything else to work.
        public void ChangeToArmy()
        {
            isHero = true;
            IsArmyLeader = true;
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(location);
            selectCounty.countyData.armiesInCountyList.Add(this);
            selectCounty.countyData.heroesInCountyList.Remove(this);
        }
        public void UpdateActivity(AllEnums.Activities activity)
        {
            this.activity = activity;
        }

        public void UpdateDestination(int destination)
        {
            this.destination = destination;
        }
        public void UpdateCurrentCountyImprovement(CountyImprovementData countyImprovementData)
        {
            currentCountyImprovement = countyImprovementData;
        }

        public void AddRandomHappiness(int maxHappiness)
        {
            Random random = new();
            Happiness += random.Next(1, maxHappiness);
            //GD.Print($"Happiness gained: {firstName} {lastName} happiness is now {Happiness}");
        }

        public string GetActivityName()
        {
            string name = TranslationServer.Translate(AllActivities.Instance.allActivityData[(int)activity].name);
            return name;
        }

        public void RemoveRandomHappiness(int maxHappiness)
        {
            Random random = new();
            Happiness -= random.Next(1, maxHappiness);
            //GD.Print($"Happiness lost: {firstName} {lastName} happiness is now {Happiness}");
        }

        public bool CheckWillWorkLoyalty()
        {
            if (LoyaltyAdjusted >= Globals.Instance.willWorkLoyalty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Makes activity idle and removes the population from the populationAtImprovement list.
        /// </summary>
        public void RemoveFromCountyImprovement()
        {
            UpdateActivity(AllEnums.Activities.Idle);
            currentCountyImprovement?.populationAtImprovement.Remove(this);
            currentCountyImprovement = null;
        }

        public void UpdateCurrentResearch(ResearchItemData researchItemData)
        {
            UpdateActivity(AllEnums.Activities.Research);
            currentResearchItemData = researchItemData;
        }

        /// <summary>
        /// Removes the populations research Item Data and sets their activity to Work.
        /// </summary>
        public void RemoveFromResearch()
        {
            currentResearchItemData = null;
            UpdateActivity(AllEnums.Activities.Work);
            // When the AI removes people from research it is going to try and do this.  I am not sure we care.
            ResearchControl.Instance.assignedResearchers.Remove(this);
        }

        public CountyPopulation(
            FactionData factionData, int location, int lastLocation, int destination, string firstName, string lastName
            , bool isMale, int age, bool isHero, bool isFactionLeader, bool isAide, bool IsArmyLeader, bool isWorker
            , Godot.Collections.Dictionary<AllEnums.Perks, PerkData> perks, int hitpoints, int maxHitpoints
            , int moraleExpendable
            , int loyaltyBase, int LoyaltyAdjusted, int Happiness, int daysStarving
            , Godot.Collections.Dictionary<AllEnums.CountyGoodType, int> needs
            , Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> attributes
            , Godot.Collections.Dictionary<AllEnums.Skills, SkillData> skills
            , SkillData preferredSkill, InterestData interest, AllEnums.Activities activity
            , CountyImprovementData currentCountyImprovement
            , ResearchItemData passiveResearchItemData
            , ResearchItemData currentResearchItemData)
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
            this.isFactionLeader = isFactionLeader;
            this.isAide = isAide;
            this.IsArmyLeader = IsArmyLeader;
            this.isWorker = isWorker;

            this.perks = perks;

            this.hitpoints = hitpoints;
            this.maxHitpoints = maxHitpoints;
            this.moraleExpendable = moraleExpendable;
            this.loyaltyBase = loyaltyBase;
            this.LoyaltyAdjusted = LoyaltyAdjusted;
            this.Happiness = Happiness;
            this.daysStarving = daysStarving;
            this.needs = needs;
            this.attributes = attributes;

            this.skills = skills;
            this.preferredSkill = preferredSkill;
            this.interestData = interest;

            this.activity = activity;
            this.currentCountyImprovement = currentCountyImprovement;
            this.passiveResearchItemData = passiveResearchItemData;
            this.currentResearchItemData = currentResearchItemData;
        }
    }
}
