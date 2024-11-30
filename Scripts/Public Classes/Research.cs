using Godot;
using System;

namespace PlayerSpace
{
    public class Research
    {
        /// <summary>
        /// Create a faction level list of researable research.
        /// </summary>
        public static void CreateResearchableResearchList(FactionData factionData)
        {
            GD.PrintRich($"[rainbow]Create Researchable Research List");
            factionData.researchableResearch.Clear();
            foreach (ResearchItemData researchItemData in factionData.researchItems)
            {
                if (researchItemData.CheckIfResearchDone() == false
                    && researchItemData.CheckIfPrerequisitesAreDone() == true)
                {
                    factionData.researchableResearch.Add(researchItemData);
                }
            }

            foreach (ResearchItemData researchItemData1 in factionData.researchableResearch)
            {
                GD.Print("Researchable Research: " + researchItemData1.researchName);
            }
        }

        /// <summary>
        /// Passive research for every hero the faction controls.  It doesn't matter where they are right now.
        /// Eventually they will research items depending on what they are doing in other places, such as
        /// diplomacy or spying.
        /// 
        /// Heroes always get passive research even if they are researching something else.
        /// Their office research is like their day job, and the passive research is them just
        /// musing over the same topic as their current research.
        /// 
        /// They can get passive research on the research item they are assigned to.
        /// 
        /// Assign by Interest then by activity and if it can't find any research then assign a 
        /// random passive research. This includes idle and moving.
        /// </summary>
        /// <param name="factionData"></param>
        public static void AssignPassiveResearch(FactionData factionData
            , Godot.Collections.Array<CountyPopulation> countyPopulationList)
        {
            foreach (CountyPopulation countyPopulation in countyPopulationList)
            {
                countyPopulation.passiveResearchItemData = GetResearchByInterest(countyPopulation);
                countyPopulation.passiveResearchItemData ??= GetResearchByActivity(countyPopulation);
                countyPopulation.passiveResearchItemData ??= GetLowestTierRandomResearch(countyPopulation.factionData);
                if (countyPopulation.passiveResearchItemData != null)
                {
                    GD.Print($"Final Passive Research Outcome: {countyPopulation.firstName} " +
                        $": {countyPopulation.passiveResearchItemData.researchName}");
                }
            }
        }

        /// <summary>
        /// It assigns the first item in the researable research list that matches the county population
        /// interest.
        /// </summary>
        /// <param name="countyPopulation"></param>
        private static ResearchItemData GetResearchByInterest(CountyPopulation countyPopulation)
        {
            //GD.Print("Assign Research By Interest to: " + countyPopulation.firstName);
            ResearchItemData researchItemData
                = GetRandomResearchByInterestType(countyPopulation.factionData
                , countyPopulation.interestData.interestType);
            /*
            GD.Print($"{countyPopulation.firstName} {countyPopulation.interestData.name} " +
                $"is having them research {researchItemData?.researchName}" +
               $" : if this is blank then their interest doesn't match.");
            */
            return researchItemData;
        }

        // I think this needs to get changed to find a random research by interest type.
        /// <summary>
        /// I can't decide if I want the county population/heroes to research depending on what they are doing,
        /// or if they should just get random research after it assigns their interest research.
        /// </summary>
        /// <param name="countyPopulation"></param>
        /// <returns></returns>
        private static ResearchItemData GetResearchByActivity(CountyPopulation countyPopulation)
        {
            ResearchItemData whatPopulationIsResearching;
            switch (countyPopulation.activity)
            {
                case AllEnums.Activities.Build:
                    whatPopulationIsResearching
                    = GetRandomResearchByInterestType(countyPopulation.factionData
                    , AllEnums.InterestType.Engineering);
                    break;
                case AllEnums.Activities.Combat:
                    whatPopulationIsResearching
                    = GetRandomResearchByInterestType(countyPopulation.factionData
                    , AllEnums.InterestType.Warfare);
                    break;
                case AllEnums.Activities.Research:
                    whatPopulationIsResearching
                    = GetRandomResearchByInterestType(countyPopulation.factionData
                    , countyPopulation.currentResearchItemData.interestData.interestType);
                    break;
                case AllEnums.Activities.Work:
                    whatPopulationIsResearching
                    = GetRandomResearchByInterestType(countyPopulation.factionData
                    , countyPopulation.currentCountyImprovement.interestData.interestType);
                    break;
                // If they are idle, scavenging or moving they get random research.
                case AllEnums.Activities.Scavenge:
                case AllEnums.Activities.Idle:
                case AllEnums.Activities.Move:
                    whatPopulationIsResearching = GetLowestTierRandomResearch(countyPopulation.factionData);
                    /*
                    GD.Print($"{countyPopulation.firstName} is either idle, scavenging or moving so they are getting " +
                        $"random passive research: {whatPopulationIsResearching.researchName}");
                    */
                    break;
                default:
                    throw new NotImplementedException("Josh says, No case in AssignResearchByActivity!");
            }
            return whatPopulationIsResearching;
        }

        private static ResearchItemData GetLowestTierRandomResearch(FactionData factionData)
        {
            Random random = new();

            // List of research by the lowest tier that is researched.
            Godot.Collections.Array<ResearchItemData> researchByLowestTier = [];
            // The current research tier is gotten from the first item in the researchableResearch list.
            ResearchItemData randomResearchItemData = null;
            if (factionData.researchableResearch.Count > 0)
            {
                AllEnums.ResearchTiers researchTier = factionData.researchableResearch[0].tier;
                foreach (ResearchItemData researchItemData in factionData.researchableResearch)
                {
                    if (researchItemData.tier == researchTier)
                    {
                        researchByLowestTier.Add(researchItemData);
                    }
                }

                randomResearchItemData = researchByLowestTier[random.Next(0, researchByLowestTier.Count)];
            }
            return randomResearchItemData;
        }

        // I think this needs to get changed to find a random research by interest type.
        private static ResearchItemData GetRandomResearchByInterestType(FactionData factionData
            , AllEnums.InterestType interestType)
        {
            Random random = new();
            ResearchItemData researchItemDataByInterest = null;
            Godot.Collections.Array<ResearchItemData> researchByInterestList = [];

            foreach (ResearchItemData researchItemData in factionData.researchableResearch)
            {
                if (interestType == researchItemData.interestData.interestType)
                {
                    researchByInterestList.Add(researchItemData);
                }
            }
            // This will return a random research by interest if the list is not null.
            if (researchByInterestList.Count > 0)
            {
                researchItemDataByInterest = researchByInterestList[random.Next(0, researchByInterestList.Count)];
            }
            /*
            else
            {
                researchItemDataByInterest = null; // GetLowestTierRandomResearch(factionData);
            }
            */
            //GD.Print($"Get Random Research By Interest Type: {researchItemDataByInterest?.researchName}");
            return researchItemDataByInterest;
        }

        // This needs to go to the ResearchItemData, maybe.
        public void RemoveResearcher(CountyPopulation countyPopulation)
        {
            ResearchControl.Instance.assignedResearchers.Remove(countyPopulation);
            countyPopulation.currentResearchItemData = null;
            // If the population isn't a hero then they must be working at a research office,
            // thus we need to make their activity be work.
            if (countyPopulation.isHero == false)
            {
                countyPopulation.UpdateActivity(AllEnums.Activities.Work);
            }
        }

        /// <summary>
        /// There is no learning check for population random research.
        /// </summary>
        public static void GeneratePassiveResearch(Godot.Collections.Array<CountyPopulation> researchers)
        {
            foreach (CountyPopulation countyPopulation in researchers)
            {
                if (countyPopulation.passiveResearchItemData == null)
                {
                    return;
                }

                if (SkillData.Check(countyPopulation
                    , countyPopulation.skills[countyPopulation.passiveResearchItemData.skill].skillLevel
                    , countyPopulation.skills[countyPopulation.passiveResearchItemData.skill].attribute
                    , false) == true)
                {
                    countyPopulation.passiveResearchItemData.AmountOfResearchDone
                        += Globals.Instance.passiveResearchIncrease + Globals.Instance.passiveResearchBonus;
                }
                else
                {
                    countyPopulation.passiveResearchItemData.AmountOfResearchDone
                        += Globals.Instance.passiveResearchIncrease;
                }
                GD.Print($"County Population: {countyPopulation.location} {countyPopulation.firstName}" +
                    $" {countyPopulation.passiveResearchItemData.researchName}: " +
                    $"{countyPopulation.passiveResearchItemData.AmountOfResearchDone}");


            }
        }
    }
}