using System;
using System.Collections.Generic;
using Godot;

namespace PlayerSpace
{
    public partial class StoryEventList : Node
    {
        public static StoryEventList Instance { get; private set; }

        [Export] public StoryEventData[] storyEventData;

        public readonly Dictionary<AllEnums.Terrain, List<StoryEventData>> eventsByTerrainDictionary = [];
        
        private string rootEventExplorationDirectory = "res://Resources/Story Events/Exploration Events/";

        public override void _Ready()
        {
            Instance = this;
            GetAllExplorationEventsFromDisk();
            TestPrintAllResourceNames();
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

        public void GetAllExplorationEventsFromDisk()
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

        public List<StoryEventData> GetEventsForTerrain(AllEnums.Terrain terrain)
        {
            List<StoryEventData> list;
            return eventsByTerrainDictionary.TryGetValue(terrain, out list) ? list : new List<StoryEventData>();
        }
    }
}