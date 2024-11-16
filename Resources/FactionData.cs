using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class FactionData : Resource
    {
        [ExportGroup("Faction Info")]
        [Export] public int factionID;
        [Export] public bool isPlayer;
        [Export] public string factionName;
        [Export] public Color factionColor;
        [Export] public int factionCapitalCounty;

        public List<ResearchItemData> researchItems = [];
        readonly List<ResearchItemData> researchableResearch = [];

        public List<CountyData> countiesFactionOwns = [];
        public List<CountyPopulation> allHeroesList = [];
        public CountyPopulation factionLeader;

        public Diplomacy diplomacy = new();
        public TokenSpawner tokenSpawner = new();

        public List<CountyImprovementData> allCountyImprovements = []; // This includes all county improvements, even possible ones.

        // Resources.
        public Godot.Collections.Dictionary<AllEnums.FactionResourceType, FactionResourceData> factionResources = [];
        public Godot.Collections.Dictionary<AllEnums.FactionResourceType, FactionResourceData> yesterdaysFactionResources = [];
        public Godot.Collections.Dictionary<AllEnums.FactionResourceType, FactionResourceData> amountUsedFactionResources = [];
        /// <summary>
        /// I am not sure we need this.
        /// </summary>
        //[Obsolete("What the flying fuck is this?")] public Godot.Collections.Dictionary<AllEnums.FactionResourceType, FactionResourceData> actualUsedFactionResources = [];

        [ExportGroup("Diplomatic Incidences")]
        public List<War> wars = [];

        [ExportGroup("Diplomatic Matrix")]
        [Export] public Godot.Collections.Dictionary<string, bool> factionWarDictionary = [];

        // Goes through all the population and adds a set number to research.
        // It should check what they are doing and try to add that research then if they aren't doing anything
        // it should add to a random research that isn't done yet.
        // Don't forget about idle heroes researching other things.
        public void PopulationResearch(CountyData countyData)
        {
            // Get a list of all the research that isn't done.
            CreateResearchableResearchList();

            // Have the population research by their interests or job.
            CheckEachPeopleForResearch(countyData);
        }

        public static FactionData GetFactionDataFromID(int id)
        {
            //GD.Print("Faction ID that is trying to be used: " + id);
            Faction faction = (Faction)Globals.Instance.factionsParent.GetChild(id);
            return faction.factionData;
        }

        /// <summary>
        /// Have the population research by their interests or job.
        /// </summary>
        private void CreateResearchableResearchList()
        {
            researchableResearch.Clear();
            foreach (ResearchItemData researchItemData in researchItems)
            {
                if (researchItemData.CheckIfResearchDone() == false
                    && researchItemData.CheckIfPrerequisitesAreDone() == true)
                {
                    researchableResearch.Add(researchItemData);
                }
            }
            /*
            foreach(ResearchItemData researchItemData1 in researchableResearch)
            {
                GD.Print("Researchable Research: " + researchItemData1.researchName);
            }
            */
        }

        private void CheckEachPeopleForResearch(CountyData countyData)
        {
            foreach (CountyPopulation countyPopulation in countyData.countyPopulationList)
            {
                ResearchByInterest(countyPopulation);
                ResearchByJob(countyPopulation);
            }
        }

        private void ResearchByJob(CountyPopulation countyPopulation)
        {
            ResearchItemData whatPopulationIsResearching = null;

            foreach (ResearchItemData researchItemData in researchableResearch)
            {
                // If the county improvement isn't null then see if the interest matches.
                if (countyPopulation.currentCountyImprovement?.interest == researchItemData.interest)
                {
                    whatPopulationIsResearching = researchItemData;
                    //GD.Print($"{countyPopulation.firstName} {countyPopulation.interest.name} is having them research {researchItemData.researchName}");
                    break;
                }
            }

            if (whatPopulationIsResearching != null)
            {
                // After a skill check, have the banker add the research to the research with a possible bonus.
                Banker.IncreaseResearchAmountWithBonus(countyPopulation, whatPopulationIsResearching, researchableResearch);
            }
        }

        private void ResearchByInterest(CountyPopulation countyPopulation)
        {
            ResearchItemData whatPopulationIsResearching = null;

            foreach (ResearchItemData researchItemData in researchableResearch)
            {
                if (countyPopulation.interest.interestType == researchItemData.interest.interestType)
                {
                    whatPopulationIsResearching = researchItemData;
                    //GD.Print($"{countyPopulation.firstName} {countyPopulation.interest.name} is having them research {researchItemData.researchName}");
                    break;
                }
            }

            if (whatPopulationIsResearching != null)
            {
                // After a skill check, have the banker add the research to the research with a possible bonus.
                Banker.IncreaseResearchAmountWithBonus(countyPopulation, whatPopulationIsResearching, researchableResearch);
            }
        }
        public void CopyFactionResourcesToYesterday()
        {
            // Creating a deep copy of the dictionary
            yesterdaysFactionResources = [];
            foreach (KeyValuePair<AllEnums.FactionResourceType, FactionResourceData> keyValuePair in factionResources)
            {
                yesterdaysFactionResources.Add(keyValuePair.Key, new FactionResourceData
                {
                    GoodName = keyValuePair.Value.GoodName,
                    description = keyValuePair.Value.description,
                    resourceType = keyValuePair.Value.resourceType,
                    amount = keyValuePair.Value.amount,
                });
            }
            if (isPlayer)
            {
                //GD.Print("Yesterday's Influence: " + yesterdaysFactionResources[AllEnums.FactionResourceType.Influence].amount);
                //GD.Print("This Influence should be the same as yesterdays: " + factionResources[AllEnums.FactionResourceType.Influence].amount);
            }
        }
        public void AddCountyImprovementToAllCountyImprovements(CountyImprovementData countyImprovementData)
        {
            allCountyImprovements.Add(CountyImprovementData.NewCopy(countyImprovementData));
            //GD.PrintRich($"[rainbow][tornado]{factionName} {countyImprovementData.improvementName} has been added.");
            // Alphabetize the list by improvementName
            allCountyImprovements
                = [.. allCountyImprovements.OrderBy(improvement => Tr(improvement.improvementName))];

        }
        // Zero resources that are summed from each county.
        // Why not foreach this and skip the first two?
        public void ZeroFactionCountyResources()
        {
            factionResources[AllEnums.FactionResourceType.Food].amount = 0;
            factionResources[AllEnums.FactionResourceType.Remnants].amount = 0;
            factionResources[AllEnums.FactionResourceType.BuildingMaterial].amount = 0;
            factionResources[AllEnums.FactionResourceType.Equipment].amount = 0;
        }

        // This should be counting just the county resources of Faction Type, not the used.
        public void CountAllCountyFactionResources()
        {
            ZeroFactionCountyResources();
            foreach (CountyData countyData in countiesFactionOwns)
            {
                factionResources[AllEnums.FactionResourceType.Food].amount
                    += countyData.CountFactionResourceOfType(AllEnums.FactionResourceType.Food);
                factionResources[AllEnums.FactionResourceType.Remnants].amount
                    += countyData.CountFactionResourceOfType(AllEnums.FactionResourceType.Remnants);
                factionResources[AllEnums.FactionResourceType.BuildingMaterial].amount
                    += countyData.CountFactionResourceOfType(AllEnums.FactionResourceType.BuildingMaterial);
                factionResources[AllEnums.FactionResourceType.Equipment].amount
                    += countyData.CountFactionResourceOfType(AllEnums.FactionResourceType.Equipment);
            }
        }

        public void CountAllCountyFactionUsedResources()
        {
            ZeroFactionCountyActualUsedResources();
            foreach (CountyData countyData in countiesFactionOwns)
            {
                amountUsedFactionResources[AllEnums.FactionResourceType.Food].amount
                    += countyData.CountUsedFactionResourceOfType(AllEnums.FactionResourceType.Food);
                amountUsedFactionResources[AllEnums.FactionResourceType.Remnants].amount
                    += countyData.CountUsedFactionResourceOfType(AllEnums.FactionResourceType.Remnants);
                amountUsedFactionResources[AllEnums.FactionResourceType.BuildingMaterial].amount
                    += countyData.CountUsedFactionResourceOfType(AllEnums.FactionResourceType.BuildingMaterial);
            }
        }

        // This is almost identical to the other zeroing out thing.
        // Why not a foreach loop and skip the first two?
        private void ZeroFactionCountyActualUsedResources()
        {
            amountUsedFactionResources[AllEnums.FactionResourceType.Food].amount = 0;
            amountUsedFactionResources[AllEnums.FactionResourceType.Remnants].amount = 0;
            amountUsedFactionResources[AllEnums.FactionResourceType.BuildingMaterial].amount = 0;
            amountUsedFactionResources[AllEnums.FactionResourceType.Equipment].amount = 0;
        }

        public void SubtractFactionResources()
        {
            // Do the math for amount used. Subtract yesterdays from todays and that is how much we have used.
            foreach (KeyValuePair<AllEnums.FactionResourceType, FactionResourceData> keyValuePair in factionResources)
            {
                amountUsedFactionResources[keyValuePair.Key].amount = factionResources[keyValuePair.Key].amount -
                    yesterdaysFactionResources[keyValuePair.Key].amount;
            }
            if (isPlayer)
            {
                //GD.Print("After subtraction yesterdays influence is: " + yesterdaysFactionResources[AllEnums.FactionResourceType.Influence].amount);
            }
        }
    }
}