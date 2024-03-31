using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class ResearchItemData : Resource
    {
        [Export] public string researchName;
        [Export] public string description;

        [Export] public bool isResearchDone;
        [Export] public CountyImprovementData[] countyImprovementDatas;

        /*
        public ResearchItemData(string researchName, string description, CountyImprovementData[] countyImprovementDatas, bool isResearchDone)
        {
            this.researchName = researchName;
            this.description = description;

            this.countyImprovementDatas = countyImprovementDatas;
            this.isResearchDone = isResearchDone;
        }
        */
    }
}