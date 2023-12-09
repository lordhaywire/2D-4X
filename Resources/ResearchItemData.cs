using Godot;
using System;

namespace PlayerSpace
{
    public partial class ResearchItemData : Resource
    {
        public string researchName;
        public string description;

        public CountyImprovementData countyImprovementData;
        public bool isResearchDone;

        public ResearchItemData(string researchName, string description, CountyImprovementData isCountyImprovement, bool isResearchDone)
        {
            this.researchName = researchName;
            this.description = description;

            this.countyImprovementData = isCountyImprovement;
            this.isResearchDone = isResearchDone;
        }
    }
}