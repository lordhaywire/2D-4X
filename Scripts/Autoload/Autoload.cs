using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using PlayerSpace;

namespace AutoloadSpace;

public partial class Autoload : Node
{
    public static Autoload Instance { get; private set; }

    private const string CountiesDirectory = "res://Resources/Counties/";
    private const string ImprovementsDirectory = "res://Resources/County Improvements/";
    private const string ActivitiesDirectory = "res://Resources/Activities/";
    private const string AttributesDirectory = "res://Resources/Attributes/";
    private const string PerksDirectory = "res://Resources/Perks/";
    private const string GoodsDirectory = "res://Resources/Goods/";
    private const string ResearchDirectory = "res://Resources/ResearchItems/";
    private const string InterestsDirectory = "res://Resources/Interests/";
    private const string SkillsDirectory = "res://Resources/Skills/";

    private const string RootEventExplorationDirectory = "res://Resources/Story Events/Exploration Events/";

    public List<CountyData> allCountyData = [];
    public List<CountyImprovementData> allCountyImprovementData = [];
    public List<ActivityData> allActivityData = [];
    public List<AttributeData> allAttributeData = [];
    public List<PerkData> allPerkData = [];
    public List<GoodData> allGoodData = [];
    public List<ResearchItemData> allResearchItemData = [];
    public List<InterestData> allInterestData = []; // Todo: Figure out why this can be private.  It should be used in populationGeneration.
    public List<SkillData> allSkillData = [];

    public readonly Dictionary<AllEnums.Terrain, List<StoryEventData>> eventsByTerrainDictionary = [];
    
    public FactionData playerFactionData;

    // Name Variables
    private string listsPath = "Lists/";
    private string maleNamesPath = "MaleNames.txt";
    private string femaleNamesPath = "FemaleNames.txt";
    private string lastNamesPath = "LastNames.txt";
    
    public readonly List<string> maleNames = [];
    public readonly List<string> femaleNames = [];
    public readonly List<string> lastNames = [];
    
    // County Generation Variables
    public int maxScavengeableScrap = 10000;
    public int maxScavengeableFood = 10000;
    public int startingPerishableStorage = 2000;
    public int startingNonperishableStorage = 2000;
    public int startingAmountOfEachGood = 100;
    
    // Population Generation Variables
    public int minStartingAge = 18;
    public int maxStartingAge = 61;
    public int startingHitPoints = 10;
    public int minimumCountyPop = 1;
    public int maximumCountyPop = 4;
    public int totalCapitolPop = 20;
    
    // Exploration Variables
    public int numberOfPrimaryTerrainEvents = 10;
    public int numberOfSecondaryTerrainEvents = 6;
    public int numberOfTertiaryTerrainEvents = 3;
    
    // These two are populated from AllResources at Ready. Is this true anymore?
    public int numberOfPerishableGoods; // Total perishable goods
    public int numberOfNonperishableGoods; // Total nonperishable goods
    
    public override void _Ready()
    {
        Instance = this;
        allCountyData = ReadResourcesFromDisk(CountiesDirectory).Cast<CountyData>().ToList();
        allActivityData = ReadResourcesFromDisk(ActivitiesDirectory).Cast<ActivityData>().ToList();
        allAttributeData = ReadResourcesFromDisk(AttributesDirectory).Cast<AttributeData>().ToList();
        allPerkData = ReadResourcesFromDisk(PerksDirectory).Cast<PerkData>().ToList();
        allGoodData = ReadResourcesFromDisk(GoodsDirectory).Cast<GoodData>().ToList();
        allResearchItemData = ReadResourcesFromDisk(ResearchDirectory).Cast<ResearchItemData>().ToList();
        allInterestData = ReadResourcesFromDisk(InterestsDirectory).Cast<InterestData>().ToList();
        allSkillData = ReadResourcesFromDisk(SkillsDirectory).Cast<SkillData>().ToList();
        allCountyImprovementData = ReadResourcesFromDisk(ImprovementsDirectory).Cast<CountyImprovementData>().ToList();

        GetAllExplorationEventsFromDisk();
        CountGoods();
        LoadNames();
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

    private void LoadNames()
    {
        // Load all the names from disk.

        // I think the variable can be used if we open up the root directory first.
        // Right now this code is doing nothing except the GD.Print stuff.
        //listDirectory = ProjectSettings.LocalizePath(listsPath); 
        GD.Print(OS.HasFeature("editor") ? "Is in the editor!!!" : "Is not in the editor!");

        //listDirectory = ProjectSettings.LocalizePath(listsPath);
        DirAccess directory = DirAccess.Open("res://");
        if (directory.DirExists("res://Lists/"))
        {
            using var maleFile = FileAccess.Open("res://Lists/MaleNames.txt", FileAccess.ModeFlags.Read);//(listsPath + maleNamesPath, FileAccess.ModeFlags.Read);
            while (maleFile.GetPosition() < maleFile.GetLength())
            {
                maleNames.Add(maleFile.GetLine());
            }
            using var femaleFile = FileAccess.Open("res://Lists/FemaleNames.txt", FileAccess.ModeFlags.Read); //(listsPath + femaleNamesPath, FileAccess.ModeFlags.Read);
            while (femaleFile.GetPosition() < femaleFile.GetLength())
            {
                femaleNames.Add(femaleFile.GetLine());
            }
            using var lastNameFile = FileAccess.Open("res://Lists/LastNames.txt", FileAccess.ModeFlags.Read); //(listsPath + lastNamesPath, FileAccess.ModeFlags.Read);
            while (lastNameFile.GetPosition() < lastNameFile.GetLength())
            {
                lastNames.Add(lastNameFile.GetLine());
            }
            //GD.Print("Names have been loaded.");
        }
        /*
        else
        {
            GD.Print("Directory doesn't exist! " + listDirectory);
        }
        */

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
                resources.Add(readResource.Duplicate());
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
        DirAccess rootDir = DirAccess.Open(RootEventExplorationDirectory);
        if (rootDir == null)
        {
            GD.PrintErr($"Failed to open root exploration directory: {RootEventExplorationDirectory}");
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

            string fullFolderPath = RootEventExplorationDirectory + folderName + "/";

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
    
    private void CountGoods()
    {
        int perishable = 0;
        int nonperishable = 0;

        foreach (GoodData resourceData in Autoload.Instance.allGoodData)
        {
            switch (resourceData.perishable)
            {
                case AllEnums.Perishable.Perishable:
                    perishable++;
                    break;
                case AllEnums.Perishable.Nonperishable:
                    nonperishable++;
                    break;
            }
        }
        numberOfPerishableGoods = perishable;
        numberOfNonperishableGoods = nonperishable;
    }
}