using Godot;

namespace PlayerSpace
{
	public partial class CountyGeneration : Node
	{
		public override void _Ready()
		{
			AssignFactionDataToCountyData();
			GenerateBuildings();
			AssignCountyDataToFaction();
		}

		// This is just temporary until we set up random faction generation.
        private void AssignFactionDataToCountyData()
        {
			// Cowlitz
			SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(0);
			GD.Print("Assigning Faction data: " + Globals.Instance.factionDatas[0].factionName);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[0];
			GD.Print("Assigned Faction Data: " + selectCounty.countyData.factionData.factionName);
            // Tillamook
			selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(1);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[1];
            // Douglas
			selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(2);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[1];
			// Portland
            selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(3);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[3];
            // Wasco
            selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(4);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[3];
			// Harney
            selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(5);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[2];
			// Umatilla
            selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(6);
            selectCounty.countyData.factionData = Globals.Instance.factionDatas[2];
        }

		
        private void AssignCountyDataToFaction()
        {
            // I don't know what the flying fuck this is.  It seems to be adding every county to the factionData.
			foreach (SelectCounty selectCounty in Globals.Instance.countiesParent.GetChildren())
			{
				selectCounty.countyData.factionData.countiesFactionOwns.Add(selectCounty.countyData);
            }
        }
		

        private void GenerateBuildings()
        {
            foreach(SelectCounty selectCounty in Globals.Instance.countiesParent.GetChildren())
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