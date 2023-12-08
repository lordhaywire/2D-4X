using Godot;
using System;

namespace PlayerSpace
{
	public partial class CountyGeneration : Node
	{
		public override void _Ready()
		{
			GenerateBuildings();
		}

        private void GenerateBuildings()
        {
            foreach(SelectCounty selectCounty in Globals.Instance.countiesParent.GetChildren())
			{
				foreach(ResearchItemData researchItem in selectCounty.countyData.faction.researchItems)
				{
					if(researchItem.countyImprovement != null)
					{
						selectCounty.countyData.countyImprovements.Add(researchItem.countyImprovement);
						GD.Print($"{selectCounty.countyData.countyName} improvement: {researchItem.countyImprovement.improvementName}");
					}
				}
			}
        }
    }
}