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

            /*
            foreach (ResearchItemData researchItemData1 in factionData.researchableResearch)
            {
                GD.Print("Researchable Research: " + researchItemData1.researchName);
            }
            */
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
        public static void AssignPassiveResearch(Godot.Collections.Array<PopulationData> populationDataList)
        {
            foreach (PopulationData populationData in populationDataList)
            {
                populationData.passiveResearchItemData = GetResearchByInterest(populationData);
                populationData.passiveResearchItemData ??= GetPassiveResearchByActivity(populationData);
                populationData.passiveResearchItemData ??= GetLowestTierRandomResearch(populationData.factionData);
                if (populationData.passiveResearchItemData != null)
                {
                    //GD.Print($"Final Passive Research Outcome: {populationData.firstName} " +
                    //   $": {populationData.passiveResearchItemData.researchName}");
                }
            }
        }

        /// <summary>
        /// It assigns the first item in the researable research list that matches the county population
        /// interest.
        /// </summary>
        /// <param name="populationData"></param>
        private static ResearchItemData GetResearchByInterest(PopulationData populationData)
        {
            //GD.Print("Assign Research By Interest to: " + populationData.firstName);
            ResearchItemData researchItemData
                = GetRandomResearchByInterestType(populationData.factionData
                , populationData.interestData.interestType);
            /*
            GD.Print($"{populationData.firstName} {populationData.interestData.name} " +
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
        /// <param name="populationData"></param>
        /// <returns></returns>
        private static ResearchItemData GetPassiveResearchByActivity(PopulationData populationData)
        {
            ResearchItemData whatPopulationIsResearching;
            switch (populationData.activity)
            {
                case AllEnums.Activities.Build:
                    whatPopulationIsResearching
                    = GetRandomResearchByInterestType(populationData.factionData
                    , AllEnums.InterestType.Engineering);
                    break;
                case AllEnums.Activities.Combat:
                    whatPopulationIsResearching
                    = GetRandomResearchByInterestType(populationData.factionData
                    , AllEnums.InterestType.Warfare);
                    break;
                case AllEnums.Activities.Research:
                    whatPopulationIsResearching
                    = GetRandomResearchByInterestType(populationData.factionData
                    , populationData.currentResearchItemData.interestData.interestType);
                    break;
                case AllEnums.Activities.Work:
                    whatPopulationIsResearching
                    = GetRandomResearchByInterestType(populationData.factionData
                    , populationData.currentCountyImprovement.interestData.interestType);
                    break;
                // If they are idle, scavenging, exploring or moving they get random research.
                case AllEnums.Activities.Explore:
                case AllEnums.Activities.Scavenge:
                case AllEnums.Activities.Idle:
                case AllEnums.Activities.Move:
                    whatPopulationIsResearching = GetLowestTierRandomResearch(populationData.factionData);
                    /*
                    GD.Print($"{populationData.firstName} is either idle, scavenging or moving so they are getting " +
                        $"random passive research: {whatPopulationIsResearching.researchName}");
                    */
                    break;
                default:
                    throw new NotImplementedException("Josh says, No case in AssignResearchByActivity!");
            }
            return whatPopulationIsResearching;
        }


        /// <summary>
        /// Check to see if there is a research office that isn't getting used by a hero.
        /// If a normal county population is using the research office it will show as available
        /// because the hero can replace them.
        /// </summary>
        /// <returns></returns>
        public static Godot.Collections.Array<CountyImprovementData> 
            GetListOfAvailableResearchOffices(FactionData factionData)
        {
            Godot.Collections.Array<CountyImprovementData> availableOffices = [];
            foreach (CountyImprovementData countyImprovementData in factionData.researchOffices)
            {
                // There can only be 1 person working at a research office currently.
                if(countyImprovementData.populationAtImprovement.Count < 1)
                {
                    availableOffices.Add(countyImprovementData);
                }
                else
                {
                    PopulationData populationData = countyImprovementData.populationAtImprovement[0];
                    if (populationData.isHero == false)
                    {
                        availableOffices.Add(countyImprovementData);
                    }
                }
            }
            return availableOffices;    
        }

        public static Godot.Collections.Array<PopulationData>
            GetListOfAvailableHeroResearchers()
        {
            Godot.Collections.Array<PopulationData> availableResearchers = [];
            foreach (CountyData countyData in Globals.Instance.playerFactionData.countiesFactionOwns)
            {
                foreach (PopulationData populationData in countyData.heroesInCountyList)
                {
                    if (populationData.currentResearchItemData == null && populationData.activity
                        != AllEnums.Activities.Move)
                    {
                        availableResearchers.Add(populationData);
                    }
                }
            }
            return availableResearchers;
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
        public void RemoveResearcher(PopulationData populationData)
        {
            ResearchControl.Instance.assignedResearchers.Remove(populationData);
            populationData.currentResearchItemData = null;
            // If the population isn't a hero then they must be working at a research office,
            // thus we need to make their activity be work.
            if (populationData.isHero == false)
            {
                populationData.UpdateActivity(AllEnums.Activities.Work);
            }
        }

        /// <summary>
        /// There is no learning check for population random research.
        /// </summary>
        public static void GeneratePassiveResearch(Godot.Collections.Array<PopulationData> researchers)
        {
            foreach (PopulationData populationData in researchers)
            {
                if (populationData.passiveResearchItemData == null)
                {
                    return;
                }

                if (SkillData.Check(populationData
                    , populationData.skills[populationData.passiveResearchItemData.skill].skillLevel
                    , populationData.skills[populationData.passiveResearchItemData.skill].attribute
                    , false) == true)
                {
                    populationData.passiveResearchItemData.AmountOfResearchDone
                        += Globals.Instance.passiveResearchIncrease + Globals.Instance.passiveResearchBonus;
                }
                else
                {
                    populationData.passiveResearchItemData.AmountOfResearchDone
                        += Globals.Instance.passiveResearchIncrease;
                }
                /*
                GD.Print($"County Population: {populationData.location} {populationData.firstName}" +
                    $" {populationData.passiveResearchItemData.researchName}: " +
                    $"{populationData.passiveResearchItemData.AmountOfResearchDone}");
                */

            }
        }


    }
}