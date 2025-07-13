using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using PlayerSpace;

namespace AutoloadSpace;

public partial class Autoload : Node
{
    public static Autoload Instance { get; private set; }

    private string activitiesDirectory = "res://Resources/Activities/";
    private string attributesDirectory = "res://Resources/Attributes/";
    private string perksDirectory = "res://Resources/Perks/";
    private string goodsDirectory = "res://Resources/Goods/";
    private string researchDirectory = "res://Resources/ResearchItems/";
    private string interestsDirectory = "res://Resources/Interests/";
    private string skillDirectory = "res://Resources/Skills/";

    private string rootEventExplorationDirectory = "res://Resources/Story Events/Exploration Events/";

    public List<ActivityData> allActivityData = [];
    public List<AttributeData> allAttributeData = [];
    public List<PerkData> allPerkData = [];
    public List<GoodData> allGoodData = [];
    public List<ResearchItemData> allResearchItemData = [];
    public List<InterestData> allInterestData = []; // Todo: Figure out why this can be private.  It should be used in populationGeneration.
    public List<SkillData> allSkillData = [];

    public readonly System.Collections.Generic.Dictionary<AllEnums.Terrain, List<StoryEventData>> eventsByTerrainDictionary = [];
    
    public FactionData playerFactionData;
    [Export] public Godot.Collections.Array<FactionData> allFactionDataList = [];

    public override void _Ready()
    {
        Instance = this;

        allActivityData = ReadResourcesFromDisk(activitiesDirectory).Cast<ActivityData>().ToList();
        allAttributeData = ReadResourcesFromDisk(attributesDirectory).Cast<AttributeData>().ToList();
        allPerkData = ReadResourcesFromDisk(perksDirectory).Cast<PerkData>().ToList();
        allGoodData = ReadResourcesFromDisk(goodsDirectory).Cast<GoodData>().ToList();
        allResearchItemData = ReadResourcesFromDisk(researchDirectory).Cast<ResearchItemData>().ToList();
        allInterestData = ReadResourcesFromDisk(interestsDirectory).Cast<InterestData>().ToList();
        allSkillData = ReadResourcesFromDisk(skillDirectory).Cast<SkillData>().ToList();

        GetAllExplorationEventsFromDisk();
        //TestPrintAllResourceNames();
    }


    private void TestPrintAllResourceNames()
    {
        foreach (KeyValuePair<AllEnums.Terrain, List<StoryEventData>> keyValuePair in eventsByTerrainDictionary)
        {
            foreach (StoryEventData testStoryEventData in keyValuePair.Value)
            {
                if (keyValuePair.Key == AllEnums.Terrain.Ruin)
                {
                    GD.Print(testStoryEventData.storyEventTitle);
                }
            }
        }
    }

    public Godot.Collections.Array<Resource> ReadResourcesFromDisk(string path)
    {
        Godot.Collections.Array<Resource> resources = [];
        DirAccess dirAccess = DirAccess.Open(path);

        if (dirAccess != null && dirAccess.ListDirBegin() == Error.Ok)
        {
            string fileName;

            while ((fileName = dirAccess.GetNext()) != "")
            {
                if (dirAccess.CurrentIsDir() || (!fileName.EndsWith(".tres") && !fileName.EndsWith(".res")))
                    continue;

                string filePath = path + fileName;
                Resource readResource = ResourceLoader.Load(filePath);
                GD.Print($"Loaded Resource: {filePath}");
                resources.Add(readResource);
            }

            dirAccess.ListDirEnd(); // Always close the directory listing
        }
        else
        {
            GD.PrintErr("Failed to open directory: " + path);
        }

        return resources;
    }

    private void GetAllExplorationEventsFromDisk()
    {
        DirAccess rootDir = DirAccess.Open(rootEventExplorationDirectory);
        if (rootDir == null)
        {
            GD.PrintErr($"Failed to open root exploration directory: {rootEventExplorationDirectory}");
            return;
        }

        rootDir.ListDirBegin();

        string folderName = rootDir.GetNext();
        while (!string.IsNullOrEmpty(folderName))
        {
            // Ignore non-folders and system entries
            if (folderName == "." || folderName == ".." || !rootDir.CurrentIsDir())
            {
                folderName = rootDir.GetNext();
                continue;
            }

            // Try to parse the folder name as an enum
            if (!Enum.TryParse(folderName, ignoreCase: true, out AllEnums.Terrain terrain))
            {
                GD.PrintErr($"Invalid terrain folder (not in enum): {folderName}");
                folderName = rootDir.GetNext();
                continue;
            }

            string fullFolderPath = rootEventExplorationDirectory + folderName + "/";

            if (!eventsByTerrainDictionary.ContainsKey(terrain))
                eventsByTerrainDictionary[terrain] = [];

            DirAccess dirAccess = DirAccess.Open(fullFolderPath);
            if (dirAccess == null)
            {
                GD.PrintErr($"Failed to open subdirectory: {fullFolderPath}");
                folderName = rootDir.GetNext();
                continue;
            }

            dirAccess.ListDirBegin();
            string fileName = dirAccess.GetNext();
            while (!string.IsNullOrEmpty(fileName))
            {
                if (!dirAccess.CurrentIsDir() && (fileName.EndsWith(".tres") || fileName.EndsWith(".res")))
                {
                    string filePath = fullFolderPath + fileName;
                    Resource resource = ResourceLoader.Load(filePath);
                    if (resource is StoryEventData storyEvent)
                    {
                        eventsByTerrainDictionary[terrain].Add(storyEvent);
                        GD.Print($"Loaded {terrain} story event: {filePath}");
                    }
                    else
                    {
                        GD.PrintErr($"Failed to load or cast: {filePath}");
                    }
                }

                fileName = dirAccess.GetNext();
            }

            dirAccess.ListDirEnd();

            folderName = rootDir.GetNext();
        }

        rootDir.ListDirEnd();
    }

    /// <summary>
    /// Since there is an enum of None (which is zero when parsed to an int), we need to subtract
    /// 1 from the CountyGoodType when getting the good with the AllEnums from the allGoods
    /// array.
    /// </summary>
    /// <param name="goodType"></param>
    /// <returns></returns>
    private GoodData GetCorrectGoodData(AllEnums.CountyGoodType goodType)
    {
        GoodData correctGoodData = allGoodData[(int)goodType - 1];
        return correctGoodData;
    }

    public InterestData GetRandomInterest()
    {
        Random random = new();
        if (allInterestData.Count == 0)
        {
            return null; // Or handle this case as appropriate
        }

        int randomIndex = random.Next(0, allInterestData.Count);
        return allInterestData[randomIndex];
    }

    public List<StoryEventData> GetEventsForTerrain(AllEnums.Terrain terrain)
    {
        return eventsByTerrainDictionary.TryGetValue(terrain, out var list) ? list : [];
    }
}