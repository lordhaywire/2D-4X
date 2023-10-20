using Godot;
using System;

namespace PlayerSpace
{
    public partial class ResearchItem : Resource
    {
        public string researchName;
        public string description;

        public bool isBuilding;
        public bool isResearchDone;

        public ResearchItem(string researchName, string description, bool isBuilding, bool isResearchDone)
        {
            this.researchName = researchName;
            this.description = description;

            this.isBuilding = isBuilding;
            this.isResearchDone = isResearchDone;
        }
    }
}