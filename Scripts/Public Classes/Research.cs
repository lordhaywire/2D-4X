using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    public class Research
    {
        /// <summary>
        /// Create a faction level list of researable research.
        /// </summary>
        public static void CreateResearchableResearchList(FactionData factionData)
        {
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
            , List<CountyPopulation> countyPopulationList)
        {
            foreach (CountyPopulation countyPopulation in countyPopulationList)
            {
                countyPopulation.passiveResearchItemData = AssignResearchByInterest(countyPopulation);
                countyPopulation.passiveResearchItemData ??= AssignResearchByActivity(countyPopulation);
                countyPopulation.passiveResearchItemData ??= GetRandomResearch(countyPopulation.factionData);
            }
        }

        /// <summary>
        /// It assigns the first item in the researable research list that matches the county population
        /// interest.
        /// </summary>
        /// <param name="countyPopulation"></param>
        private static ResearchItemData AssignResearchByInterest(CountyPopulation countyPopulation)
        {
            ResearchItemData researchItemData
                = GetFirstResearchByInterestType(countyPopulation.factionData
                , countyPopulation.interestData.interestType);
            GD.Print($"{countyPopulation.firstName} {countyPopulation.interestData.name} " +
                $"is having them research {countyPopulation.passiveResearchItemData.researchName}");
            return researchItemData;
        }

        // I think this needs to get changed to find a random research by interest type.
        /// <summary>
        /// I can't decide if I want the county population/heroes to research depending on what they are doing,
        /// or if they should just get random research after it assigns their interest research.
        /// </summary>
        /// <param name="countyPopulation"></param>
        /// <returns></returns>
        private static ResearchItemData AssignResearchByActivity(CountyPopulation countyPopulation)
        {
            ResearchItemData whatPopulationIsResearching = null;
            switch (countyPopulation.activity)
            {
                case AllEnums.Activities.Build:
                    whatPopulationIsResearching
                    = GetFirstResearchByInterestType(countyPopulation.factionData
                    , AllEnums.InterestType.Engineering);
                    break;
                case AllEnums.Activities.Combat:
                    whatPopulationIsResearching
                    = GetFirstResearchByInterestType(countyPopulation.factionData
                    , AllEnums.InterestType.Warfare);
                    break;
                case AllEnums.Activities.Research:
                    whatPopulationIsResearching
                    = GetFirstResearchByInterestType(countyPopulation.factionData
                    , countyPopulation.currentResearchItemData.interestData.interestType);
                    break;
                case AllEnums.Activities.Work:
                    whatPopulationIsResearching
                    = GetFirstResearchByInterestType(countyPopulation.factionData
                    , countyPopulation.currentCountyImprovement.interestData.interestType);
                    break;
                // If they are idle or moving they get random research.
                case AllEnums.Activities.Idle:
                case AllEnums.Activities.Move:
                    GD.Print($"{countyPopulation.firstName} is either idle or moving so they are getting " +
                        $"random passive research.");
                    whatPopulationIsResearching = GetRandomResearch(countyPopulation.factionData);
                    break;
            }
            return whatPopulationIsResearching;
        }

        private static ResearchItemData GetRandomResearch(FactionData factionData)
        {
            Random random = new();

            // List of research by the lowest tier that is researched.
            Godot.Collections.Array<ResearchItemData> researchByLowestTier = [];
            // The current research tier is gotten from the first item in the researchableResearch list.
            AllEnums.ResearchTiers researchTier = factionData.researchableResearch[0].tier;
            foreach (ResearchItemData researchItemData in factionData.researchableResearch)
            {
                if (researchItemData.tier == researchTier)
                {
                    researchByLowestTier.Add(researchItemData);
                }
            }

            ResearchItemData randomResearchItemData = researchByLowestTier[random.Next(0, researchByLowestTier.Count)];
            return randomResearchItemData;
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

        // I think this needs to get changed to find a random research by interest type.
        private static ResearchItemData GetFirstResearchByInterestType(FactionData factionData
            , AllEnums.InterestType interestType)
        {
            ResearchItemData researchItemDataByInterest = null;
            foreach (ResearchItemData researchItemData in factionData.researchableResearch)
            {
                if (interestType == researchItemData.interestData.interestType)
                {
                    researchItemDataByInterest = researchItemData;
                    break;
                }
            }
            return researchItemDataByInterest;
        }
    }
}