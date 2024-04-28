using Godot;
using System;
using System.Diagnostics.Metrics;
using System.Linq;

namespace PlayerSpace
{
    public partial class CountyGeneration : Node
    {
        private int perishable;
        private int nonperishable;
        public override void _Ready()
        {
            AssignFactionDataToCountyData();
            GenerateBuildings();
            AssignCountyDataToFaction();
            SubscribeToCountyHeroLists();
            UpdateResources();
            UpdateStorage();
        }

        private void UpdateResources()
        {
            // Assign a copy of each resource to each county.
            foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
                CopyAndAssignResources(county, AllResources.Instance.allResources);
            }
        }

        private void CopyAndAssignResources(County county, ResourceData[] resources)
        {
            perishable = 0;
            nonperishable = 0;

            for (int i = 0; i < resources.Length; i++)
            {
                if (resources[i].perishable)
                {
                    county.countyData.perishableResources[perishable] = (ResourceData)resources[i].Duplicate();
                    perishable++;
                }
                else
                {
                    county.countyData.nonperishableResources[nonperishable] = (ResourceData)resources[i].Duplicate();
                    nonperishable++;
                }
            }
            GD.Print($"Perishable: {perishable}, Nonperishable: {nonperishable}");
            SetMaxStorage(county, county.countyData.perishableResources);
            SetMaxStorage(county, county.countyData.nonperishableResources);
        }

        private void SetMaxStorage(County county, ResourceData[] resources)
        {
            foreach (ResourceData resource in resources)
            {
                if (resource.perishable)
                {
                    resource.maxAmount = Globals.Instance.startingPerishableStorage / resources.Length;
                }
                else
                {
                    resource.maxAmount = Globals.Instance.startingNonperishableStorage / resources.Length;
                }
                GD.Print($"{county.countyData.countyName} - {resource.resourceName}: " +
                        $"{resource.maxAmount}");
            }
        }

        private void UpdateStorage()
        {
            foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
            {
                county.countyData.perishableStorage = Globals.Instance.startingPerishableStorage;
                county.countyData.nonperishableStorage = Globals.Instance.startingNonperishableStorage;
                GD.Print($"{county.countyData.countyName} has {county.countyData.perishableStorage} perishable storage.");
            }
        }

        private void SubscribeToCountyHeroLists()
        {
            foreach (County selectCounty in Globals.Instance.countiesParent.GetChildren())
            {
                selectCounty.countyData.herosInCountyList.ItemAdded += (sender, item) => Globals.Instance.AddToFactionHeroList(item);
            }
        }

        // This is just temporary until we set up random faction generation.
        private void AssignFactionDataToCountyData()
        {
            // Cowlitz
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(0);
            //GD.Print("Assigning Faction data: " + Globals.Instance.factionDatas[0].factionName);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[0];
            //GD.Print("Assigned Faction Data: " + selectCounty.countyData.factionData.factionName);
            // Tillamook
            selectCounty = (County)Globals.Instance.countiesParent.GetChild(1);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[1];
            // Douglas
            selectCounty = (County)Globals.Instance.countiesParent.GetChild(2);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[1];
            // Portland
            selectCounty = (County)Globals.Instance.countiesParent.GetChild(3);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[3];
            // Wasco
            selectCounty = (County)Globals.Instance.countiesParent.GetChild(4);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[3];
            // Harney
            selectCounty = (County)Globals.Instance.countiesParent.GetChild(5);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[1];
            // Umatilla
            selectCounty = (County)Globals.Instance.countiesParent.GetChild(6);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[2];
        }


        private void AssignCountyDataToFaction()
        {
            // This goes through every county and adds itself to the faction data that is already assigned to the county.
            foreach (County selectCounty in Globals.Instance.countiesParent.GetChildren())
            {
                selectCounty.countyData.factionData.countiesFactionOwns.Add(selectCounty.countyData);
                //GD.Print($"Faction: {selectCounty.countyData.factionData.factionName} {selectCounty.countyData.countyName}");
            }
        }

        private void GenerateBuildings()
        {
            foreach (County selectCounty in Globals.Instance.countiesParent.GetChildren())
            {
                //GD.Print("County Generation: " + selectCounty.Name);
                //GD.Print("County Data: " + selectCounty.countyData.countyName);
                //GD.Print("Faction Data: " + selectCounty.countyData.factionData.factionName);

                foreach (ResearchItemData researchItemData in selectCounty.countyData.factionData.researchItems)
                {
                    if (researchItemData.isResearchDone == true && researchItemData.countyImprovementDatas.Length > 0)
                    {
                        foreach (CountyImprovementData countyImprovementData in researchItemData.countyImprovementDatas)
                        {
                            selectCounty.countyData.allCountyImprovements
                                .Add((CountyImprovementData)countyImprovementData.Duplicate());
                            //GD.Print($"{selectCounty.countyData.countyName} improvement: {countyImprovementData.improvementName}");
                        }
                    }
                }
            }
        }
    }
}