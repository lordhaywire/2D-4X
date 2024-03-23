using Godot;
using System;

namespace PlayerSpace
{
	public partial class CountyGeneration : Node
	{
		public override void _Ready()
		{
			AssignFactionDataToCountyData();
			GenerateBuildings();
			AssignCountyDataToFaction();
            SubscribeToCountyHeroLists();
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
            foreach(County selectCounty in Globals.Instance.countiesParent.GetChildren())
			{
				/*
				GD.Print("County Generation: " + selectCounty.Name);
				GD.Print("County Data: " + selectCounty.countyData.countyName);
				GD.Print("Faction Data: " + selectCounty.countyData.factionData.factionName);
				*/
				foreach(ResearchItemData researchItemData in selectCounty.countyData.factionData.researchItems)
				{
					if(researchItemData.countyImprovementData != null)
					{
						selectCounty.countyData.allCountyImprovements.Add(researchItemData.countyImprovementData);
						//GD.Print($"{selectCounty.countyData.countyName} improvement: {researchItemData.countyImprovementData.improvementName}");
					}
				}
			}
        }
    }
}