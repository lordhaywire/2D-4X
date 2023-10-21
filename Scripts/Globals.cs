using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class Globals : Node2D
    {
        public static Globals Instance { get; private set; }

        [Export] public Node2D countyParent;
        public int researchClicked;
        [Export] public bool playerControlsEnabled = true;

        [Export] public Control countyInfoControl;
        [Export] public Label countyNameLabel;

        string listsPath = "res://Lists/";
        string maleNamesPath = "Male Names.txt";
        string femaleNamesPath = "Female Names.txt";
        string lastNamesPath = "Last Names.txt";

        public List<string> maleNames = new();
        public List<string> femaleNames = new();
        public List<string> lastNames = new();

        public override void _Ready()
        {
            Instance = this;
            LoadNames();
        }

        private void LoadNames()
        {
            //maleFirstNames = null;
            //femaleFirstNames = null;
            //lastNames = null;

            // Load all the demon resources from disk into the possibleDemons list.
            using var directory = DirAccess.Open(listsPath);
            if (directory.DirExists(listsPath))
            {
                using var maleFile = FileAccess.Open(listsPath + maleNamesPath, FileAccess.ModeFlags.Read);
                while(maleFile.GetPosition() < maleFile.GetLength())
                {
                    maleNames.Add(maleFile.GetLine());
                    //GD.Print("First Name: " + maleNames[^1]);
                }
                using var femaleFile = FileAccess.Open(listsPath + femaleNamesPath, FileAccess.ModeFlags.Read);
                while (femaleFile.GetPosition() < femaleFile.GetLength())
                {
                    femaleNames.Add(femaleFile.GetLine());
                    //GD.Print("First Name: " + femaleNames[^1]);
                }
                using var lastNameFile = FileAccess.Open(listsPath + lastNamesPath, FileAccess.ModeFlags.Read);
                while (lastNameFile.GetPosition() < lastNameFile.GetLength())
                {
                    lastNames.Add(lastNameFile.GetLine());
                    //GD.Print("Last Name: " + lastNames[^1]);
                }
            }
            else
            {
                GD.Print("[rainbows]Directory doesn't exist!");
            }
        }
    }
}

