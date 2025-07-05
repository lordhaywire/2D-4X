using System.Collections.Generic;
using System.Linq;
using Godot;

namespace PlayerSpace;

public partial class AllResearch : Node
{
    public static AllResearch Instance { get; private set; }

    private string researchDirectory = "res://Resources/ResearchItems/";
    public List<ResearchItemData> allResearchItemDatas;

    public override void _Ready()
    {
        Instance = this;
        
        allResearchItemDatas = Globals.Instance.ReadResourcesFromDisk(researchDirectory).Cast<ResearchItemData>().ToList();
    }
}