using Godot;
using System;

namespace PlayerSpace
{
    public partial class AllResearch : Node
    {
        public static AllResearch Instance { get; private set; }

        [Export] public ResearchItemData[] allResearchItemDatas;

        public override void _Ready()
        {
            Instance = this;
            //AssignCountyResourceConstructCostsToCountyImprovements();
            //AssignFactionResourceContructCostsToCountyImprovements();
        }

        private void AssignCountyResourceConstructCostsToCountyImprovements()
        {
            // Basic Production - Fisher's Shed - Construction Cost - Wood - 1
            allResearchItemDatas[0].countyImprovementDatas[0].countyResourceConstructionCost
                .Add(AllCountyResources.Instance.GetCountyResourceData(AllEnums.CountyResourceType.Wood), 1);
            // Basic Production - Fisher's Shed - Construction Cost - Remnants - 1
            allResearchItemDatas[0].countyImprovementDatas[0].countyResourceConstructionCost
                .Add(AllCountyResources.Instance.GetCountyResourceData(AllEnums.CountyResourceType.Remnants), 1);
        }

        private void AssignFactionResourceContructCostsToCountyImprovements()
        {
            // Basic Production - Fisher's Shed - Construction Cost - Influence - 1
            allResearchItemDatas[0].countyImprovementDatas[0].factionResourceConstructionCost
                .Add(AllFactionResources.Instance.GetFactionResourceData(AllEnums.FactionResourceType.Influence), 1);
        }
    }
}