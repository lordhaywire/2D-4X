using Godot;
using PlayerSpace;
using System;
using System.Collections.Generic;

namespace GlobalSpace
{
    public partial class CountyResourcesAutoLoad : Node
    {
        public static CountyResourcesAutoLoad Instance { get; private set; }

        private string pathToCountyDatas = "res://Resources/Counties/";
        public List<CountyData> countyDatas = new();

        public override void _Ready()
        {
            Instance = this;

            GD.Print("This is the autoload thing.");
            LoadCountyResources();
        }

        private void LoadCountyResources()
        {
            DirAccess directory = DirAccess.Open(pathToCountyDatas);
            if (directory != null)
            {
                directory.ListDirBegin();
                string[] files = directory.GetFiles();
                if (files.Length > 0)
                {
                    // Load county resources from disk.
                    foreach (string name in files)
                    {
                        CountyData countyData = (CountyData)GD.Load(pathToCountyDatas + name);
                        countyDatas.Add(countyData);
                        //GD.Print("County that was added: " + countyData.countyName);
                    }
                }
                else
                {
                    GD.Print($"No files in {pathToCountyDatas}");
                }
            }
            else
            {
                GD.Print($"{pathToCountyDatas} directory is missing.");
            }
        }
    }
}